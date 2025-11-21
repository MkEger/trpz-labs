using System;
using System.Windows.Forms;

namespace TextEditor.Observers
{
    public class StatusBarObserver : IObserver
    {
        private readonly ToolStripStatusLabel _statusLabel;

        public StatusBarObserver(ToolStripStatusLabel statusLabel)
        {
            _statusLabel = statusLabel;
        }

        public void Update(string textContent)
        {
            if (textContent == null) return;

            int charCount = textContent.Length;

            int wordCount = string.IsNullOrWhiteSpace(textContent)
                ? 0
                : textContent.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;

            _statusLabel.Text = $"Chars: {charCount} | Words: {wordCount} | Updated: {DateTime.Now:HH:mm:ss}";
        }
    }
}
