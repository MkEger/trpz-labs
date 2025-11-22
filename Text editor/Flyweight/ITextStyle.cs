using System.Windows.Forms;

namespace TextEditor.Flyweight
{
    public interface ITextStyle
    {
        void Apply(RichTextBox control, int index, int length);
    }
}