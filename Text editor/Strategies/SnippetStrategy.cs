using System;
using System.Collections.Generic;
using TextEditor;

namespace TextEditor.Strategies
{
    public class SnippetStrategy : ITextEditingStrategy
    {
        public string Name => "Сніппети";
        public string Description => "Інтерактивні шаблони з параметрами";

        private readonly Dictionary<string, SnippetTemplate> _snippets;

        public SnippetStrategy()
        {
            _snippets = new Dictionary<string, SnippetTemplate>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "prop",
                    new SnippetTemplate(
                        "public ${type} ${name} { get; set; }",
                        new Dictionary<string, string> { {"type", "string"}, {"name", "Property"} }
                    )
                },
                {
                    "method",
                    new SnippetTemplate(
                        "public ${returnType} ${name}(${parameters})\n{\n    ${body}\n}",
                        new Dictionary<string, string> {
                            {"returnType", "void"}, {"name", "MethodName"},
                            {"parameters", ""}, {"body", "throw new NotImplementedException();"}
                        }
                    )
                }
            };
        }

        public EditResult Execute(string text, int selectionStart, int selectionLength, string parameter = "")
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                var availableSnippets = string.Join(", ", _snippets.Keys);
                return new EditResult(text, selectionStart, selectionLength, false,
                    $"Вкажіть ім'я сніппету. Доступні: {availableSnippets}");
            }

            var parts = parameter.Split('|');
            var snippetName = parts[0].Trim();

            if (!_snippets.ContainsKey(snippetName))
            {
                var availableSnippets = string.Join(", ", _snippets.Keys);
                return new EditResult(text, selectionStart, selectionLength, false,
                    $"Сніппет '{snippetName}' не знайдено. Доступні: {availableSnippets}");
            }

            var snippet = _snippets[snippetName];
            var customValues = new Dictionary<string, string>();

            for (int i = 1; i < parts.Length; i++)
            {
                var keyValue = parts[i].Split('=');
                if (keyValue.Length == 2)
                {
                    customValues[keyValue[0].Trim()] = keyValue[1].Trim();
                }
            }

            string expandedContent = snippet.Expand(customValues);
            string beforeSelection = text.Substring(0, selectionStart);
            string afterSelection = text.Substring(selectionStart + selectionLength);
            string newText = beforeSelection + expandedContent + afterSelection;

            int newSelectionStart = selectionStart + expandedContent.Length;

            return new EditResult(newText, newSelectionStart, 0, true,
                $"Сніппет '{snippetName}' розгорнуто успішно");
        }

        public IEnumerable<string> GetAvailableSnippets()
        {
            return _snippets.Keys;
        }
    }
}