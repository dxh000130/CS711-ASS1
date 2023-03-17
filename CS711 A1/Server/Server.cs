using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Server
{
    public const int SERVER_PORT = 8081;
    public const string SERVER_HOST = "127.0.0.1";
    private readonly TcpListener _listener;

    public Server()
    {
        _listener = new TcpListener(IPAddress.Parse(SERVER_HOST), SERVER_PORT);
        
    }
    public Action<string> OutputCallback { get; set; }
    public Action<string> StatusLabelCallback { get; set; }
    public async Task StartAsync()
    {
        OutputCallback?.Invoke("Starting listening: " + IPAddress.Parse(SERVER_HOST) + ":" + SERVER_PORT);
        _listener.Start();
        OutputCallback?.Invoke("Start listening: " + IPAddress.Parse(SERVER_HOST) + ":" + SERVER_PORT + ", waiting for connection");

        while (true)
        {
            TcpClient client = await _listener.AcceptTcpClientAsync();
            _ = ProcessRequestAsync(client);
            OutputCallback?.Invoke("Connected: " + IPAddress.Parse(SERVER_HOST) + ":" + SERVER_PORT);
        }
    }

    private async Task ProcessRequestAsync(TcpClient client)
    {

        using (StreamReader reader = new StreamReader(client.GetStream(), Encoding.UTF8))
        using (StreamWriter writer = new StreamWriter(client.GetStream(), Encoding.UTF8))
        {
            string request = await reader.ReadLineAsync();
            
            if (request.StartsWith("LIST_FILES"))
            {
                StatusLabelCallback?.Invoke("Client request file list.");
                // Send the list of files available on the server
                string fileList = GetFileList();
                await writer.WriteLineAsync(fileList);
                StatusLabelCallback?.Invoke("File list sent.");
            }
            else if (request.StartsWith("GET_FILE_FRAGMENT"))
            {
                // Serve a file fragment from the server
                string[] requestParts = request.Split(' ');
                string fileName = requestParts[1];
                StatusLabelCallback?.Invoke(fileName);
                int startByte = int.Parse(requestParts[2]);
                int fragmentSize = int.Parse(requestParts[3]);

                byte[] fileFragment = await ServeFileFragmentAsync(fileName, startByte, fragmentSize);
                await writer.WriteLineAsync(Convert.ToBase64String(fileFragment));
            }

            await writer.FlushAsync();
        }

        client.Close();
    }

    private string GetFileList()
    {
        
        string serverFilesDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "File_Storage"));

        // If the directory does not exist, create it
        if (!Directory.Exists(serverFilesDirectory))
        {
            Directory.CreateDirectory(serverFilesDirectory);
        }

        // Get the list of files in the "File_Storage" directory
        string[] files = Directory.GetFiles(serverFilesDirectory);

        // Get the file names without the full path
        for (int i = 0; i < files.Length; i++)
        {
            files[i] = Path.GetFileName(files[i]);
        }

        // Combine the file names into a single string, separated by semicolons
        string fileList = string.Join(";", files);
        return fileList;
    }

    private async Task<byte[]> ServeFileFragmentAsync(string fileName, int startByte, int fragmentSize)
    {
        // Serve a file fragment from the server
        string filePath = Path.Combine("server_files", fileName); // Replace "server_files" with the actual directory holding the files
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            byte[] buffer = new byte[fragmentSize];
            fileStream.Seek(startByte, SeekOrigin.Begin);
            int bytesRead = await fileStream.ReadAsync(buffer, 0, fragmentSize);
            Array.Resize(ref buffer, bytesRead);
            return buffer;
        }
    }
}
}