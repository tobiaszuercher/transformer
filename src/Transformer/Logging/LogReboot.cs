using System.Drawing;
using Colorful;
using Console = Colorful.Console;

namespace Transformer.Logging
{
    public class LogReboot
    {
        public void Info(string text)
        {
            Console.WriteLine(text);
        }

        public void Warn(string text)
        {
            Console.WriteLine(text, Color.Orange);
        }

        public void Error(string text)
        {
            Console.WriteLine(text, Color.Red);
        }

        public void InitStylesheet(string environemnt)
        {
            StyleSheet styleSheet = new StyleSheet(Color.White);
            styleSheet.AddStyle(environemnt, Color.MediumSlateBlue, match => match.ToUpper());

            //Console.WriteLineStyled(storyAboutRain, styleSheet);
        }
    }
}