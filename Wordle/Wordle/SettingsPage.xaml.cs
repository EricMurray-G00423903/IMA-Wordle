namespace Wordle;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
        WebViewUtility.LoadHtmlContent(backgroundWebView, AppSettings.IsDarkMode);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        WebViewUtility.LoadHtmlContent(backgroundWebView, AppSettings.IsDarkMode);
    }

    private void toggleDarkModeButton_Clicked(object sender, EventArgs e)
    {
        AppSettings.IsDarkMode = !AppSettings.IsDarkMode;
        App.SetTheme(AppSettings.IsDarkMode);
        WebViewUtility.LoadHtmlContent(backgroundWebView, AppSettings.IsDarkMode);
    }

    private async void backToMainButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void logoutButton_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("playername", "");
        Navigation.PushAsync(new WelcomePage());
    }

    private async void clearHistoryButton_Clicked(object sender, EventArgs e)
    {
        // Confirm with the user before deleting
        bool confirmDelete = await DisplayAlert("Clear History",
                                                "Are you sure you want to delete all game history? This action cannot be undone.",
                                                "Delete", "Cancel");
        if (confirmDelete)
        {
            try
            {
                string playerName = Preferences.Get("playerName", string.Empty);
                var fileName = $"{playerName}_history.json";
                var localPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                if (File.Exists(localPath))
                {
                    File.Delete(localPath);
                    // Optionally inform the user of success
                    await DisplayAlert("History Cleared", "Your game history has been successfully cleared.", "OK");
                }
                else
                {
                    // If the file does not exist, inform the user
                    await DisplayAlert("No History", "There is no history to clear.", "OK");
                }
            }
            catch (Exception ex)
            {
                // If there was an error deleting the file, inform the user
                await DisplayAlert("Error", $"An error occurred while clearing history: {ex.Message}", "OK");
            }
        }
    }

}