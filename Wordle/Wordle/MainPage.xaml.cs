using System.Net.Http;
using System.IO;
using System.Threading.Tasks;

namespace Wordle
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            LoadWordList();
        }

        private async void LoadWordList()
        {
            // Check if word list exists
            // If not, download it
            
            var localFolder = FileSystem.AppDataDirectory;
            var localFilePath = Path.Combine(localFolder, "words.txt");

            
            if (!File.Exists(localFilePath))
            {
                await DownloadWordList(localFilePath);
            }

            // Add a delay if you want to show the welcome message for a set time
            await Task.Delay(2000); // e.g., 2 seconds delay

            // Fade out the welcome message
            await welcomeLabel.FadeTo(0, 1000); // Fades out the label over 1 second

            // Fade in the buttons
            await Task.WhenAll(
                newGameButton.FadeTo(1, 500),
                historyButton.FadeTo(1, 500),
                settingsButton.FadeTo(1, 500),
                backgroundWebView.FadeTo(1, 500));
        }

        private async Task DownloadWordList(string filePath)
        {
            var client = new HttpClient();
            var wordsUrl = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt";

            try
            {
                var response = await client.GetAsync(wordsUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    File.WriteAllText(filePath, content);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., network issues)
                Console.WriteLine($"Error downloading word list: {ex.Message}");
            }
        }

        private void OnNewGameClicked(object sender, EventArgs e)
        {
            // Navigate to the New Game page
        }

        private void OnHistoryClicked(object sender, EventArgs e)
        {
            // Navigate to the History page
        }

        private void OnSettingsClicked(object sender, EventArgs e)
        {
            // Navigate to the Settings page
        }
    }
}