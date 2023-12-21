using System.Text.Json;

namespace Wordle;

public partial class HistoryPage : ContentPage
{
    public HistoryPage()
    {
        InitializeComponent();
        WebViewUtility.LoadHtmlContent(backgroundWebView, AppSettings.IsDarkMode);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadHistory();
    }

    private async Task LoadHistory()
    {
        string playerName = Preferences.Get("playerName", string.Empty);
        var fileName = $"{playerName}_history.json";
        var localPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

        if (File.Exists(localPath))
        {
            var json = await File.ReadAllTextAsync(localPath);
            var history = JsonSerializer.Deserialize<List<GameHistoryEntry>>(json);

            if (history != null && history.Count > 0)
            {
                historyListView.ItemsSource = history;
                historyListView.IsVisible = true;
                noHistoryLabel.IsVisible = false;
                newGameButton.IsVisible = false;
            }
            else
            {
                historyListView.IsVisible = false;
                noHistoryLabel.IsVisible = true;
                newGameButton.IsVisible = true;
            }
        }
        else
        {
            historyListView.IsVisible = false;
            noHistoryLabel.IsVisible = true;
            newGameButton.IsVisible = true;
        }
    }


    private async void OnNewGameClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GamePage());
    }

    private async void OnMainMenuClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
