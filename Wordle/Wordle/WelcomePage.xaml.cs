namespace Wordle;

public partial class WelcomePage : ContentPage
{
    public WelcomePage()
    {
        InitializeComponent();
    }

    private async void OnPlayButtonClicked(object sender, EventArgs e)
    {
        var playerName = playerNameEntry.Text?.Trim();

        // Check if the player entered a name
        if (!string.IsNullOrEmpty(playerName))
        {
            // Save the player's name in preferences for later use
            Preferences.Set("playerName", playerName);

            // Check if a playername.txt file exists
            var localFolder = FileSystem.AppDataDirectory;
            var playerFile = Path.Combine(localFolder, "playername.txt");

            if (!File.Exists(playerFile))
            {
                // If the file doesn't exist, create it with the player's name
                File.WriteAllText(playerFile, playerName);
            }
            else
            {
                // If the file exists, we could do something else, but for now, do nothing
            }

            // Navigate to the MainPage or another appropriate page
            await Navigation.PushAsync(new MainPage());
        }
        else
        {
            // If no name was entered, display an alert
            await DisplayAlert("No Name", "Please enter your name to continue.", "OK");
        }
    }
}
