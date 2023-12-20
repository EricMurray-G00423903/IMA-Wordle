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
}