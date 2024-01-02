using System;
using System.Globalization;
using System.Text;
using Microsoft.Maui.Controls;

namespace Wordle
{
    public class EmojiGridConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string emojiGridString)
            {
                return ConvertEmojiGridStringToEmoji(emojiGridString);
            }
            return string.Empty;
        }

        private string ConvertEmojiGridStringToEmoji(string emojiGridString)
        {
            // use a string builder to build the emoji grid like the one on real wordle 5 emojis per line
            StringBuilder emojiGrid = new StringBuilder();
            for (int i = 0; i < emojiGridString.Length; i++)
            {
                if (i > 0 && i % 5 == 0)
                {
                    emojiGrid.AppendLine(); // Add a new line after every 5 emojis
                }
                emojiGrid.Append(GetEmojiForChar(emojiGridString[i]));
            }
            return emojiGrid.ToString();
        }

        private string GetEmojiForChar(char c)
        {
            //translates the file, structured storage into appropriate emojis
            return c switch
            {
                'G' => "🟩", // Green square emoji
                'O' => "🟧", // Orange square emoji
                'W' => "🟫", // Changed to grey square emoji
                _ => "🟫",   // Default to grey if unknown
            };
        }

        //convert back not really needed but added for visibility
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
