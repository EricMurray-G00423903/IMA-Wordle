namespace Wordle
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            bool isDarkTheme = AppSettings.IsDarkMode;
            SetTheme(isDarkTheme);
            MainPage = new NavigationPage(new WelcomePage());
        }

        public static void SetTheme(bool isDark)
        {
            if (isDark)
            {
                Current.Resources["ButtonStyle"] = Current.Resources["DarkThemeButton"];
            }
            else
            {
                Current.Resources["ButtonStyle"] = Current.Resources["LightThemeButton"];
            }
        }
    }

}