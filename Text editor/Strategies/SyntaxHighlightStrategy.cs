using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TextEditor;

namespace TextEditor.Strategies
{
    public class SyntaxHighlightStrategy : ITextEditingStrategy
    {
        public string Name => "Підсвічування синтаксису";
        public string Description => "Застосування кольорового кодування до елементів синтаксису";

        private readonly Dictionary<string, SyntaxRule[]> _languageRules;

        public SyntaxHighlightStrategy()
        {
            _languageRules = new Dictionary<string, SyntaxRule[]>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "csharp",
                    new SyntaxRule[]
                    {
                        new SyntaxRule(@"\b(class|interface|struct|enum|namespace|using|public|private|protected|internal|static|virtual|override|abstract|sealed|readonly|const|void|int|string|bool|double|float|char|byte|long|short|decimal|object|var|new|this|base|null|true|false|if|else|while|for|foreach|do|switch|case|default|break|continue|return|try|catch|finally|throw|async|await|yield)\b", Color.Blue),
                        new SyntaxRule("\".*?\"|'.'", Color.Brown),
                        new SyntaxRule("//.*$", Color.Green, RegexOptions.Multiline),
                        new SyntaxRule("/\\*.*?\\*/", Color.Green, RegexOptions.Singleline),
                        new SyntaxRule("\\b\\d+(\\.\\d+)?\\b", Color.Red),
                        new SyntaxRule("\\b[A-Z][a-zA-Z0-9]*\\b", Color.DarkCyan),
                        new SyntaxRule("\\[[^\\]]+\\]", Color.Gray)
                    }
                },
                {
                    "html",
                    new SyntaxRule[]
                    {
                        new SyntaxRule("</?\\w+[^>]*>", Color.Blue),
                        new SyntaxRule("\\w+\\s*=\\s*\"[^\"]*\"", Color.Red),
                        new SyntaxRule("<!--.*?-->", Color.Green, RegexOptions.Singleline),
                        new SyntaxRule("<!DOCTYPE[^>]*>", Color.Purple, RegexOptions.IgnoreCase)
                    }
                },
                {
                    "css",
                    new SyntaxRule[]
                    {
                        new SyntaxRule("^\\s*[.#]?[\\w-]+\\s*(?=\\{)", Color.DarkRed, RegexOptions.Multiline),
                        new SyntaxRule("\\b[\\w-]+\\s*(?=:)", Color.Blue),
                        new SyntaxRule(":\\s*[^;}]+", Color.DarkGreen),
                        new SyntaxRule("/\\*.*?\\*/", Color.Green, RegexOptions.Singleline),
                        new SyntaxRule("#[0-9A-Fa-f]{3,6}|rgb\\([^)]+\\)|rgba\\([^)]+\\)", Color.Purple)
                    }
                },
                {
                    "javascript",
                    new SyntaxRule[]
                    {
                        new SyntaxRule("\\b(function|var|let|const|if|else|while|for|do|switch|case|default|break|continue|return|try|catch|finally|throw|new|this|null|undefined|true|false|typeof|instanceof)\\b", Color.Blue),
                        new SyntaxRule("\".*?\"|'.*?'|`.*?`", Color.Brown),
                        new SyntaxRule("//.*$", Color.Green, RegexOptions.Multiline),
                        new SyntaxRule("/\\*.*?\\*/", Color.Green, RegexOptions.Singleline),
                        new SyntaxRule("\\b\\d+(\\.\\d+)?\\b", Color.Red),
                        new SyntaxRule("/[^/\\n]+/[gimuy]*", Color.DarkMagenta)
                    }
                }
            };
        }

        public EditResult Execute(string text, int selectionStart, int selectionLength, string parameter = "")
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                parameter = DetectLanguage(text);
            }

            if (!_languageRules.ContainsKey(parameter))
            {
                var availableLanguages = string.Join(", ", _languageRules.Keys);
                return new EditResult(text, selectionStart, selectionLength, false,
                    $"Мову '{parameter}' не підтримується. Доступні: {availableLanguages}");
            }

            return new EditResult(text, selectionStart, selectionLength, true,
                $"Підсвічування синтаксису для мови '{parameter}' застосовано");
        }

        public void ApplyHighlighting(RichTextBox richTextBox, string language = null)
        {
            if (richTextBox == null || string.IsNullOrEmpty(richTextBox.Text))
                return;

            if (string.IsNullOrWhiteSpace(language))
            {
                language = DetectLanguage(richTextBox.Text);
            }

            if (!_languageRules.ContainsKey(language))
                return;

            int originalSelectionStart = richTextBox.SelectionStart;
            int originalSelectionLength = richTextBox.SelectionLength;

            richTextBox.SelectAll();
            richTextBox.SelectionColor = Color.Black;
            richTextBox.SelectionFont = new Font(richTextBox.Font, FontStyle.Regular);

            var rules = _languageRules[language];
            foreach (var rule in rules)
            {
                ApplyRule(richTextBox, rule);
            }

            richTextBox.Select(originalSelectionStart, originalSelectionLength);
        }

        private void ApplyRule(RichTextBox richTextBox, SyntaxRule rule)
        {
            var matches = Regex.Matches(richTextBox.Text, rule.Pattern, rule.Options);

            foreach (Match match in matches)
            {
                richTextBox.Select(match.Index, match.Length);
                richTextBox.SelectionColor = rule.Color;

                if (rule.FontStyle.HasValue)
                {
                    richTextBox.SelectionFont = new Font(richTextBox.Font, rule.FontStyle.Value);
                }
            }
        }

        private string DetectLanguage(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "csharp";

            if (Regex.IsMatch(text, "</?\\w+[^>]*>", RegexOptions.IgnoreCase))
            {
                return "html";
            }

            if (Regex.IsMatch(text, "[.#]?[\\w-]+\\s*\\{[^}]*\\}", RegexOptions.IgnoreCase))
            {
                return "css";
            }

            if (Regex.IsMatch(text, "\\b(function|var|let|const)\\b", RegexOptions.IgnoreCase))
            {
                return "javascript";
            }

            return "csharp";
        }

        public void AddLanguageRule(string language, SyntaxRule[] rules)
        {
            if (!string.IsNullOrWhiteSpace(language) && rules != null)
            {
                _languageRules[language] = rules;
            }
        }

        public IEnumerable<string> GetSupportedLanguages()
        {
            return _languageRules.Keys;
        }
    }

    public class SyntaxRule
    {
        public string Pattern { get; set; }
        public Color Color { get; set; }
        public RegexOptions Options { get; set; }
        public FontStyle? FontStyle { get; set; }

        public SyntaxRule(string pattern, Color color, RegexOptions options = RegexOptions.None, FontStyle? fontStyle = null)
        {
            Pattern = pattern;
            Color = color;
            Options = options;
            FontStyle = fontStyle;
        }
    }
}