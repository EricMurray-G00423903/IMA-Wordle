using System.Text;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace Wordle;

public partial class GamePage : ContentPage
{
    private int currentRowIndex = 0;
    private int currentRowPosition = 0;
    private string currentWordToGuess;
    private List<GameHistoryEntry> gameHistory = new List<GameHistoryEntry>();
    private StringBuilder cumulativeEmojiGrid = new StringBuilder();

    public GamePage()
    {
        InitializeComponent();
        InitializeGameGrid();
        InitializeAlphabetButtons();
        WebViewUtility.LoadHtmlContent(backgroundWebView, AppSettings.IsDarkMode);
        LoadRandomWord();
    }

    private async Task LoadRandomWordFromList()
    {
        var localFolder = FileSystem.AppDataDirectory;
        var localFilePath = Path.Combine(localFolder, "words.txt");

        if (File.Exists(localFilePath))
        {
            var allWords = await File.ReadAllLinesAsync(localFilePath);
            if (allWords.Length > 0)
            {
                var random = new Random();
                currentWordToGuess = allWords[random.Next(allWords.Length)].ToUpper();
            }
        }
    }

    private async void LoadRandomWord()
    {
        await LoadRandomWordFromList();
    }

    private void InitializeGameGrid()
    {
        for (int row = 0; row < 6; row++) // 6 rows
        {
            for (int col = 0; col < 5; col++) // 5 columns
            {
                var entry = new Entry
                {
                    Placeholder = " ",
                    MaxLength = 1,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(5),
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Entry)),
                    BackgroundColor = Color.FromArgb("#FFFFFF"), // white background for the entry
                    IsReadOnly = true // This will prevent the keyboard from popping up
                };

                // Disable focus for the entry
                entry.Focused += (sender, args) => ((Entry)sender).Unfocus();

                // Set the row and column for each entry
                Grid.SetRow(entry, row);
                Grid.SetColumn(entry, col);

                // Add the entry to the grid
                wordGrid.Children.Add(entry);
            }
        }

        // Set the background color of the grid to differentiate from the background WebView
        wordGrid.BackgroundColor = Color.FromArgb("#00000000"); // Transparent background
    }

    private void InitializeAlphabetButtons()
    {
        string alphabet = "QWERTYUIOPASDFGHJKLZXCVBNM";
        foreach (char letter in alphabet)
        {
            var button = new Button
            {
                Text = letter.ToString(),
                WidthRequest = 40,
                HeightRequest = 40
            };

            // Set the style dynamically
            if (Application.Current.Resources.TryGetValue("ButtonStyle", out var buttonStyle))
            {
                button.Style = (Style)buttonStyle;
            }

            button.Clicked += OnLetterButtonClicked;
            alphabetButtons.Children.Add(button);
        }
    }

    private void OnLetterButtonClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == null) return;

        // Check if the current row is complete (5 letters)
        if (currentRowPosition < 5)
        {
            var entry = wordGrid.Children[currentRowIndex * 5 + currentRowPosition] as Entry;
            if (entry != null)
            {
                entry.Text = button.Text;
                currentRowPosition++; // Move to the next position

                // Optionally focus on the next Entry if not at the end of the row
                if (currentRowPosition < 5)
                {
                    FocusNextEntry(entry);
                }
            }
        }
        // If the row is complete, you could automatically invoke the submit action or alert the user
        else
        {
            // Invoke submit or show an alert
            // OnSubmitGuessClicked(null, null); // Uncomment to auto-submit
        }
    }

    private void FocusNextEntry(Entry currentEntry)
    {
        // Find the index of the current entry
        var index = wordGrid.Children.IndexOf(currentEntry);
        if (index >= 0 && index < wordGrid.Children.Count - 1)
        {
            var nextEntry = wordGrid.Children[index + 1] as Entry;
            nextEntry?.Focus();
        }
    }

    private async void OnSubmitGuessClicked(object sender, EventArgs e)
    {
        // Get the current guess from the grid
        string userGuess = GetCurrentGuess();

        // Check if the user has entered 5 letters
        if (userGuess.Length == 5)
        {
            // Compare the guess to the target word
            CompareGuess(userGuess);
        }
        else
        {
            // Shake the grid to indicate the guess is incomplete
            await ShakeGridAsync();
        }
    }

    private string GetCurrentGuess()
    {
        StringBuilder sb = new StringBuilder();
        for (int col = 0; col < 5; col++) // Assuming 5 letters per guess
        {
            if (wordGrid.Children[currentRowIndex * 5 + col] is Entry entry && !string.IsNullOrEmpty(entry.Text))
            {
                sb.Append(entry.Text.ToUpper());
            }
        }
        return sb.ToString();
    }

    private async void CompareGuess(string userGuess)
    {
        bool isGuessCorrect = true;
        int guesses = currentRowIndex + 1;

        for (int i = 0; i < userGuess.Length; i++)
        {
            char guessLetter = userGuess[i];
            Entry entry = wordGrid.Children[currentRowIndex * 5 + i] as Entry;

            // Check if the guessed letter is in the word to guess
            if (currentWordToGuess.Contains(guessLetter))
            {
                // Correct letter in the correct position
                if (currentWordToGuess[i] == guessLetter)
                {
                    entry.BackgroundColor = Colors.Green;
                }
                // Correct letter in the wrong position
                else
                {
                    entry.BackgroundColor = Colors.Orange;
                    isGuessCorrect = false;
                }
            }
            // Letter not in the word to guess
            else
            {
                entry.BackgroundColor = Colors.Gray;
                isGuessCorrect = false;
            }
        }

        cumulativeEmojiGrid.Append(GenerateEmojiGridString(userGuess));

        if (isGuessCorrect)
        {
            await ShowWinDialogAsync();
            SaveGameToHistory(guesses, true);
        }
        else
        {
            // Check if it's the last row and the guess is incorrect
            if (currentRowIndex == 5) // Assuming 6 rows indexed from 0 to 5
            {
                await ShowLoseDialogAsync();
                SaveGameToHistory(guesses, false);
            }
            else
            {
                // Move to the next row for the next guess
                currentRowIndex++;
                currentRowPosition = 0;
                ResetCurrentRow();
            }
        }

        string emojiGridString = GenerateEmojiGridString(userGuess);

    }

    private void SaveGameToHistory(int guesses, bool isWin)
    {
        var historyEntry = new GameHistoryEntry(
            DateTime.Now, // Timestamp
            currentWordToGuess, // Correct word
            guesses, // Number of guesses
            cumulativeEmojiGrid.ToString() // Cumulative emoji grid string
        );

        SaveHistoryToFile(historyEntry);
    }

    private string GenerateEmojiGridString(string userGuess)
    {
        StringBuilder emojiGrid = new StringBuilder();

        for (int i = 0; i < userGuess.Length; i++)
        {
            char guessLetter = userGuess[i];

            // Correct letter in the correct position
            if (currentWordToGuess[i] == guessLetter)
            {
                emojiGrid.Append("G"); // Green
            }
            // Correct letter in the wrong position
            else if (currentWordToGuess.Contains(guessLetter))
            {
                emojiGrid.Append("O"); // Orange
            }
            // Letter not in the word to guess
            else
            {
                emojiGrid.Append("W"); // Wrong
            }
        }

        return emojiGrid.ToString();
    }

    private async void SaveHistoryToFile(GameHistoryEntry newEntry)
    {
        var localPath = GetHistoryFilePath();

        List<GameHistoryEntry> history;
        if (File.Exists(localPath))
        {
            // Read existing history
            var json = await File.ReadAllTextAsync(localPath);
            history = JsonSerializer.Deserialize<List<GameHistoryEntry>>(json) ?? new List<GameHistoryEntry>();
        }
        else
        {
            // No existing history
            history = new List<GameHistoryEntry>();
        }

        // Add new entry to history
        history.Add(newEntry);

        // Write updated history to file
        var updatedJson = JsonSerializer.Serialize(history);
        await File.WriteAllTextAsync(localPath, updatedJson);
    }

    private string GetHistoryFilePath()
    {
        string playerName = Preferences.Get("playerName", string.Empty);
        var fileName = $"{playerName}_history.json";
        return Path.Combine(FileSystem.AppDataDirectory, fileName);
    }

    private async Task ShowWinDialogAsync()
    {
        string message = $"Congratulations! You guessed the word '{currentWordToGuess}' in {currentRowIndex + 1} attempts.";
        bool restartGame = await DisplayAlert("You Win!", message, "Restart Game", "Main Menu");

        if (restartGame)
        {
            RestartGame();
        }
        else
        {
            GoToMainMenu();
        }
    }

    private async Task ShowLoseDialogAsync()
    {
        string message = $"You ran out of guesses. The word was '{currentWordToGuess}'.";
        bool restartGame = await DisplayAlert("Game Over", message, "Restart Game", "Main Menu");

        if (restartGame)
        {
            RestartGame();
        }
        else
        {
            GoToMainMenu();
        }
    }

    private void RestartGame()
    {
        currentRowIndex = 0;
        currentRowPosition = 0;
        cumulativeEmojiGrid.Clear();
        LoadRandomWord();
        ResetGameGrid();
    }

    private void GoToMainMenu()
    {
        // Navigate back to the main menu
        // Assuming you have a navigation mechanism to go back to the main menu
        Navigation.PopAsync();
    }

    private void ResetGameGrid()
    {
        foreach (var child in wordGrid.Children)
        {
            if (child is Entry entry)
            {
                entry.Text = null;
                entry.BackgroundColor = Colors.White;
            }
        }
        // Optionally, reset the state of the alphabet buttons
    }

    private async Task ShakeGridAsync()
    {
        uint timeout = 50;
        await wordGrid.TranslateTo(-15, 0, timeout);
        await wordGrid.TranslateTo(15, 0, timeout);
        await wordGrid.TranslateTo(-10, 0, timeout);
        await wordGrid.TranslateTo(10, 0, timeout);
        await wordGrid.TranslateTo(0, 0, timeout);
    }

    private void ResetCurrentRow()
    {
        for (int col = 0; col < 5; col++)
        {
            var entry = wordGrid.Children[currentRowIndex * 5 + col] as Entry;
            if (entry != null)
            {
                entry.Text = null;
                entry.BackgroundColor = Colors.White; // Set to transparent or the original color
            }
        }
        
    }

    private void OnBackspaceClicked(object sender, EventArgs e)
    {
        if (currentRowPosition > 0)
        {
            currentRowPosition--; // Move back one position
            var entry = wordGrid.Children[currentRowIndex * 5 + currentRowPosition] as Entry;
            if (entry != null)
            {
                entry.Text = null;
                entry.Focus();
            }
        }
        ShakeGridAsync();
    }

}