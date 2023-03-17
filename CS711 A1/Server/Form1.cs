using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        private Server server;
        public Form1()
        {
            InitializeComponent();
            server = new Server();
            LoadFileList();
            server.OutputCallback = UpdateIPLable;
            server.StatusLabelCallback = UpdateStatusLable;
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            StorageFilePathLabel.Text = Path.GetFullPath(Path.Combine(currentDirectory, "..", "..", ".."));
            IPLable.Text = "Stop listening: " + IPAddress.Parse(Server.SERVER_HOST) + ":" + Server.SERVER_PORT.ToString();
        }
        private void LoadFileList()
        {
            // 指定目录
            string directoryPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "File_Storage"));
            // 获取目录中的所有文件
            string[] files = Directory.GetFiles(directoryPath);
            // 清空列表
            FilesListbox.Items.Clear();
            // 添加所有文件
            foreach (string file in files)
            {
                FilesListbox.Items.Add(Path.GetFileName(file));
            }
        }
        private void UpdateIPLable(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateIPLable), text);
                return;
            }

            IPLable.Text = text;
        }
        private void UpdateStatusLable(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateStatusLable), text);
                return;
            }

            StatusLabel.Text = text;
        }
        private async void startButton_Click(object sender, EventArgs e)
        {
            startButton.Enabled = false;
            //StorageFilePathLabel.Text = "Starting server...";
            try
            {
                await server.StartAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
            
            //StorageFilePathLabel.Text = "Server started";
        }
        private void btnAddFile_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
            openFileDialog.Title = "Select an Image File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sourceFilePath = openFileDialog.FileName;
                string destinationDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "File_Storage"));
                string destinationFilePath = Path.Combine(destinationDirectory, Path.GetFileName(sourceFilePath));

                if (!Directory.Exists(destinationDirectory))
                {
                    StatusLabel.Text = "文件夹不存在，创立新文件夹";
                    Directory.CreateDirectory(destinationDirectory);
                }
                if (File.Exists(destinationFilePath))
                {
                    StatusLabel.Text = @"File already exist";
                }
                else
                {
                    File.Copy(sourceFilePath, destinationFilePath, true);
                    StatusLabel.Text = $@"File added: {Path.GetFileName(sourceFilePath)}";
                    LoadFileList();
                }
            }
        }
    }
}