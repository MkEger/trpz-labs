using System;

namespace Texteditor.Commands
{
    /// <summary>
    /// Command pattern interface for text operations
    /// </summary>
    public interface ICommand
    {
        void Execute();
        void Undo();
        string Description { get; }
    }
}