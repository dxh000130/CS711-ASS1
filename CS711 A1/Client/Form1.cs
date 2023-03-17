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

namespace Client
{
    public partial class Form1 : Form
    {
        private const int CACHE_SERVER_PORT = 8080;
        private const string CACHE_SERVER_HOST = "127.0.0.1";
        public Form1()
        {
            InitializeComponent();
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
                    string[] files = fileList.Split(';');
                    label1.Text = "Reply received!";
                    // Update the ListBox with the list of files
                    listBoxFiles.Items.Clear();
                    listBoxFiles.Items.AddRange(files);
                }
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
                    await writer.FlushAsync();
        
                    // Read the file content from the cache server
                    string fileContent = await reader.ReadLineAsync();

                    label1.Text = fileContent;
                    byte[] fileBytes = Convert.FromBase64String(fileContent);

                    
                    
                    // Load the downloaded image into the PictureBox
                    using (MemoryStream stream = new MemoryStream(fileBytes))
                    {
                        pictureBoxPreview.Image = Image.FromStream(stream);
                    }
                }
            }
        }
    }
    
}