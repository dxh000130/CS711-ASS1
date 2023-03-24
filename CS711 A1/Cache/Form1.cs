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
using System.Threading;
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
        Cached_File_List newForm = new Cached_File_List();
        Cached_File_Block_List newForm2 = new Cached_File_Block_List();
        
        public Form1()
        {
            InitializeComponent();
            _cache = new Dictionary<string, string>();
            Server_IP_label.Text = SERVER_HOST + ":" + SERVER_PORT;
            Client_IP_label.Text = CACHE_SERVER_HOST + ":" + CACHE_SERVER_PORT;
            if (!Directory.Exists(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log"))))
            {
                Directory.CreateDirectory(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log")));
            }
            // Check if the log file exists and delete it
            if (File.Exists("./Log/Cache_log.txt"))
            {
                File.Delete("./Log/Cache_log.txt");
                Log("Log file exists and delete it");
            }
            if (File.Exists("./Log/Cache_detailed_log.txt"))
            {
                File.Delete("./Log/Cache_detailed_log.txt");
            }
        }
        private void buttonStart_Click(object sender, EventArgs e)
        {
            _listener = new TcpListener(IPAddress.Parse(CACHE_SERVER_HOST), CACHE_SERVER_PORT);
            _listener.Start();
            btnStart.Enabled = false;
            Log($"Listening on {CACHE_SERVER_HOST}:{CACHE_SERVER_PORT}");

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    _ = ProcessRequestAsync(client);
                }
            });
        }
        private void button_Cached_File_list_Click(object sender, EventArgs e)
        {
            

            // Display Cached_File_List
            newForm.Show();

        }
        private void button_Cached_FileBlock_list_Click(object sender, EventArgs e)
        {
            

            // Display Cached_File_List
            newForm2.Show();

            // 添加所有文件
            foreach (KeyValuePair<string, string> file_ in _cache)
            {
                newForm2.listBox1.Items.Add(file_.Key + " : " + file_.Value);
            }
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
                                List<string> File_BLock_hexadecimal = new List<string>();
                                var Total_number_of_file_blocks = 0;
                                var Number_of_file_blocks_that_exist = 0;
                                foreach (var tuple in kv.Value)
                                {
                                    Total_number_of_file_blocks += 1;
                                    string file_block_hash = tuple.Item4;
                                    if (_cache.ContainsKey(file_block_hash))
                                    {
                                        Number_of_file_blocks_that_exist += 1;
                                        Log_Detail("file_block_hash Exist!");
                                        File_BLock_hexadecimal.Add(_cache[file_block_hash]);
                                        Log_Detail("Add " + _cache[file_block_hash]);
                                    }
                                    else
                                    {
                                        Log_Detail("file_block_hash Doesn't Exist!");
                                        using (TcpClient serverClient = new TcpClient())
                                        {
                                            await serverClient.ConnectAsync(SERVER_HOST, SERVER_PORT);
                                            Log_Detail("Connect To Server");
                                            using (StreamReader serverReader = new StreamReader(serverClient.GetStream(), Encoding.UTF8))
                                            using (StreamWriter serverWriter = new StreamWriter(serverClient.GetStream(), Encoding.UTF8) { AutoFlush = true })
                                            {
                                                await serverWriter.WriteLineAsync($"GET_FILE_FRAGMENT {fileName} {tuple.Item2} {fragmentSize}");
                                                Log_Detail("Send request To Server");
                                                string fileFragmenthexadecimal = await serverReader.ReadLineAsync();
                                                Log_Detail("Server reply: " + fileFragmenthexadecimal);
                                                _cache[file_block_hash] = fileFragmenthexadecimal;
                                                newForm2.listBox1.Items.Add(file_block_hash + " : " + fileFragmenthexadecimal);
                                                Log_Detail("Add " + fileFragmenthexadecimal);
                                                File_BLock_hexadecimal.Add(fileFragmenthexadecimal);
                                            }
                                        }
                                    }
                                }

                                Log("response: " + (double)Number_of_file_blocks_that_exist/Total_number_of_file_blocks*100 + "% of file " + fileName +
                                    " was constructed with the cached data. " + (Total_number_of_file_blocks-Number_of_file_blocks_that_exist).ToString() + " file chunks need to be downloaded from the server.");
                                string jsonString = JsonConvert.SerializeObject(File_BLock_hexadecimal);
                                await writer.WriteLineAsync(jsonString);

                                newForm.listBox1.Items.Add(fileName);
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
                }else if (request.StartsWith("Vaild_File_Change"))
                {
                    Log("Client request Vaild File change or not");
                    
                    using (TcpClient serverClient = new TcpClient())
                    {
                        await serverClient.ConnectAsync(SERVER_HOST, SERVER_PORT);
                        using (StreamReader serverReader = new StreamReader(serverClient.GetStream(), Encoding.UTF8))
                        using (StreamWriter serverWriter = new StreamWriter(serverClient.GetStream(), Encoding.UTF8) { AutoFlush = true })
                        {
                            await serverWriter.WriteLineAsync(request);
                            Log("Send Vaild_File_Change to Server");
                            string fileListJson = await serverReader.ReadLineAsync();

                            Log("Receive response from Server");
                            await writer.WriteLineAsync(fileListJson);
                            Log("Vaild_File_Change send to Client");
                        }
                    }
                }
            }
        }
        private void Log(string message)
        {
            string logFilePath = "./Log/Cache_log.txt";
            lblCacheStatus.Text = message;
            Log_Detail(message);
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine($"{DateTime.Now}: {message}");
            }
        }
        private void Log_Detail(string message)
        {
            string logFilePath = "./Log/Cache_detailed_log.txt";

            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine($"{DateTime.Now}: {message}");
            }
        }
        private void Clear_Button(object sender, EventArgs eventArgs)
        {
            _cache = new Dictionary<string, string>();
            Log("Clear Cache!");
        }

        private void Open_Log_f(object sender, EventArgs eventArgs)
        {
            System.Diagnostics.Process.Start("explorer.exe", "/select," + Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", "Cache_log.txt")));
        }
        private void Open_DLog_f(object sender, EventArgs eventArgs)
        {
            System.Diagnostics.Process.Start("explorer.exe", "/select," + Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", "Cache_detailed_log.txt")));
        }
    }
}