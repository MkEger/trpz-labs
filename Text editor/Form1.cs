using System;
using System.Drawing;
using System.Windows.Forms;
using TextEditor.Exporters;
using TextEditor.Flyweight;

namespace TextEditor
{
    public partial class Form1 : Form
    {
        private TextStyleFactory _styleFactory;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML Report|*.xml|Log File|*.log";
            saveFileDialog.Title = "Export Document Report";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                string extension = System.IO.Path.GetExtension(filePath).ToLower();

                ReportGenerator generator = null;

                if (extension == ".xml")
                {
                    generator = new XmlReportGenerator();
                }
                else
                {
                    generator = new LogReportGenerator();
                }

                try
                {
                    generator.ExportDocument(filePath, richTextBox1.Text);
                    MessageBox.Show("Звіт успішно збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnHighlight_Click(object sender, EventArgs e)
        {
            if (_styleFactory == null) _styleFactory = new TextStyleFactory();

            string text = richTextBox1.Text;
            int originalIndex = richTextBox1.SelectionStart;
            int originalLength = richTextBox1.SelectionLength;

            richTextBox1.SuspendLayout();

            var keywordStyle = _styleFactory.GetStyle("Consolas", 10, FontStyle.Bold, Color.Blue);
            var typeStyle = _styleFactory.GetStyle("Consolas", 10, FontStyle.Regular, Color.Teal);
            var stringStyle = _styleFactory.GetStyle("Consolas", 10, FontStyle.Italic, Color.Brown);
            var defaultStyle = _styleFactory.GetStyle("Consolas", 10, FontStyle.Regular, Color.Black);

            defaultStyle.Apply(richTextBox1, 0, text.Length);

            HighlightWords(text, new[] { "public", "private", "class", "void", "return", "new" }, keywordStyle);
            HighlightWords(text, new[] { "string", "int", "bool", "var", "double" }, typeStyle);
            HighlightStringLiterals(text, stringStyle);

            richTextBox1.SelectionStart = originalIndex;
            richTextBox1.SelectionLength = originalLength;
            richTextBox1.SelectionColor = Color.Black;
            richTextBox1.ResumeLayout();

            MessageBox.Show($"Форматування завершено! Створено унікальних об'єктів стилів: {_styleFactory.TotalObjectsCreated}", "Flyweight Stats");
        }

        private void HighlightWords(string text, string[] words, ITextStyle style)
        {
            foreach (var word in words)
            {
                int index = -1;
                while ((index = text.IndexOf(word, index + 1)) != -1)
                {
                    style.Apply(richTextBox1, index, word.Length);
                }
            }
        }

        private void HighlightStringLiterals(string text, ITextStyle style)
        {
            int start = -1;
            bool inString = false;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '"')
                {
                    if (!inString)
                    {
                        start = i;
                        inString = true;
                    }
                    else
                    {
                        style.Apply(richTextBox1, start, i - start + 1);
                        inString = false;
                    }
                }
            }
        }
    }
}