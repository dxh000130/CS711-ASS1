using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Client
{
    public partial class Form1 : Form
    {
        private const int CACHE_SERVER_PORT = 8080;
        private const string CACHE_SERVER_HOST = "127.0.0.1";
        public Form1()
        {
            InitializeComponent();
            if (File.Exists("Clientlog.txt"))
            {
                File.Delete("Clientlog.txt");
            }

        }
        public async void buttonRefreshList_Click(object sender, EventArgs e)
        {
            label1.Text = "Connecting....";
            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync(CACHE_SERVER_HOST, CACHE_SERVER_PORT);
                label1.Text = "Connected";
                using (StreamReader reader = new StreamReader(client.GetStream(), Encoding.UTF8))
                using (StreamWriter writer = new StreamWriter(client.GetStream(), Encoding.UTF8))
                {
                    // Request the list of files from the cache server
                    await writer.WriteLineAsync("LIST_FILES");
                    await writer.FlushAsync();
                    label1.Text = "Sent Request!";
                    // Read the file list from the cache server
                    string fileList = await reader.ReadLineAsync();
                    label1.Text = fileList;
                    List<Dictionary<string, List<Tuple<int, int, int, string>>>> deserializedListOfDictionaries = JsonConvert.DeserializeObject<List<Dictionary<string, List<Tuple<int, int, int, string>>>>>(fileList);
                    //string[] files = fileList.Split(';');
                    label1.Text = "Reply received!";
                    // Update the ListBox with the list of files
                    listBoxFiles.Items.Clear();
                    foreach (var dictionary in deserializedListOfDictionaries)
                    {
                        foreach (var entry in dictionary)
                        {
                            listBoxFiles.Items.Add(entry.Key);
                        }
                    }
                    //Log(ConvertListOfDictionariesToString(deserializedListOfDictionaries));
                }
            }
        }
        public string ConvertListOfDictionariesToString(List<Dictionary<string, List<Tuple<int, int, int, string>>>> listOfDictionaries)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var dictionary in listOfDictionaries)
            {
                foreach (var kvp in dictionary)
                {
                    string fileName = kvp.Key;
                    List<Tuple<int, int, int, string>> fileDataList = kvp.Value;

                    sb.AppendLine($"File: {fileName}");

                    foreach (var tuple in fileDataList)
                    {
                        int blockIndex = tuple.Item1;
                        int startByte = tuple.Item2;
                        int endByte = tuple.Item3;
                        string hash = tuple.Item4;

                        sb.AppendLine($"\tBlock {blockIndex}: Start Byte: {startByte}, End Byte: {endByte}, Hash: {hash}");
                    }
                }
            }

            return sb.ToString();
        }
        private void Log(string message)
        {
            string logFilePath = "Clientlog.txt";
    
            // Check if the log file exists and delete it before creating a new one
            
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine($"{DateTime.Now}: {message}");
            }
        }
        private async void buttonDownloadFile_Click(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedItem == null)
            {
                MessageBox.Show("Please select a file from the list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        
            string fileName = listBoxFiles.SelectedItem.ToString();
        
            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync(CACHE_SERVER_HOST, CACHE_SERVER_PORT);
        
                using (StreamReader reader = new StreamReader(client.GetStream(), Encoding.UTF8))
                using (StreamWriter writer = new StreamWriter(client.GetStream(), Encoding.UTF8))
                {
                    // Request the file from the cache server
                    await writer.WriteLineAsync($"GET_FILE {fileName}");
                    Log("发送下载文件指令");
                    await writer.FlushAsync();
        
                    // Read the file content from the cache server
                    string fileContent = await reader.ReadLineAsync();
                    Log("收到回复");
                    Log(fileContent);
                    List<string> resultList = JsonConvert.DeserializeObject<List<string>>(fileContent);
                    byte[] completeFileBytes = CombineBase64List(resultList);

                    // 将完整的byte[]写入文件
                    string outputFilePath = fileName; // 输出文件路径
                    File.WriteAllBytes(outputFilePath, completeFileBytes);
                    label1.Text = "文件已成功合并和保存.";
                    
                    // Load the downloaded image into the PictureBox
                    using (MemoryStream stream = new MemoryStream(completeFileBytes))
                    {
                        pictureBoxPreview.Image = Image.FromStream(stream);
                    }
                }
            }
        }
        static byte[] CombineBase64List(List<string> base64List)
        {
            List<byte[]> byteArrays = new List<byte[]>();

            // 将base64解码为byte[]
            foreach (string base64 in base64List)
            {
                byte[] decodedBytes = Convert.FromBase64String(base64);
                byteArrays.Add(decodedBytes);
            }

            // 合并byte[]列表为一个完整的byte[]
            byte[] completeFileBytes = byteArrays.SelectMany(byteArray => byteArray).ToArray();

            return completeFileBytes;
        }
    }
    
}