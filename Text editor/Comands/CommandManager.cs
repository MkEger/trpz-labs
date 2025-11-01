using System;
using System.Collections.Generic;
using System.Linq;

namespace TextEditorMK.Commands
{
    public class CommandManager
    {
        private readonly Stack<ICommand> _executedCommands;
        private readonly Stack<ICommand> _undoneCommands;
        private readonly int _maxHistorySize;

        public CommandManager(int maxHistorySize = 100)
        {
            if (maxHistorySize <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxHistorySize));

            _maxHistorySize = maxHistorySize;
            _executedCommands = new Stack<ICommand>();
            _undoneCommands = new Stack<ICommand>();
        }

        public void ExecuteCommand(ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (!command.CanExecute())
                throw new InvalidOperationException($"Command '{command.Name}' cannot be executed");

            try
            {
                command.Execute();
                _executedCommands.Push(command);
                _undoneCommands.Clear();
                TrimHistoryIfNeeded();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CommandManager: Error executing '{command.Name}': {ex.Message}");
                throw;
            }
        }

        private void TrimHistoryIfNeeded()
        {
            if (_executedCommands.Count <= _maxHistorySize)
                return;

            var commands = new ICommand[_maxHistorySize];
            for (int i = 0; i < _maxHistorySize; i++)
            {
                commands[i] = _executedCommands.Pop();
            }

            _executedCommands.Clear();

            for (int i = _maxHistorySize - 1; i >= 0; i--)
            {
                _executedCommands.Push(commands[i]);
            }
        }

        public bool Undo()
        {
            if (!CanUndo())
                return false;

            var command = _executedCommands.Pop();

            try
            {
                command.Undo();
                _undoneCommands.Push(command);
                return true;
            }
            catch (Exception ex)
            {
                _executedCommands.Push(command);
                System.Diagnostics.Debug.WriteLine($"CommandManager: Error undoing '{command.Name}': {ex.Message}");
                throw;
            }
        }

        public bool Redo()
        {
            if (!CanRedo())
                return false;

            var command = _undoneCommands.Pop();

            try
            {
                command.Execute();
                _executedCommands.Push(command);
                return true;
            }
            catch (Exception ex)
            {
                _undoneCommands.Push(command);
                System.Diagnostics.Debug.WriteLine($"CommandManager: Error redoing '{command.Name}': {ex.Message}");
                throw;
            }
        }

        public bool CanUndo() => _executedCommands.Count > 0;

        public bool CanRedo() => _undoneCommands.Count > 0;

        public IEnumerable<string> GetExecutedCommandsHistory()
        {
            return _executedCommands.Select(c => c.Name).ToList().AsReadOnly();
        }

        public void ClearHistory()
        {
            _executedCommands.Clear();
            _undoneCommands.Clear();
        }

        public int ExecutedCommandsCount => _executedCommands.Count;

        public int UndoneCommandsCount => _undoneCommands.Count;
    }
}