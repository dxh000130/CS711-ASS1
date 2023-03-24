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
            if (!Directory.Exists(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log"))))
            {
                Directory.CreateDirectory(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log")));
            }
            if (File.Exists(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", "Server_detailed_log.txt"))))
            {
                File.Delete(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", "Server_detailed_log.txt")));
                
            }
            if (File.Exists(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", "Server_log.txt"))))
            {
                File.Delete(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", "Server_log.txt")));
                Log("Log file exists and delete it");
            }
            LoadFileList();
            server.OutputCallback = UpdateIPLable;
            server.StatusLabelCallback = UpdateStatusLable;
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            StorageFilePathLabel.Text = Path.GetFullPath(Path.Combine(currentDirectory, "..", "..", ".."));
            IPLable.Text = "Not Start listening: " + IPAddress.Parse(Server.SERVER_HOST) + ":" + Server.SERVER_PORT.ToString();
            Log("Not Start listening: " + IPAddress.Parse(Server.SERVER_HOST) + ":" +
                       Server.SERVER_PORT.ToString());
            // Check if the log file exists and delete it
            
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
            Log("List all files.");
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
                Log(text);
                Invoke(new Action<string>(UpdateStatusLable), text);
                return;
            }

            StatusLabel.Text = text;
        }
        private async void startButton_Click(object sender, EventArgs e)
        {
            startButton.Enabled = false;
            try
            {
                await server.StartAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
            
        }
        private void btnAddFile_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.gif;*.png)|*.bmp;*.jpg;*.jpeg;*.gif;*.png";
            openFileDialog.Title = "Select an Image File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string sourceFilePath = openFileDialog.FileName;
                string destinationDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "File_Storage"));
                string destinationFilePath = Path.Combine(destinationDirectory, Path.GetFileName(sourceFilePath));

                if (!Directory.Exists(destinationDirectory))
                {
                    StatusLabel.Text = "Destination Folder does not exist, creating a new folder.";
                    Log("Destination Folder does not exist, creating a new folder.");
                    Directory.CreateDirectory(destinationDirectory);
                }
                if (File.Exists(destinationFilePath))
                {
                    StatusLabel.Text = @"File already exist";
                    Log("File already exist");
                }
                else
                {
                    File.Copy(sourceFilePath, destinationFilePath, true);
                    StatusLabel.Text = $@"File added: {Path.GetFileName(sourceFilePath)}";
                    Log($@"File added: {Path.GetFileName(sourceFilePath)}");
                    LoadFileList();
                }
            }
        }

        private static void Log(string message)
        {
            string logFilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", "Server_log.txt"));
            Log_Detail(message);
        
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine($"{DateTime.Now}: {message}");
            }
        }

        private static void Log_Detail(string message)
        {
            string logFilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", "Server_detailed_log.txt"));
        
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine($"{DateTime.Now}: {message}");
            }
        }
    }
}