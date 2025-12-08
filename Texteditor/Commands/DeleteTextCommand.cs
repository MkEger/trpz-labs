using System.Windows.Forms;
using Texteditor.Commands;

namespace Texteditor.Commands
{
    /// <summary>
    /// Command for deleting text (Command pattern)
    /// </summary>
    public class DeleteTextCommand : ICommand
    {
        private readonly RichTextBox _textBox;
        private readonly int _start;
        private readonly int _length;
        private string _deletedText;
        private string _previousText;

        public string Description => $"Delete {_length} characters";

        public DeleteTextCommand(RichTextBox textBox, int start, int length)
        {
            _textBox = textBox;
            _start = start;
            _length = length;
        }

        public void Execute()
        {
            _previousText = _textBox.Text;
            if (_start >= 0 && _start + _length <= _textBox.Text.Length)
            {
                _deletedText = _textBox.Text.Substring(_start, _length);
                _textBox.Text = _textBox.Text.Remove(_start, _length);
                _textBox.SelectionStart = _start;
            }
        }

        public void Undo()
        {
            _textBox.Text = _previousText;
            _textBox.SelectionStart = _start;
        }
    }
}