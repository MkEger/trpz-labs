using System;
using TextEditor;

namespace TextEditor.Strategies
{
    public interface ITextEditingStrategy
    {
        string Name { get; }
        string Description { get; }
        EditResult Execute(string text, int selectionStart, int selectionLength, string parameter = "");
    }
}
