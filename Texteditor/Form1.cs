using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Texteditor.Commands;
using Texteditor.Strategies;
using Texteditor.Services;
using Texteditor.Contracts;
using Texteditor.Server;
using Texteditor.Client;

namespace Texteditor
{
    public partial class Form1 : Form
    {
        private CommandManager _commandManager;
        private TextProcessingServer _server;
        private TextProcessingClient _client;
        private ITextFormattingStrategy _currentStrategy;
        private ITextProcessingService _textService;

        public Form1()
        {
            InitializeComponent();
            InitializeEditor();
        }

        private void InitializeEditor()
        {
            _commandManager = new CommandManager();
            _currentStrategy = new UpperCaseStrategy();
            
            // Initialize local service instance
            _textService = new TextProcessingService();
            
            // Initialize TCP server
            _server = new TextProcessingServer(8080);
            _server.LogMessage += OnServerLogMessage;
            
            UpdateMenuState();
        }

        private void OnServerLogMessage(string message)
        {
            // Використовуємо Invoke для безпечного оновлення UI з іншого потоку
            if (InvokeRequired)
            {
                Invoke(new Action<string>(OnServerLogMessage), message);
                return;
            }
            
            statusLabel.Text = message;
        }

        private void UpdateMenuState()
        {
            undoToolStripMenuItem.Enabled = _commandManager.CanUndo;
            redoToolStripMenuItem.Enabled = _commandManager.CanRedo;
        }

        private async void ApplyFormatting(string formatType)
        {
            if (string.IsNullOrEmpty(textEditor.SelectedText) && string.IsNullOrEmpty(textEditor.Text))
                return;

            string textToFormat = !string.IsNullOrEmpty(textEditor.SelectedText) 
                ? textEditor.SelectedText 
                : textEditor.Text;

            try
            {
                statusLabel.Text = "Обробка тексту через SOA сервіс...";
                
                // Використовуємо локальний сервіс або віддалений через TCP
                TextProcessingResponse response;
                
                if (_server != null && _server.IsRunning)
                {
                    // Спробуємо використати віддалений сервіс через TCP
                    if (_client == null)
                    {
                        _client = new TextProcessingClient();
                    }
                    
                    response = await _client.FormatTextAsync(textToFormat, formatType);
                    
                    // Якщо віддалений сервіс не працює, використовуємо локальний
                    if (!response.Success)
                    {
                        response = await Task.Run(() => _textService.FormatText(textToFormat, formatType));
                    }
                }
                else
                {
                    // Використовуємо локальний сервіс
                    response = await Task.Run(() => _textService.FormatText(textToFormat, formatType));
                }
                
                if (response.Success)
                {
                    if (!string.IsNullOrEmpty(textEditor.SelectedText))
                    {
                        var command = new InsertTextCommand(textEditor, response.ProcessedText, textEditor.SelectionStart);
                        textEditor.SelectedText = response.ProcessedText;
                    }
                    else
                    {
                        textEditor.Text = response.ProcessedText;
                    }
                    statusLabel.Text = $"Готово. Час обробки: {response.ProcessingTime}";
                }
                else
                {
                    MessageBox.Show($"Помилка: {response.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    statusLabel.Text = "Помилка обробки";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при обробці тексту: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Помилка";
            }
        }

        #region Menu Event Handlers

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textEditor.Text))
            {
                var result = MessageBox.Show("Зберегти зміни?", "Новий файл", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }
            textEditor.Clear();
            statusLabel.Text = "Новий документ";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        textEditor.Text = File.ReadAllText(openFileDialog.FileName, Encoding.UTF8);
                        statusLabel.Text = $"Відкрито: {Path.GetFileName(openFileDialog.FileName)}";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка відкриття файлу: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllText(saveFileDialog.FileName, textEditor.Text, Encoding.UTF8);
                        statusLabel.Text = $"Збережено: {Path.GetFileName(saveFileDialog.FileName)}";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка збереження файлу: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _commandManager.Undo();
            UpdateMenuState();
            statusLabel.Text = "Скасовано";
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _commandManager.Redo();
            UpdateMenuState();
            statusLabel.Text = "Повторено";
        }

        private void upperCaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFormatting("uppercase");
        }

        private void lowerCaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFormatting("lowercase");
        }

        private void titleCaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFormatting("titlecase");
        }

        private void wordCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFormatting("wordcount");
        }

        private void extractEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFormatting("extractemail");
        }

        private void removeDuplicatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFormatting("removeduplicates");
        }

        private void sortLinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFormatting("sortlines");
        }

        private void base64EncodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFormatting("base64encode");
        }

        private void transliterateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFormatting("transliterate");
        }

        private async void startServiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                await _server.StartAsync();
                
                startServiceToolStripMenuItem.Enabled = false;
                stopServiceToolStripMenuItem.Enabled = true;
                
                MessageBox.Show("TCP SOA сервіс успішно запущено!\nПорт: 8080\nАдреса: 127.0.0.1:8080", 
                    "Сервіс запущено", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка запуску сервісу: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Помилка запуску сервісу";
            }
        }

        private void stopServiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                _server?.Stop();
                
                startServiceToolStripMenuItem.Enabled = true;
                stopServiceToolStripMenuItem.Enabled = false;
                
                MessageBox.Show("TCP SOA сервіс зупинено", "Сервіс зупинено", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка зупинки сервісу: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void testClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var testForm = new TestClientForm();
            testForm.ShowDialog(this);
        }

        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                _server?.Stop();
                _client?.Dispose();
            }
            catch { }
            
            base.OnFormClosing(e);
        }
    }
}
