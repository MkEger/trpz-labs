using System;
using System.Windows.Forms;
using TextEditor.Exporters;
using TextEditor.Observers;

namespace TextEditor
{
    public partial class Form1 : Form
    {
        private TextEventManager _textEventManager;

        public Form1()
        {
            InitializeComponent();
            InitializeObservers();
        }

        private void InitializeObservers()
        {
            _textEventManager = new TextEventManager();

            if (toolStripStatusLabel1 != null)
            {
                var statusObserver = new StatusBarObserver(toolStripStatusLabel1);
                _textEventManager.Attach(statusObserver);
            }

            var autoSaveObserver = new AutoSaveObserver();
            _textEventManager.Attach(autoSaveObserver);

            richTextBox1.TextChanged += (sender, e) =>
            {
                _textEventManager.TextContent = richTextBox1.Text;
            };
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
    }
}
