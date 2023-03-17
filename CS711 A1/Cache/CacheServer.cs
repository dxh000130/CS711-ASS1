using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CacheServer
{
    public class CacheServer
    {
        private const int CACHE_SERVER_PORT = 8080;
        private const string CACHE_SERVER_HOST = "127.0.0.1";
        private const string SERVER_HOST = "127.0.0.1";
        private const int SERVER_PORT = 8081;
        private readonly TcpListener _listener;
        private bool _isRunning;

        public CacheServer()
        {
            _listener = new TcpListener(IPAddress.Parse(CACHE_SERVER_HOST), CACHE_SERVER_PORT);
            _isRunning = false;
        }

        public Action<string> LogCallback { get; set; }
        public Action<string> CacheStatusCallback { get; set; }

        public void Start()
        {
            _listener.Start();
            _isRunning = true;
            Task.Run(() => ListenLoop());
        }

        public void Stop()
        {
            _isRunning = false;
            _listener.Stop();
        }

        private async Task ListenLoop()
        {
            while (_isRunning)
            {
                try
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client);
                }
                catch (Exception ex)
                {
                    LogCallback?.Invoke($"Error: {ex.Message}");
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (StreamReader reader = new StreamReader(client.GetStream(), Encoding.UTF8))
            using (StreamWriter writer = new StreamWriter(client.GetStream(), Encoding.UTF8))
            {
                string request = await reader.ReadLineAsync();

                if (request.StartsWith("LIST_FILES"))
                {
                    // Forward the list files request to the origin server
                    string fileList = await RequestFileListAsync();
                    await writer.WriteLineAsync(fileList);
                }
                else if (request.StartsWith("GET_FILE"))
                {
                    string fileName = request.Substring("GET_FILE ".Length);
                    string fileContent = await RequestFileAsync(fileName);
                    await writer.WriteLineAsync(fileContent);
                }

                await writer.FlushAsync();
            }

            client.Close();
        }

        private async Task<string> RequestFileListAsync()
        {
            using (TcpClient serverClient = new TcpClient())
            {
                await serverClient.ConnectAsync(SERVER_HOST, SERVER_PORT);
                using (StreamReader reader = new StreamReader(serverClient.GetStream(), Encoding.UTF8))
                using (StreamWriter writer = new StreamWriter(serverClient.GetStream(), Encoding.UTF8))
                {
                    await writer.WriteLineAsync("LIST_FILES");
                    await writer.FlushAsync();
                    return await reader.ReadLineAsync();
                }
            }
        }

        private async Task<string> RequestFileAsync(string fileName)
        {
            using (TcpClient serverClient = new TcpClient())
            {
                await serverClient.ConnectAsync(SERVER_HOST, SERVER_PORT);
                using (StreamReader reader = new StreamReader(serverClient.GetStream(), Encoding.UTF8))
                using (StreamWriter writer = new StreamWriter(serverClient.GetStream(), Encoding.UTF8))
                {
                    await writer.WriteLineAsync($"GET_FILE {fileName}");
                    await writer.FlushAsync();
                    return await reader.ReadLineAsync();
                }
            }
        }
    }
}
