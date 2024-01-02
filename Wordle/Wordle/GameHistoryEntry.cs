using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wordle
{
    public class GameHistoryEntry
    {
        //custom class for serialising the game history to be written to a users history json file
        public DateTime Timestamp { get; set; }
        public string CorrectWord { get; set; }
        public int NumberOfGuesses { get; set; }
        public string EmojiGrid { get; set; }

        public GameHistoryEntry(DateTime timestamp, string correctWord, int numberOfGuesses, string emojiGrid)
        {
            Timestamp = timestamp;
            CorrectWord = correctWord;
            NumberOfGuesses = numberOfGuesses;
            EmojiGrid = emojiGrid;
        }
    }

}
