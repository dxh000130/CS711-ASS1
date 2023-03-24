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
                
            }
            if (File.Exists("Client_log.txt"))
            {
                File.Delete("Client_log.txt");
                Log("Log file exists and delete it");
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
            string logFilePath = "Client_detailed_log.txt";
            
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine($"{DateTime.Now}: {message}");
            }
        }
        private async void buttonDownloadFile_Click(object sender, EventArgs e)
        {
            var FileVailed = false;
            if (listBoxFiles.SelectedItem == null)
            {
                MessageBox.Show("Please select a file from the list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log("Error : Please select a file from the list.");
                return;
            }
            
            string fileName = listBoxFiles.SelectedItem.ToString();
            using (TcpClient client = new TcpClient())
            {
                Log("Connecting....");
                await client.ConnectAsync(CACHE_SERVER_HOST, CACHE_SERVER_PORT);
                Log("Connected");
                using (StreamReader reader = new StreamReader(client.GetStream(), Encoding.UTF8))
                using (StreamWriter writer = new StreamWriter(client.GetStream(), Encoding.UTF8))
                {
                    // Request the file from the cache server
                    await writer.WriteLineAsync($"Vaild_File_Change {fileName}");
                    Log("Send check file command to cache, Please Wait For response");
                    await writer.FlushAsync();
        
                    // Read the file content from the cache server
                    string VaildFile = await reader.ReadLineAsync();
                    Log("Received response from cache!");
                    Log_Detail(VaildFile);
                    // Convert from json to string list
                    if (VaildFile == "true")
                    {
                        FileVailed = true;
                        Log("File Doesn't not change.");
                    }
                    else if(VaildFile == "false")
                    {
                        Log("File Changed");
                        // Request the list of files from the cache server
                        Log("Refresh File List");
                        Log("Connecting....");
                        using (TcpClient client1 = new TcpClient())
                        {
                            await client1.ConnectAsync(CACHE_SERVER_HOST, CACHE_SERVER_PORT);
                            Log("Connected");
                            using (StreamReader reader1 = new StreamReader(client1.GetStream(), Encoding.UTF8))
                            using (StreamWriter writer1 = new StreamWriter(client1.GetStream(), Encoding.UTF8))
                            {
                                // Request the list of files from the cache server
                                await writer1.WriteLineAsync("LIST_FILES");
                                await writer1.FlushAsync();
                                Log("Sent Request! Please Wait For response");
                                // Read the file list from the cache server
                                string fileList = await reader1.ReadLineAsync();
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
                        FileVailed = true;
                    }
                }
            }
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
                    byte[] completeFileBytes = CombinehexadecimalList(resultList);

                    // Write the entire byte[] to a file.
                    string outputFilePath = fileName; // Output file
                    File.WriteAllBytes(outputFilePath, completeFileBytes);
                    Log("The file has been successfully merged and saved.");

                    // Load the downloaded image into the PictureBox
                    using (MemoryStream stream = new MemoryStream(completeFileBytes))
                    {
                        pictureBoxPreview.Image = Image.FromStream(stream);
                        pictureBoxPreview.Text = fileName;
                        pictureBoxPreview.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
            }
        }
        static byte[] CombinehexadecimalList(List<string> hexadecimalList)
        {
            List<byte[]> byteArrays = new List<byte[]>();

            // Decode hexadecimal into byte[].
            foreach (string hexadecimal in hexadecimalList)
            {
                int numberChars = hexadecimal.Length;
                byte[] decodedBytes = new byte[numberChars / 2];
                for (int i = 0; i < numberChars; i += 2)
                    decodedBytes[i / 2] = Convert.ToByte(hexadecimal.Substring(i, 2), 16);
                //byte[] decodedBytes = Convert.FromhexadecimalString(hexadecimal);
                byteArrays.Add(decodedBytes);
            }

            // Merge a list of byte[] into a single, complete byte[].
            byte[] completeFileBytes = byteArrays.SelectMany(byteArray => byteArray).ToArray();

            return completeFileBytes;
        }
        private void Open_Log_f(object sender, EventArgs eventArgs)
        {
            System.Diagnostics.Process.Start("explorer.exe", "/select," + "Client_log.txt");
        }
        private void Open_DLog_f(object sender, EventArgs eventArgs)
        {
            System.Diagnostics.Process.Start("explorer.exe", "/select," + "Client_detailed_log.txt");
        }
        private void Open_Download_file(object sender, EventArgs eventArgs)
        {
            System.Diagnostics.Process.Start("explorer.exe", "/select," + pictureBoxPreview.Text);
        }
    }
    
}