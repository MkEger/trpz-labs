using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using Texteditor.Client;

namespace Texteditor
{
    /// <summary>
    /// Test form for demonstrating SOA client functionality
    /// </summary>
    public partial class TestClientForm : Form
    {
        private TextProcessingClient _client;
        private Button btnConnect;
        private Button btnTest;
        private Button btnWordCount;
        private Button btnExtractEmail;
        private Button btnGeneratePassword;
        private TextBox txtInput;
        private TextBox txtOutput;
        private Label lblStatus;
        private ComboBox cmbOperations;

        public TestClientForm()
        {
            InitializeComponent();
            _client = new TextProcessingClient();
        }

        private void InitializeComponent()
        {
            this.btnConnect = new Button();
            this.btnTest = new Button();
            this.btnWordCount = new Button();
            this.btnExtractEmail = new Button();
            this.btnGeneratePassword = new Button();
            this.txtInput = new TextBox();
            this.txtOutput = new TextBox();
            this.lblStatus = new Label();
            this.cmbOperations = new ComboBox();

            // Form
            this.Text = "SOA Test Client - Розширений функціонал";
            this.Size = new System.Drawing.Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            // Connect button
            this.btnConnect.Text = "Підключитися";
            this.btnConnect.Location = new System.Drawing.Point(20, 20);
            this.btnConnect.Size = new System.Drawing.Size(120, 30);
            this.btnConnect.Click += BtnConnect_Click;

            // Operations ComboBox
            this.cmbOperations.Items.AddRange(new string[] {
                "uppercase - ВЕЛИКІ ЛІТЕРИ",
                "wordcount - Підрахунок слів",
                "extractemail - Витягти Email",
                "removeduplicates - Видалити дублікати",
                "sortlines - Сортувати рядки",
                "base64encode - Base64 кодування",
                "transliterate - Транслітерація"
            });
            this.cmbOperations.SelectedIndex = 0;
            this.cmbOperations.Location = new System.Drawing.Point(160, 20);
            this.cmbOperations.Size = new System.Drawing.Size(200, 30);

            // Test button
            this.btnTest.Text = "Тестувати";
            this.btnTest.Location = new System.Drawing.Point(380, 20);
            this.btnTest.Size = new System.Drawing.Size(100, 30);
            this.btnTest.Click += BtnTest_Click;
            this.btnTest.Enabled = false;

            // Quick action buttons
            this.btnWordCount.Text = "Підрахунок";
            this.btnWordCount.Location = new System.Drawing.Point(20, 70);
            this.btnWordCount.Size = new System.Drawing.Size(100, 30);
            this.btnWordCount.Click += BtnWordCount_Click;
            this.btnWordCount.Enabled = false;

            this.btnExtractEmail.Text = "Email";
            this.btnExtractEmail.Location = new System.Drawing.Point(140, 70);
            this.btnExtractEmail.Size = new System.Drawing.Size(100, 30);
            this.btnExtractEmail.Click += BtnExtractEmail_Click;
            this.btnExtractEmail.Enabled = false;

            this.btnGeneratePassword.Text = "Пароль";
            this.btnGeneratePassword.Location = new System.Drawing.Point(260, 70);
            this.btnGeneratePassword.Size = new System.Drawing.Size(100, 30);
            this.btnGeneratePassword.Click += BtnGeneratePassword_Click;
            this.btnGeneratePassword.Enabled = false;

            // Input textbox
            this.txtInput.Text = @"Привіт! Мене звати Іван Петренко.
Мій email: ivan@example.com
Другий email: petro@test.ua
Мене звати Іван Петренко.
Цей рядок повторюється.
Цей рядок повторюється.
Контакт: support@company.org";
            this.txtInput.Location = new System.Drawing.Point(20, 120);
            this.txtInput.Size = new System.Drawing.Size(540, 150);
            this.txtInput.Multiline = true;
            this.txtInput.ScrollBars = ScrollBars.Both;

            // Output textbox
            this.txtOutput.Location = new System.Drawing.Point(20, 290);
            this.txtOutput.Size = new System.Drawing.Size(540, 120);
            this.txtOutput.Multiline = true;
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = ScrollBars.Both;

            // Status label
            this.lblStatus.Text = "Не підключено до SOA сервера";
            this.lblStatus.Location = new System.Drawing.Point(20, 430);
            this.lblStatus.Size = new System.Drawing.Size(540, 30);

            // Add controls
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.cmbOperations);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnWordCount);
            this.Controls.Add(this.btnExtractEmail);
            this.Controls.Add(this.btnGeneratePassword);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.lblStatus);

            // Labels
            var lblInput = new Label { Text = "Вхідний текст:", Location = new System.Drawing.Point(20, 105), Size = new System.Drawing.Size(100, 15) };
            var lblOutput = new Label { Text = "Результат обробки:", Location = new System.Drawing.Point(20, 275), Size = new System.Drawing.Size(150, 15) };
            this.Controls.Add(lblInput);
            this.Controls.Add(lblOutput);
        }

        private async void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Підключення до SOA сервера...";
                var connected = await _client.ConnectAsync();
                
                if (connected)
                {
                    lblStatus.Text = "Підключено до SOA сервера! Готовий до роботи.";
                    btnTest.Enabled = true;
                    btnWordCount.Enabled = true;
                    btnExtractEmail.Enabled = true;
                    btnGeneratePassword.Enabled = true;
                    btnConnect.Enabled = false;
                }
                else
                {
                    lblStatus.Text = "Помилка підключення. Переконайтесь, що сервер запущено.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"? Помилка: {ex.Message}";
            }
        }

        private async void BtnTest_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedOp = cmbOperations.SelectedItem.ToString();
                var operation = selectedOp.Split(' ')[0]; // Витягуємо операцію до пробілу
                
                lblStatus.Text = $"Обробка через SOA сервіс: {operation}...";
                
                var response = await _client.FormatTextAsync(txtInput.Text, operation);
                
                if (response.Success)
                {
                    txtOutput.Text = response.ProcessedText;
                    lblStatus.Text = $"Успішно! Операція: {operation}, Час: {response.ProcessingTime}";
                }
                else
                {
                    txtOutput.Text = response.Message;
                    lblStatus.Text = "Помилка обробки на сервері";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Помилка: {ex.Message}";
            }
        }

        private async void BtnWordCount_Click(object sender, EventArgs e)
        {
            await PerformOperation("wordcount", "Підрахунок слів та символів");
        }

        private async void BtnExtractEmail_Click(object sender, EventArgs e)
        {
            await PerformOperation("extractemail", "Витягування email адрес");
        }

        private async void BtnGeneratePassword_Click(object sender, EventArgs e)
        {
            await PerformOperation("generatepassword", "Генерація безпечного пароля");
        }

        private async Task PerformOperation(string operation, string description)
        {
            try
            {
                lblStatus.Text = $"SOA сервіс: {description}...";
                
                var response = await _client.FormatTextAsync(txtInput.Text, operation);
                
                if (response.Success)
                {
                    txtOutput.Text = response.ProcessedText;
                    lblStatus.Text = $"{description} завершено за {response.ProcessingTime}";
                }
                else
                {
                    txtOutput.Text = response.Message;
                    lblStatus.Text = $"Помилка при {description.ToLower()}";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Помилка: {ex.Message}";
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _client?.Dispose();
            base.OnFormClosed(e);
        }
    }
}