using System;
using System.Collections.Generic;
using System.Linq;
using TextEditor;
using TextEditor.Strategies;

namespace TextEditor
{
    public class TextEditorContext
    {
        private readonly Dictionary<string, ITextEditingStrategy> _strategies;
        private ITextEditingStrategy _currentStrategy;

        public TextEditorContext()
        {
            _strategies = new Dictionary<string, ITextEditingStrategy>(StringComparer.OrdinalIgnoreCase)
            {
                { "macro", new MacroStrategy() },
                { "snippet", new SnippetStrategy() }
            };

            _currentStrategy = _strategies["macro"];
        }

        public bool SetStrategy(string strategyName)
        {
            if (_strategies.ContainsKey(strategyName))
            {
                _currentStrategy = _strategies[strategyName];
                return true;
            }
            return false;
        }

        public EditResult ExecuteOperation(string text, int selectionStart, int selectionLength, string parameter = "")
        {
            if (_currentStrategy == null)
            {
                return new EditResult(text, selectionStart, selectionLength, false, "Стратегію не вибрано");
            }

            try
            {
                return _currentStrategy.Execute(text, selectionStart, selectionLength, parameter);
            }
            catch (Exception ex)
            {
                return new EditResult(text, selectionStart, selectionLength, false,
                    $"Помилка виконання операції: {ex.Message}");
            }
        }

        public EditResult ExecuteWithStrategy(string strategyName, string text, int selectionStart, int selectionLength, string parameter = "")
        {
            if (!_strategies.ContainsKey(strategyName))
            {
                var availableStrategies = string.Join(", ", _strategies.Keys);
                return new EditResult(text, selectionStart, selectionLength, false,
                    $"Стратегію '{strategyName}' не знайдено. Доступні: {availableStrategies}");
            }

            try
            {
                return _strategies[strategyName].Execute(text, selectionStart, selectionLength, parameter);
            }
            catch (Exception ex)
            {
                return new EditResult(text, selectionStart, selectionLength, false,
                    $"Помилка виконання операції: {ex.Message}");
            }
        }

        public ITextEditingStrategy GetCurrentStrategy()
        {
            return _currentStrategy;
        }

        public IEnumerable<string> GetStrategyNames()
        {
            return _strategies.Keys;
        }

        public void AddStrategy(string name, ITextEditingStrategy strategy)
        {
            if (!string.IsNullOrWhiteSpace(name) && strategy != null)
            {
                _strategies[name] = strategy;
            }
        }
    }
}
