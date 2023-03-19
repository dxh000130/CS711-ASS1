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
        private Dictionary<string, string> _cache;
        
        public Form1()
        {
            InitializeComponent();
            _cache = new Dictionary<string, string>();
            if (File.Exists("Cachelog.txt"))
            {
                File.Delete("Cachelog.txt");
            }
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
                Log(request);
                if (request.StartsWith("GET_FILE"))
                {
                    string[] requestParts = request.Split(' ');
                    string fileName = requestParts[1];
                    Log("Client Request Download File: " + fileName);
                    var fragmentSize = 2048;
                    foreach (var dict in deserializedListOfDictionaries)
                    {
                        foreach (var kv in dict)
                        {
                            if (kv.Key == fileName)
                            {
                                Log("Find File Name in File List!");
                                List<string> File_BLock_Base64 = new List<string>();
                                foreach (var tuple in kv.Value)
                                {
                                    string file_block_hash = tuple.Item4;
                                    if (_cache.ContainsKey(file_block_hash))
                                    {
                                        Log("file_block_hash Exist!");
                                        File_BLock_Base64.Add(_cache[file_block_hash]);
                                        Log("Add " + _cache[file_block_hash]);
                                        //await writer.WriteLineAsync(Convert.ToBase64String(_cache[file_block_hash]));
                                    }
                                    else
                                    {
                                        Log("file_block_hash Doesn't Exist!");
                                        using (TcpClient serverClient = new TcpClient())
                                        {
                                            await serverClient.ConnectAsync(SERVER_HOST, SERVER_PORT);
                                            Log("Connect To Server");
                                            using (StreamReader serverReader = new StreamReader(serverClient.GetStream(), Encoding.UTF8))
                                            using (StreamWriter serverWriter = new StreamWriter(serverClient.GetStream(), Encoding.UTF8) { AutoFlush = true })
                                            {
                                                await serverWriter.WriteLineAsync($"GET_FILE_FRAGMENT {fileName} {tuple.Item2} {fragmentSize}");
                                                Log("Send request To Server");
                                                string fileFragmentBase64 = await serverReader.ReadLineAsync();
                                                Log("Server reply" + fileFragmentBase64);
                                                //byte[] fileFragment = Convert.FromBase64String(fileFragmentBase64);
                                                _cache[file_block_hash] = fileFragmentBase64;
                                                Log("Add " + fileFragmentBase64);
                                                File_BLock_Base64.Add(fileFragmentBase64);
                                                //await writer.WriteLineAsync(fileFragmentBase64);
                                            }
                                        }
                                    }
                                }
                                Log("转换前" + File_BLock_Base64);
                                string jsonString = JsonConvert.SerializeObject(File_BLock_Base64);
                                Log("转换后" + jsonString);
                                await writer.WriteLineAsync(jsonString);
                                Log("Send back to Client.");
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
            string logFilePath = "Cachelog.txt";
            lblCacheStatus.Text = message;
            // Check if the log file exists and delete it before creating a new one
           

            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine($"{DateTime.Now}: {message}");
            }
        }
    }
}