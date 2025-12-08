using System;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text;
using System.IO;
using Texteditor.Contracts;
using System.Runtime.Serialization.Formatters.Binary;

namespace Texteditor.Client
{
    /// <summary>
    /// TCP Client for connecting to the TextProcessingService (SOA architecture)
    /// </summary>
    public class TextProcessingClient : IDisposable
    {
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private bool _disposed = false;
        private readonly string _serverAddress;
        private readonly int _serverPort;

        public bool IsConnected => _tcpClient?.Connected == true;

        public TextProcessingClient(string serverAddress = "127.0.0.1", int serverPort = 8080)
        {
            _serverAddress = serverAddress;
            _serverPort = serverPort;
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(_serverAddress, _serverPort);
                _networkStream = _tcpClient.GetStream();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<TextProcessingResponse> ProcessTextAsync(string text, string operation)
        {
            if (!IsConnected)
            {
                var connected = await ConnectAsync();
                if (!connected)
                {
                    return new TextProcessingResponse
                    {
                        Success = false,
                        Message = "Немає з'єднання з сервісом"
                    };
                }
            }

            try
            {
                var request = new TextProcessingRequest
                {
                    Text = text,
                    Operation = operation,
                    UserId = Environment.UserName
                };

                // Простий протокол без JSON - використовуємо BinaryFormatter
                using (var memoryStream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(memoryStream, request);
                    byte[] requestData = memoryStream.ToArray();
                    byte[] lengthPrefix = BitConverter.GetBytes(requestData.Length);

                    // Відправка довжини повідомлення та самого повідомлення
                    await _networkStream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);
                    await _networkStream.WriteAsync(requestData, 0, requestData.Length);
                }

                // Читання відповіді
                byte[] lengthBuffer = new byte[4];
                await _networkStream.ReadAsync(lengthBuffer, 0, 4);
                int responseLength = BitConverter.ToInt32(lengthBuffer, 0);

                byte[] responseBuffer = new byte[responseLength];
                await _networkStream.ReadAsync(responseBuffer, 0, responseLength);
                
                using (var memoryStream = new MemoryStream(responseBuffer))
                {
                    var formatter = new BinaryFormatter();
                    return (TextProcessingResponse)formatter.Deserialize(memoryStream);
                }
            }
            catch (Exception ex)
            {
                return new TextProcessingResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<TextProcessingResponse> FormatTextAsync(string text, string formatType)
        {
            return await ProcessTextAsync(text, formatType);
        }

        public async Task<string[]> GetAvailableFormatsAsync()
        {
            try
            {
                var response = await ProcessTextAsync("", "getformats");
                if (response.Success && !string.IsNullOrEmpty(response.ProcessedText))
                {
                    return response.ProcessedText.Split(',');
                }
                return new string[0];
            }
            catch (Exception)
            {
                return new string[0];
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        _networkStream?.Close();
                        _tcpClient?.Close();
                    }
                    catch (Exception)
                    {
                        // Ігноруємо помилки при закритті
                    }
                    finally
                    {
                        _networkStream = null;
                        _tcpClient = null;
                    }
                }
                _disposed = true;
            }
        }
    }
}