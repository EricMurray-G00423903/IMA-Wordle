using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wordle
{
    public static class WebViewUtility
    {
        //simple class for changing the colors on the webview background for dark/light mode
        public static void LoadHtmlContent(WebView webView, bool isDarkMode)
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("Wordle.Resources.bg.html");
            using (var reader = new StreamReader(stream))
            {
                var html = reader.ReadToEnd();
                html = html.Replace("{{backgroundColor}}", isDarkMode ? "#000000" : "#FFFFFF")
                           .Replace("{{particleColor}}", isDarkMode ? "#FFFFFF" : "#000000")
                           .Replace("{{lineColor}}", isDarkMode ? "#FFFFFF" : "#000000");

                webView.Source = new HtmlWebViewSource { Html = html };
            }
        }
    }
}
