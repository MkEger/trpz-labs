using System.Windows.Forms;
using Texteditor.Commands;

namespace Texteditor.Commands
{
    /// <summary>
    /// Command for inserting text (Command pattern)
    /// </summary>
    public class InsertTextCommand : ICommand
    {
        private readonly RichTextBox _textBox;
        private readonly string _text;
        private readonly int _position;
        private string _previousText;

        public string Description => $"Insert '{_text}'";

        public InsertTextCommand(RichTextBox textBox, string text, int position)
        {
            _textBox = textBox;
            _text = text;
            _position = position;
        }

        public void Execute()
        {
            _previousText = _textBox.Text;
            _textBox.Text = _textBox.Text.Insert(_position, _text);
            _textBox.SelectionStart = _position + _text.Length;
        }

        public void Undo()
        {
            _textBox.Text = _previousText;
            _textBox.SelectionStart = _position;
        }
    }
}