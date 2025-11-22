using System.Collections.Generic;
using System.Drawing;

namespace TextEditor.Flyweight
{
    public class TextStyleFactory
    {
        private readonly Dictionary<string, ITextStyle> _styles = new Dictionary<string, ITextStyle>();

        public int TotalObjectsCreated => _styles.Count;

        public ITextStyle GetStyle(string fontFamily, float size, FontStyle style, Color color)
        {
            string key = $"{fontFamily}_{size}_{style}_{color.Name}";

            if (!_styles.ContainsKey(key))
            {
                _styles[key] = new SharedTextStyle(fontFamily, size, style, color);
            }

            return _styles[key];
        }
    }
}