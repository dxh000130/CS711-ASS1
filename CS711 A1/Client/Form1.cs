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
            // Check if the log file exists and delete it
            if (File.Exists("Client_detailed_log.txt"))
            {
                File.Delete("Client_detailed_log.txt");
                Log("Log file exists and delete it");
            }
            if (File.Exists("Client_log.txt"))
            {
                File.Delete("Client_log.txt");
            }

        }
        public async void buttonRefreshList_Click(object sender, EventArgs e)
        {
            Log("Refresh File List");
            Log("Connecting....");
            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync(CACHE_SERVER_HOST, CACHE_SERVER_PORT);
                Log("Connected");
                using (StreamReader reader = new StreamReader(client.GetStream(), Encoding.UTF8))
                using (StreamWriter writer = new StreamWriter(client.GetStream(), Encoding.UTF8))
                {
                    // Request the list of files from the cache server
                    await writer.WriteLineAsync("LIST_FILES");
                    await writer.FlushAsync();
                    Log("Sent Request! Please Wait For response");
                    // Read the file list from the cache server
                    string fileList = await reader.ReadLineAsync();
                    // Convert to List<Dictionary<string, List<Tuple<int, int, int, string>>>> from json
                    List<Dictionary<string, List<Tuple<int, int, int, string>>>> deserializedListOfDictionaries = JsonConvert.DeserializeObject<List<Dictionary<string, List<Tuple<int, int, int, string>>>>>(fileList);
                    Log("Reply received!");
                    // Update the ListBox with the list of files
                    listBoxFiles.Items.Clear();
                    foreach (var dictionary in deserializedListOfDictionaries)
                    {
                        foreach (var entry in dictionary)
                        {
                            listBoxFiles.Items.Add(entry.Key);
                        }
                    }
                    Log_Detail(ConvertListOfDictionariesToString(deserializedListOfDictionaries));
                }
            }
        }
        public string ConvertListOfDictionariesToString(List<Dictionary<string, List<Tuple<int, int, int, string>>>> listOfDictionaries)
        // Displaying hash values for all file blocks in each file.
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
            string logFilePath = "Client_log.txt";
            label1.Text = message;
            Log_Detail(message);
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine($"{DateTime.Now}: {message}");
            }
        }
        private void Log_Detail(string message)
        {
            string logFilePath = "Client_detailed_log";
            
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
                Log("Error : Please select a file from the list.");
                return;
            }
        
            string fileName = listBoxFiles.SelectedItem.ToString();
            Log("Select " + fileName);
            using (TcpClient client = new TcpClient())
            {
                Log("Connecting....");
                await client.ConnectAsync(CACHE_SERVER_HOST, CACHE_SERVER_PORT);
                Log("Connected");
                using (StreamReader reader = new StreamReader(client.GetStream(), Encoding.UTF8))
                using (StreamWriter writer = new StreamWriter(client.GetStream(), Encoding.UTF8))
                {
                    // Request the file from the cache server
                    await writer.WriteLineAsync($"GET_FILE {fileName}");
                    Log("Send download file command to cache, Please Wait For response");
                    await writer.FlushAsync();
        
                    // Read the file content from the cache server
                    string fileContent = await reader.ReadLineAsync();
                    Log("Received response from cache!");
                    Log_Detail(fileContent);
                    // Convert from json to string list
                    List<string> resultList = JsonConvert.DeserializeObject<List<string>>(fileContent);
                    byte[] completeFileBytes = CombineBase64List(resultList);

                    // Write the entire byte[] to a file.
                    string outputFilePath = fileName; // Output file
                    File.WriteAllBytes(outputFilePath, completeFileBytes);
                    Log("The file has been successfully merged and saved.");

                    // Load the downloaded image into the PictureBox
                    using (MemoryStream stream = new MemoryStream(completeFileBytes))
                    {
                        pictureBoxPreview.Image = Image.FromStream(stream);
                        pictureBoxPreview.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
            }
        }
        static byte[] CombineBase64List(List<string> base64List)
        {
            List<byte[]> byteArrays = new List<byte[]>();

            // Decode base64 into byte[].
            foreach (string base64 in base64List)
            {
                int numberChars = base64.Length;
                byte[] decodedBytes = new byte[numberChars / 2];
                for (int i = 0; i < numberChars; i += 2)
                    decodedBytes[i / 2] = Convert.ToByte(base64.Substring(i, 2), 16);
                //byte[] decodedBytes = Convert.FromBase64String(base64);
                byteArrays.Add(decodedBytes);
            }

            // Merge a list of byte[] into a single, complete byte[].
            byte[] completeFileBytes = byteArrays.SelectMany(byteArray => byteArray).ToArray();

            return completeFileBytes;
        }
    }
    
}