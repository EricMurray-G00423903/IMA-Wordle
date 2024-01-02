using System;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

namespace Wordle
{
    public partial class MainPage : ContentPage
    {

        public string PlayerName { get; set; }

        public MainPage()
        {
            PlayerName = Preferences.Get("playerName", "Player");

            InitializeComponent();
            LoadWordList();

            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Load or reload the HTML content when the page appears or reappears
            wordleLabel.TextColor = AppSettings.IsDarkMode ? Color.FromHex("#FFFFFF") : Color.FromHex("#000000");
            WebViewUtility.LoadHtmlContent(backgroundWebView, AppSettings.IsDarkMode);

            // Delay to ensure UI is ready for animations
            await Task.Delay(2000); // Adjust delay as needed

            // Fade in animations
            await welcomeLabel.FadeTo(0, 2000);
            await Task.WhenAll(
                wordleLabel.FadeTo(1, 500),
                newGameButton.FadeTo(1, 500),
                historyButton.FadeTo(1, 500),
                settingsButton.FadeTo(1, 500),
                backgroundWebView.FadeTo(1, 1000));
        }

        private async void LoadWordList() 
        {
            var localFolder = FileSystem.AppDataDirectory;
            var localFilePath = Path.Combine(localFolder, "words.txt");

            if (!File.Exists(localFilePath))
            {
                await DownloadWordList(localFilePath);
            }

        }

        private async Task DownloadWordList(string filePath)
        {
            var client = new HttpClient();
            var wordsUrl = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt";
            //download words.txt
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

        private async void OnNewGameClicked(object sender, EventArgs e)
        {
            // Navigate to the New Game
            await Navigation.PushAsync(new GamePage());
        }

        private async void OnHistoryClicked(object sender, EventArgs e)
        {
            // Navigate to the History page
            await Navigation.PushAsync(new HistoryPage());
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            // Navigate to settings
            await Navigation.PushAsync(new SettingsPage());

        }
    }
}
