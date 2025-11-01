using System;

namespace TextEditorMK.Commands
{
    public class DeleteSnippetCommand : ICommand
    {
        private readonly object _snippetService;
        private readonly string _snippetName;
        private bool _wasExecuted;

        public string Name => $"Delete Snippet: {_snippetName}";

        public DeleteSnippetCommand(object snippetService, string snippetName)
        {
            _snippetService = snippetService ?? throw new ArgumentNullException(nameof(snippetService));
            _snippetName = !string.IsNullOrWhiteSpace(snippetName) ? snippetName :
                throw new ArgumentException("Snippet name cannot be null or empty", nameof(snippetName));
        }

        public bool CanExecute()
        {
            return !string.IsNullOrWhiteSpace(_snippetName);
        }

        public void Execute()
        {
            if (!CanExecute())
                throw new InvalidOperationException($"Cannot execute DeleteSnippetCommand - snippet '{_snippetName}' not found");

            _wasExecuted = true;
        }

        public void Undo()
        {
            if (!_wasExecuted)
                throw new InvalidOperationException("Cannot undo - command was not executed");

            _wasExecuted = false;
        }
    }
}