using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Cache
{
    public partial class Form1 : Form
    {
        private const int CACHE_SERVER_PORT = 8080;
        private const string CACHE_SERVER_HOST = "127.0.0.1";
        private List<Dictionary<string, List<Tuple<int, int, int, string>>>> deserializedListOfDictionaries;
        private const int SERVER_PORT = 8081;
        private const string SERVER_HOST = "127.0.0.1";

        private TcpListener _listener;
        private Dictionary<string, byte[]> _cache;
        
        public Form1()
        {
            InitializeComponent();
            _cache = new Dictionary<string, byte[]>();
            // _cacheServer.LogCallback = Log;
            // _cacheServer.CacheStatusCallback = UpdateCacheStatus;
        }
        private void buttonStart_Click(object sender, EventArgs e)
        {
            _listener = new TcpListener(IPAddress.Parse(CACHE_SERVER_HOST), CACHE_SERVER_PORT);
            _listener.Start();

            lblCacheStatus.Text = $"Listening on {CACHE_SERVER_HOST}:{CACHE_SERVER_PORT}";

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    _ = ProcessRequestAsync(client);
                }
            });
        }

        private async Task ProcessRequestAsync(TcpClient client)
        {
            using (StreamReader reader = new StreamReader(client.GetStream(), Encoding.UTF8))
            using (StreamWriter writer = new StreamWriter(client.GetStream(), Encoding.UTF8) { AutoFlush = true })
            {
                string request = await reader.ReadLineAsync();

                if (request.StartsWith("GET_FILE"))
                {
                    string[] requestParts = request.Split(' ');
                    string fileName = requestParts[1];
                    foreach (var dict in deserializedListOfDictionaries)
                    {
                        foreach (var kv in dict)
                        {
                            if (kv.Key == fileName)
                            {
                                foreach (var tuple in kv.Value)
                                {
                                    string lastString = tuple.Item4;
                                    Console.WriteLine(lastString);
                                }
                            }
                            
                        }
                    }
                    //int startByte = int.Parse(requestParts[2]);
                    //int fragmentSize = int.Parse(requestParts[3]);
                    //string cacheKey = $"{fileName}_{startByte}";

                    if (_cache.ContainsKey(cacheKey))
                    {
                        await writer.WriteLineAsync(Convert.ToBase64String(_cache[cacheKey]));
                    }
                    else
                    {
                        using (TcpClient serverClient = new TcpClient())
                        {
                            await serverClient.ConnectAsync(SERVER_HOST, SERVER_PORT);
                            using (StreamReader serverReader = new StreamReader(serverClient.GetStream(), Encoding.UTF8))
                            using (StreamWriter serverWriter = new StreamWriter(serverClient.GetStream(), Encoding.UTF8) { AutoFlush = true })
                            {
                                await serverWriter.WriteLineAsync($"GET_FILE_FRAGMENT {fileName} {startByte} {fragmentSize}");
                                string fileFragmentBase64 = await serverReader.ReadLineAsync();

                                byte[] fileFragment = Convert.FromBase64String(fileFragmentBase64);
                                _cache[cacheKey] = fileFragment;

                                await writer.WriteLineAsync(fileFragmentBase64);
                            }
                        }
                    }
                }
                else if (request.StartsWith("LIST_FILES"))
                {
                    Log("Client request File List");
                    
                    using (TcpClient serverClient = new TcpClient())
                    {
                        await serverClient.ConnectAsync(SERVER_HOST, SERVER_PORT);
                        using (StreamReader serverReader = new StreamReader(serverClient.GetStream(), Encoding.UTF8))
                        using (StreamWriter serverWriter = new StreamWriter(serverClient.GetStream(), Encoding.UTF8) { AutoFlush = true })
                        {
                            await serverWriter.WriteLineAsync("LIST_FILES");
                            Log("Send LIST_FILES to Server");
                            string fileListJson = await serverReader.ReadLineAsync();
                            deserializedListOfDictionaries = JsonConvert.DeserializeObject<List<Dictionary<string, List<Tuple<int, int, int, string>>>>>(fileListJson);
                            
                            Log("Receive FIle_List from Server");
                            await writer.WriteLineAsync(fileListJson);
                            Log("File_List send to Client");
                        }
                    }
                }
                else
                {
                    await writer.WriteLineAsync("INVALID_REQUEST");
                }
            }
        }
        private void Log(string message)
        {
            string logFilePath = "log.txt";
            lblCacheStatus.Text = message;
            // Check if the log file exists and delete it before creating a new one
            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine($"{DateTime.Now}: {message}");
            }
        }
    }
}