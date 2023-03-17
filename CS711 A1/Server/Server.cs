using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server
{
    public class Server
{
    public const int SERVER_PORT = 8083;
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
        using (StreamWriter writer = new StreamWriter(client.GetStream(), Encoding.UTF8) { AutoFlush = true })
        {
            string request = await reader.ReadLineAsync();
            
            if (request.StartsWith("LIST_FILES"))
            {
                StatusLabelCallback?.Invoke("Client request file list.");
                // Send the list of files available on the server
                string fileList = GetFileList();
                StatusLabelCallback?.Invoke(fileList);
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
        int blockSize = 2048; // 2KB
        // Get the file names without the full path
        List<Dictionary<string, Tuple<int, int, int, string>>> listOfDictionaries = new List<Dictionary<string, Tuple<int, int, int, string>>>();
        for (int i = 0; i < files.Length; i++)
        {
            var blockHashes = GetFileBlockHashes(files[i], blockSize);

             // for (int j = 0; j < blockHashes.Count; j++)
             // {
             //     Console.WriteLine($"Block {i}: {blockHashes[i]}");
             // }
             StatusLabelCallback?.Invoke(files[i]);
             listOfDictionaries.Add(blockHashes);
             
            //files[i] = blockHashes;
        }

        // Combine the file names into a single string, separated by semicolons
        string jsonString = JsonConvert.SerializeObject(listOfDictionaries);
        //string fileList = string.Join(";", files);
        return jsonString;
    }
    private static Dictionary<string, Tuple<int, int, int, string>> GetFileBlockHashes(string filePath, int blockSize)
    {
        var blockHashes = new Dictionary<string, Tuple<int, int, int, string>>();

        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            byte[] buffer = new byte[blockSize];
            int bytesRead;
            int blockIndex = 0;
            int startByte = 0;

            while ((bytesRead = fileStream.Read(buffer, 0, blockSize)) > 0)
            {
                // Resize the buffer if necessary (for the last block)
                if (bytesRead < blockSize)
                {
                    Array.Resize(ref buffer, bytesRead);
                }

                // Compute the hash for the current block
                string hash = ComputeHash(buffer);
                int endByte = startByte + bytesRead - 1;
                blockHashes.Add(Path.GetFileName(filePath), Tuple.Create(blockIndex, startByte, endByte, hash));

                startByte = endByte + 1;
                blockIndex++;
            }
        }

        return blockHashes;
    }

    private static string ComputeHash(byte[] data)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(data);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
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