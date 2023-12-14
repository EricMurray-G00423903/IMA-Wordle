using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordle
{
    public static class AppSettings
    {
        private const string DarkModeKey = "IsDarkMode";
        private static bool _isDarkMode = Preferences.Get(DarkModeKey, false);

        public static bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                if (_isDarkMode != value)
                {
                    _isDarkMode = value;
                    Preferences.Set(DarkModeKey, value);
                    MessagingCenter.Send<object, bool>(new object(), "ModeChanged", _isDarkMode);
                }
            }
        }
    }

}
