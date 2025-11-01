using System;

namespace TextEditorMK.Commands
{
    public interface ICommand
    {
        string Name { get; }
        bool CanExecute();
        void Execute();
        void Undo();
    }
}
