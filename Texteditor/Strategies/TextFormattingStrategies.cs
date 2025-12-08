using Texteditor.Strategies;

namespace Texteditor.Strategies
{
    /// <summary>
    /// Strategy for converting text to uppercase
    /// </summary>
    public class UpperCaseStrategy : ITextFormattingStrategy
    {
        public string Description => "Перетворити в великі літери";

        public string FormatText(string text)
        {
            return text.ToUpper();
        }
    }

    /// <summary>
    /// Strategy for converting text to lowercase
    /// </summary>
    public class LowerCaseStrategy : ITextFormattingStrategy
    {
        public string Description => "Перетворити в малі літери";

        public string FormatText(string text)
        {
            return text.ToLower();
        }
    }

    /// <summary>
    /// Strategy for capitalizing first letter of each word
    /// </summary>
    public class TitleCaseStrategy : ITextFormattingStrategy
    {
        public string Description => "Перша літера кожного слова велика";

        public string FormatText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var words = text.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                }
            }
            return string.Join(" ", words);
        }
    }
}