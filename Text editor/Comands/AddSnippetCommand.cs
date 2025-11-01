using System;

namespace TextEditorMK.Commands
{
    public class AddSnippetCommand : ICommand
    {
        private readonly object _snippetService;
        private readonly object _snippet;
        private bool _wasExecuted;

        public string Name => "Add Snippet";

        public AddSnippetCommand(object snippetService, object snippet)
        {
            _snippetService = snippetService ?? throw new ArgumentNullException(nameof(snippetService));
            _snippet = snippet ?? throw new ArgumentNullException(nameof(snippet));
        }

        public bool CanExecute()
        {
            return _snippet != null;
        }

        public void Execute()
        {
            if (!CanExecute())
                throw new InvalidOperationException("Cannot execute AddSnippetCommand");

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