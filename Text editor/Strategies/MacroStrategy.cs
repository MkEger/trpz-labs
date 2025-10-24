using System;
using System.Collections.Generic;
using TextEditor;

namespace TextEditor.Strategies
{
    public class MacroStrategy : ITextEditingStrategy
    {
        public string Name => "Макроси";
        public string Description => "Вставка попередньо записаних фрагментів коду";

        private readonly Dictionary<string, string> _macros;

        public MacroStrategy()
        {
            _macros = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "class", "public class ClassName\n{\n    public ClassName()\n    {\n        \n    }\n}" },
                { "method", "public void MethodName()\n{\n    \n}" },
                { "property", "public string PropertyName { get; set; }" },
                { "for", "for (int i = 0; i < length; i++)\n{\n    \n}" },
                { "if", "if (condition)\n{\n    \n}" },
                { "try", "try\n{\n    \n}\ncatch (Exception ex)\n{\n    \n}" }
            };
        }

        public EditResult Execute(string text, int selectionStart, int selectionLength, string parameter = "")
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                var availableMacros = string.Join(", ", _macros.Keys);
                return new EditResult(text, selectionStart, selectionLength, false,
                    $"Вкажіть ім'я макроса. Доступні: {availableMacros}");
            }

            if (!_macros.ContainsKey(parameter))
            {
                var availableMacros = string.Join(", ", _macros.Keys);
                return new EditResult(text, selectionStart, selectionLength, false,
                    $"Макрос '{parameter}' не знайдено. Доступні: {availableMacros}");
            }

            string macroContent = _macros[parameter];
            string beforeSelection = text.Substring(0, selectionStart);
            string afterSelection = text.Substring(selectionStart + selectionLength);
            string newText = beforeSelection + macroContent + afterSelection;

            int newSelectionStart = selectionStart + macroContent.Length;

            return new EditResult(newText, newSelectionStart, 0, true,
                $"Макрос '{parameter}' вставлено успішно");
        }

        public void AddMacro(string name, string content)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _macros[name] = content ?? string.Empty;
            }
        }

        public IEnumerable<string> GetAvailableMacros()
        {
            return _macros.Keys;
        }
    }
}
