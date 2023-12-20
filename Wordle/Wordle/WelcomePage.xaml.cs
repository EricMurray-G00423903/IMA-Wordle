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
