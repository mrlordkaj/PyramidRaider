using Microsoft.Xna.Framework.Graphics;
using System.Text;

namespace OpenitvnGame.Helpers
{
    public class StringHelper
    {
        public static string WordWrap(SpriteFont spriteFont, string phrase, int maxWidth)
        {
            string[] words = phrase.Split(' ');
            StringBuilder text = new StringBuilder();
            int lineWidth = 0;
            foreach (string word in words)
            {
                string curWord = word + " ";
                int wordWidth = (int)spriteFont.MeasureString(curWord).X;
                if ((lineWidth += wordWidth) < maxWidth) text.Append(curWord);
                else
                {
                    text.Append("\r\n" + curWord);
                    lineWidth = wordWidth;
                }
            }
            return text.ToString();
        }
    }
}
