using System;
using System.Windows.Forms;

namespace TextEditorMK.Commands
{
    public class InsertSnippetCommand : ICommand
    {
        private readonly object _snippetService;
        private readonly object _snippet;
        private readonly int _insertPosition;
        private readonly RichTextBox _textBox;
        private string? _previousText;
        private int _previousSelectionStart;
        private int _previousSelectionLength;

        public string Name => $"Insert Snippet";

        public InsertSnippetCommand(object snippetService, object snippet,
            int insertPosition, RichTextBox textBox)
        {
            _snippetService = snippetService ?? throw new ArgumentNullException(nameof(snippetService));
            _snippet = snippet ?? throw new ArgumentNullException(nameof(snippet));
            _insertPosition = insertPosition;
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
        }

        public bool CanExecute()
        {
            return _snippet != null && _textBox != null && _insertPosition >= 0 &&
                   _insertPosition <= _textBox.Text.Length;
        }

        public void Execute()
        {
            if (!CanExecute())
                throw new InvalidOperationException("Cannot execute InsertSnippetCommand");

            _previousText = _textBox.Text;
            _previousSelectionStart = _textBox.SelectionStart;
            _previousSelectionLength = _textBox.SelectionLength;
        }

        public void Undo()
        {
            if (string.IsNullOrEmpty(_previousText))
                throw new InvalidOperationException("Cannot undo - no previous state saved");

            _textBox.Text = _previousText;
            _textBox.SelectionStart = _previousSelectionStart;
            _textBox.SelectionLength = _previousSelectionLength;
        }
    }
}