using System.Drawing;
using System.Windows.Forms;

namespace TextEditor.Flyweight
{
    public class SharedTextStyle : ITextStyle
    {
        private readonly Font _font;
        private readonly Color _color;

        public SharedTextStyle(string fontFamily, float size, FontStyle style, Color color)
        {
            _font = new Font(fontFamily, size, style);
            _color = color;
        }

        public void Apply(RichTextBox control, int index, int length)
        {
            control.Select(index, length);
            control.SelectionFont = _font;
            control.SelectionColor = _color;
        }
    }
}