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
    public const int SERVER_PORT = 8081;
    public const string SERVER_HOST = "127.0.0.1";
    private readonly TcpListener _listener;
    private static Dictionary<string, string> FileHash; 

    public Server()
    {
        _listener = new TcpListener(IPAddress.Parse(SERVER_HOST), SERVER_PORT);
        FileHash = new Dictionary<string, string>();
    }
    public Action<string> OutputCallback { get; set; }
    public Action<string> StatusLabelCallback { get; set; }
    public async Task StartAsync()
    {
        OutputCallback?.Invoke("Starting listening: " + IPAddress.Parse(SERVER_HOST) + ":" + SERVER_PORT);
        _listener.Start();
        OutputCallback?.Invoke("Started listening: " + IPAddress.Parse(SERVER_HOST) + ":" + SERVER_PORT + ", waiting for connection");

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
                StatusLabelCallback?.Invoke("Client request download "+fileName);
                int startByte = int.Parse(requestParts[2]);
                int fragmentSize = int.Parse(requestParts[3]);
                Log_Detail("Filename: "+fileName + "StartByte: " + startByte + "fragmentSize: "+ fragmentSize);
                byte[] fileFragment = await ServeFileFragmentAsync(fileName, startByte, fragmentSize);
                Log_Detail("Send to Cache or Client");
                Log_Detail("hexadecimal: "+BitConverter.ToString(fileFragment).Replace("-", ""));
                await writer.WriteLineAsync(BitConverter.ToString(fileFragment).Replace("-", ""));
            }
            else if (request.StartsWith("Vaild_File_Change"))
            {
                string[] requestParts = request.Split(' ');
                string fileName = requestParts[1];
                string File = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "File_Storage", fileName));
                var SingleFileHash = ComputeFileHash(File);
                if (FileHash[fileName] == SingleFileHash)
                {
                    await writer.WriteLineAsync("true");
                }
                else
                {
                    await writer.WriteLineAsync("false");
                }
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
        List<Dictionary<string, List<Tuple<int, int, int, string>>>> listOfDictionaries = new List<Dictionary<string, List<Tuple<int, int, int, string>>>>();
        for (int i = 0; i < files.Length; i++)
        {
            FileHash[Path.GetFileName(files[i])] = ComputeFileHash(files[i]);
            StatusLabelCallback?.Invoke(files[i]);
            var blockHashes = GetFileBlockHashes(files[i], blockSize);

             listOfDictionaries.Add(blockHashes);

        }

        string jsonString = JsonConvert.SerializeObject(listOfDictionaries);

        return jsonString;
    }
    private Dictionary<string, List<Tuple<int, int, int, string>>> GetFileBlockHashes(string filePath, int blockSize)
    {
        var blockHashes = new Dictionary<string, List<Tuple<int, int, int, string>>>();

        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            byte[] buffer = new byte[blockSize];
            int bytesRead;
            int blockIndex = 0;
            int startByte = 0;
            int count = 0;
            bytesRead = fileStream.Read(buffer, 0, blockSize);
            while (bytesRead == blockSize)
            {
                count += 1;

                // Compute the hash for the current block
                string hash = ComputeHash(buffer);

                int endByte = startByte + bytesRead - 1;

                if (!blockHashes.ContainsKey(Path.GetFileName(filePath)))
                {
                    blockHashes[Path.GetFileName(filePath)] = new List<Tuple<int, int, int, string>>();
                }

                // Add the block information to the list associated with the file name
                blockHashes[Path.GetFileName(filePath)].Add(Tuple.Create(blockIndex, startByte, endByte, hash));
                startByte = endByte + 1;
                blockIndex++;
                Log_Detail($"一次循环结束 Iteration {count}: bytesRead={bytesRead}, startByte={startByte}");

                bytesRead = fileStream.Read(buffer, 0, blockSize);
            }

            // Process the last block if it exists
            if (bytesRead > 0)
            {
                Array.Resize(ref buffer, bytesRead);

                // Compute the hash for the current block
                string hash = ComputeHash(buffer);
                int endByte = startByte + bytesRead - 1;

                if (!blockHashes.ContainsKey(Path.GetFileName(filePath)))
                {
                    blockHashes[Path.GetFileName(filePath)] = new List<Tuple<int, int, int, string>>();
                }

                // Add the block information to the list associated with the file name
                blockHashes[Path.GetFileName(filePath)].Add(Tuple.Create(blockIndex, startByte, endByte, hash));
                Log_Detail($"最后一次循环结束 Iteration {count}: bytesRead={bytesRead}, startByte={startByte}");
            }
            Log_Detail(filePath+"循环结束");
        }

        return blockHashes;
    }
    public static void Log(string message)
    {
        string logFilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", "Server_log.txt"));
        Log_Detail(message);
        
        using (StreamWriter sw = new StreamWriter(logFilePath, true))
        {
            sw.WriteLine($"{DateTime.Now}: {message}");
        }
    }
    public static void Log_Detail(string message)
    {
        string logFilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", "Server_detailed_log.txt"));
        
        using (StreamWriter sw = new StreamWriter(logFilePath, true))
        {
            sw.WriteLine($"{DateTime.Now}: {message}");
        }
    }
    private static string ComputeHash(byte[] data)
    {
        //Compute Hash for file blocks.
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(data);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
    private static string ComputeFileHash(string filePath)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = sha256.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
    private async Task<byte[]> ServeFileFragmentAsync(string fileName, int startByte, int fragmentSize)
    {
        // Serve a file fragment from the server
        string filePath = Path.Combine(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "File_Storage")), fileName); 
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