using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Texteditor.Contracts;
using Texteditor.Strategies;

namespace Texteditor.Services
{
    /// <summary>
    /// SOA Service implementation for text processing operations
    /// </summary>
    public class TextProcessingService : ITextProcessingService
    {
        private readonly Dictionary<string, ITextFormattingStrategy> _strategies;

        public TextProcessingService()
        {
            _strategies = new Dictionary<string, ITextFormattingStrategy>
            {
                {"uppercase", new UpperCaseStrategy()},
                {"lowercase", new LowerCaseStrategy()},
                {"titlecase", new TitleCaseStrategy()}
            };
        }

        public TextProcessingResponse ProcessText(TextProcessingRequest request)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                string result = request.Text;
                
                switch (request.Operation.ToLower())
                {
                    case "uppercase":
                    case "lowercase":
                    case "titlecase":
                        if (_strategies.ContainsKey(request.Operation.ToLower()))
                        {
                            result = _strategies[request.Operation.ToLower()].FormatText(request.Text);
                        }
                        break;
                    case "reverse":
                        result = new string(request.Text.Reverse().ToArray());
                        break;
                    case "removewhitespace":
                        result = request.Text.Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "");
                        break;
                    case "addtimestamp":
                        result = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {request.Text}";
                        break;
                    case "wordcount":
                        var words = request.Text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                        result = $"Слів: {words.Length}, Символів: {request.Text.Length}, Рядків: {request.Text.Split('\n').Length}";
                        break;
                    case "extractemail":
                        var emails = System.Text.RegularExpressions.Regex.Matches(request.Text, @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b");
                        result = emails.Count > 0 ? string.Join(", ", emails.Cast<System.Text.RegularExpressions.Match>().Select(m => m.Value)) : "Email адреси не знайдено";
                        break;
                    case "removeduplicates":
                        var lines = request.Text.Split('\n').Select(l => l.Trim()).Distinct().Where(l => !string.IsNullOrEmpty(l));
                        result = string.Join("\n", lines);
                        break;
                    case "sortlines":
                        var sortedLines = request.Text.Split('\n').Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)).OrderBy(l => l);
                        result = string.Join("\n", sortedLines);
                        break;
                    case "base64encode":
                        var bytes = Encoding.UTF8.GetBytes(request.Text);
                        result = Convert.ToBase64String(bytes);
                        break;
                    case "base64decode":
                        try
                        {
                            var decodedBytes = Convert.FromBase64String(request.Text);
                            result = Encoding.UTF8.GetString(decodedBytes);
                        }
                        catch
                        {
                            result = "Помилка: неправильний Base64 формат";
                        }
                        break;
                    case "urlify":
                        result = request.Text.Replace(" ", "%20").Replace("&", "%26").Replace("=", "%3D").Replace("?", "%3F");
                        break;
                    case "generatepassword":
                        var length = 12;
                        if (int.TryParse(request.Text, out int customLength) && customLength > 0 && customLength <= 100)
                        {
                            length = customLength;
                        }
                        result = GeneratePassword(length);
                        break;
                    case "transliterate":
                        result = TransliterateUkrainianToLatin(request.Text);
                        break;
                    case "jsonformat":
                        try
                        {
                            // Простий JSON formatter
                            result = FormatJson(request.Text);
                        }
                        catch
                        {
                            result = "Помилка: неправильний JSON формат";
                        }
                        break;
                    default:
                        return new TextProcessingResponse
                        {
                            Success = false,
                            Message = $"Невідома операція: {request.Operation}",
                            ProcessingTime = stopwatch.ElapsedMilliseconds.ToString()
                        };
                }

                stopwatch.Stop();
                
                return new TextProcessingResponse
                {
                    ProcessedText = result,
                    Success = true,
                    Message = "Текст успішно оброблено",
                    ProcessingTime = $"{stopwatch.ElapsedMilliseconds} мс"
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return new TextProcessingResponse
                {
                    Success = false,
                    Message = $"Помилка при обробці тексту: {ex.Message}",
                    ProcessingTime = stopwatch.ElapsedMilliseconds.ToString()
                };
            }
        }

        private string GeneratePassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string TransliterateUkrainianToLatin(string input)
        {
            var translitMap = new Dictionary<char, string>
            {
                {'а', "a"}, {'б', "b"}, {'в', "v"}, {'г', "h"}, {'ґ', "g"}, {'д', "d"},
                {'е', "e"}, {'є', "ie"}, {'ж', "zh"}, {'з', "z"}, {'и', "y"}, {'і', "i"},
                {'ї', "i"}, {'й', "i"}, {'к', "k"}, {'л', "l"}, {'м', "m"}, {'н', "n"},
                {'о', "o"}, {'п', "p"}, {'р', "r"}, {'с', "s"}, {'т', "t"}, {'у', "u"},
                {'ф', "f"}, {'х', "kh"}, {'ц', "ts"}, {'ч', "ch"}, {'ш', "sh"}, {'щ', "shch"},
                {'ь', ""}, {'ю', "iu"}, {'я', "ia"},
                {'А', "A"}, {'Б', "B"}, {'В', "V"}, {'Г', "H"}, {'Ґ', "G"}, {'Д', "D"},
                {'Е', "E"}, {'Є', "Ie"}, {'Ж', "Zh"}, {'З', "Z"}, {'И', "Y"}, {'І', "I"},
                {'Ї', "I"}, {'Й', "I"}, {'К', "K"}, {'Л', "L"}, {'М', "M"}, {'Н', "N"},
                {'О', "O"}, {'П', "P"}, {'Р', "R"}, {'С', "S"}, {'Т', "T"}, {'У', "U"},
                {'Ф', "F"}, {'Х', "Kh"}, {'Ц', "Ts"}, {'Ч', "Ch"}, {'Ш', "Sh"}, {'Щ', "Shch"},
                {'Ь', ""}, {'Ю', "Iu"}, {'Я', "Ia"}
            };

            var result = new StringBuilder();
            foreach (char c in input)
            {
                if (translitMap.ContainsKey(c))
                    result.Append(translitMap[c]);
                else
                    result.Append(c);
            }
            return result.ToString();
        }

        private string FormatJson(string input)
        {
            // Простий JSON formatter без зовнішніх бібліотек
            var formatted = new StringBuilder();
            int indent = 0;
            bool inString = false;
            bool escapeNext = false;

            foreach (char c in input)
            {
                if (escapeNext)
                {
                    formatted.Append(c);
                    escapeNext = false;
                    continue;
                }

                if (c == '\\')
                {
                    formatted.Append(c);
                    escapeNext = true;
                    continue;
                }

                if (c == '"' && !escapeNext)
                {
                    inString = !inString;
                    formatted.Append(c);
                    continue;
                }

                if (inString)
                {
                    formatted.Append(c);
                    continue;
                }

                switch (c)
                {
                    case '{':
                    case '[':
                        formatted.Append(c);
                        formatted.AppendLine();
                        indent++;
                        formatted.Append(new string(' ', indent * 2));
                        break;
                    case '}':
                    case ']':
                        formatted.AppendLine();
                        indent--;
                        formatted.Append(new string(' ', indent * 2));
                        formatted.Append(c);
                        break;
                    case ',':
                        formatted.Append(c);
                        formatted.AppendLine();
                        formatted.Append(new string(' ', indent * 2));
                        break;
                    case ':':
                        formatted.Append(c);
                        formatted.Append(' ');
                        break;
                    default:
                        if (!char.IsWhiteSpace(c))
                            formatted.Append(c);
                        break;
                }
            }

            return formatted.ToString();
        }

        public TextProcessingResponse FormatText(string text, string formatType)
        {
            var request = new TextProcessingRequest
            {
                Text = text,
                Operation = formatType,
                UserId = Environment.UserName
            };
            
            return ProcessText(request);
        }

        public TextProcessingResponse CheckSpelling(string text)
        {
            // Простий spell checker для української мови - можна розширити
            var commonWords = new[] { 
                "the", "and", "or", "but", "hello", "world", "text", "editor",
                "привіт", "світ", "текст", "редактор", "сервіс", "обробка", "програма"
            };
            var words = text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var misspelled = new List<string>();
            
            foreach (var word in words)
            {
                var cleanWord = word.Trim('.', ',', '!', '?', ';', ':').ToLower();
                if (!string.IsNullOrWhiteSpace(cleanWord) && 
                    !commonWords.Contains(cleanWord))
                {
                    misspelled.Add(word);
                }
            }
            
            return new TextProcessingResponse
            {
                Success = true,
                ProcessedText = misspelled.Count > 0 ? 
                    $"Потенційно неправильні слова: {string.Join(", ", misspelled)}" : 
                    "Помилок не знайдено",
                Message = $"Перевірено {words.Length} слів"
            };
        }

        public string[] GetAvailableFormats()
        {
            return new string[] { 
                "uppercase", "lowercase", "titlecase", "reverse", "removewhitespace", "addtimestamp",
                "wordcount", "extractemail", "removeduplicates", "sortlines", "base64encode", 
                "base64decode", "urlify", "generatepassword", "transliterate", "jsonformat"
            };
        }
    }
}