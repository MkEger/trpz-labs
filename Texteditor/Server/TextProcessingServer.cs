using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Texteditor.Contracts;
using Texteditor.Services;

namespace Texteditor.Server
{
    /// <summary>
    /// TCP Server for SOA text processing service
    /// </summary>
    public class TextProcessingServer
    {
        private TcpListener _tcpListener;
        private readonly TextProcessingService _textService;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isRunning = false;

        public event Action<string> LogMessage;
        public bool IsRunning => _isRunning;

        public TextProcessingServer(int port = 8080)
        {
            _tcpListener = new TcpListener(IPAddress.Any, port);
            _textService = new TextProcessingService();
        }

        public async Task StartAsync()
        {
            if (_isRunning) return;

            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _tcpListener.Start();
                _isRunning = true;
                
                LogMessage?.Invoke($"SOA сервер запущено на порту {((IPEndPoint)_tcpListener.LocalEndpoint).Port}");
                
                // Асинхронно обробляємо з'єднання
                _ = Task.Run(async () => await AcceptClientsAsync(_cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Помилка запуску сервера: {ex.Message}");
                throw;
            }
        }

        private async Task AcceptClientsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && _isRunning)
            {
                try
                {
                    var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                    LogMessage?.Invoke($"Клієнт підключився: {tcpClient.Client.RemoteEndPoint}");
                    
                    // Обробляємо кожного клієнта в окремому таску
                    _ = Task.Run(async () => await HandleClientAsync(tcpClient, cancellationToken));
                }
                catch (ObjectDisposedException)
                {
                    // Сервер був зупинений
                    break;
                }
                catch (Exception ex)
                {
                    if (_isRunning)
                    {
                        LogMessage?.Invoke($"Помилка прийняття з'єднання: {ex.Message}");
                    }
                }
            }
        }

        private async Task HandleClientAsync(TcpClient tcpClient, CancellationToken cancellationToken)
        {
            NetworkStream networkStream = null;
            
            try
            {
                networkStream = tcpClient.GetStream();
                
                while (tcpClient.Connected && !cancellationToken.IsCancellationRequested)
                {
                    // Читаємо довжину повідомлення (4 байти)
                    byte[] lengthBuffer = new byte[4];
                    int bytesRead = await networkStream.ReadAsync(lengthBuffer, 0, 4, cancellationToken);
                    
                    if (bytesRead == 0) break; // Клієнт відключився
                    
                    int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
                    
                    // Читаємо саме повідомлення
                    byte[] messageBuffer = new byte[messageLength];
                    bytesRead = await networkStream.ReadAsync(messageBuffer, 0, messageLength, cancellationToken);
                    
                    if (bytesRead == 0) break;
                    
                    TextProcessingRequest request;
                    using (var memoryStream = new MemoryStream(messageBuffer))
                    {
                        var formatter = new BinaryFormatter();
                        request = (TextProcessingRequest)formatter.Deserialize(memoryStream);
                    }
                    
                    LogMessage?.Invoke($"Отримано запит: {request.Operation}");
                    
                    // Обробка запиту
                    TextProcessingResponse response;
                    if (request.Operation.ToLower() == "getformats")
                    {
                        var formats = _textService.GetAvailableFormats();
                        response = new TextProcessingResponse
                        {
                            Success = true,
                            ProcessedText = string.Join(",", formats),
                            Message = "Формати отримано"
                        };
                    }
                    else
                    {
                        response = _textService.ProcessText(request);
                    }
                    
                    // Серіалізація відповіді
                    byte[] responseData;
                    using (var memoryStream = new MemoryStream())
                    {
                        var formatter = new BinaryFormatter();
                        formatter.Serialize(memoryStream, response);
                        responseData = memoryStream.ToArray();
                    }
                    
                    byte[] responseLengthPrefix = BitConverter.GetBytes(responseData.Length);
                    
                    // Відправка відповіді
                    await networkStream.WriteAsync(responseLengthPrefix, 0, responseLengthPrefix.Length, cancellationToken);
                    await networkStream.WriteAsync(responseData, 0, responseData.Length, cancellationToken);
                    
                    LogMessage?.Invoke($"Відправлено відповідь: {response.Success}");
                }
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Помилка обробки клієнта: {ex.Message}");
            }
            finally
            {
                try
                {
                    networkStream?.Close();
                    tcpClient?.Close();
                    LogMessage?.Invoke("Клієнт відключився");
                }
                catch (Exception ex)
                {
                    LogMessage?.Invoke($"Помилка закриття з'єднання: {ex.Message}");
                }
            }
        }

        public void Stop()
        {
            if (!_isRunning) return;

            try
            {
                _isRunning = false;
                _cancellationTokenSource?.Cancel();
                _tcpListener?.Stop();
                
                LogMessage?.Invoke("SOA сервер зупинено");
            }
            catch (Exception ex)
            {
                LogMessage?.Invoke($"Помилка зупинки сервера: {ex.Message}");
            }
        }

        public void Dispose()
        {
            Stop();
            _cancellationTokenSource?.Dispose();
        }
    }
}