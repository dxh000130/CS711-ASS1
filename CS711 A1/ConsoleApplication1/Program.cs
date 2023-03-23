using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApplication1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            
            
            int blockSize = 2048;
            int minBlockSize = 1024;
            int maxBlockSize = 4096;
            ulong splitMarker = 97; // 这是一个示例分割标记，你可以根据需要调整
            string file1Path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "File_Storage", "test3.bmp"));
            string file2Path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "File_Storage", "test1.bmp"));

            List<Tuple<ulong, int>> file1HashesAndSizes = ComputeFileBlockHashes(file1Path, blockSize, minBlockSize, maxBlockSize, splitMarker);
            List<Tuple<ulong, int>> file2HashesAndSizes = ComputeFileBlockHashes(file2Path, blockSize, minBlockSize, maxBlockSize, splitMarker);

            List<int> blocksToRedownload = GetBlocksToRedownload(file1HashesAndSizes.Select(x => x.Item1).ToList(), file2HashesAndSizes.Select(x => x.Item1).ToList());

            Console.WriteLine("需要重新下载的文件块索引：");
            Console.WriteLine(blocksToRedownload.Count);
            // foreach (int index in blocksToRedownload)
            // {
            //     Console.WriteLine(index);
            // }
        }
        public static List<int> GetBlocksToRedownload(List<ulong> hashes1, List<ulong> hashes2)
        {
            List<int> blocksToRedownload = new List<int>();
            int count = 0;
            int minLength = Math.Min(hashes1.Count, hashes2.Count);

            for (int i = 0; i < minLength; i++)
            {
                if (hashes1[i] != hashes2[i])
                {
                    blocksToRedownload.Add(i);
                }

                count++;
            }

            for (int i = minLength; i < hashes2.Count; i++)
            {
                blocksToRedownload.Add(i);
                count++;
            }
            Console.WriteLine(count);
            return blocksToRedownload;
        }
        public static List<Tuple<ulong, int>> ComputeFileBlockHashes(string filePath, int blockSize, int minBlockSize, int maxBlockSize, ulong splitMarker)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            List<Tuple<ulong, int>> blockHashesAndSizes = new List<Tuple<ulong, int>>();

            RabinKarpHash hasher = new RabinKarpHash(blockSize);

            int blockStart = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                hasher.Add(bytes[i]);

                if (i >= minBlockSize - 1 && (i - blockStart + 1) >= minBlockSize)
                {
                    if (hasher.Hash % splitMarker == 0 || (i - blockStart + 1) >= maxBlockSize)
                    {
                        blockHashesAndSizes.Add(Tuple.Create(hasher.Hash, i - blockStart + 1));

                        hasher = new RabinKarpHash(blockSize);
                        blockStart = i + 1;
                    }
                }
            }

            if (hasher.Count > 0)
            {
                blockHashesAndSizes.Add(Tuple.Create(hasher.Hash, hasher.Count));
            }

            return blockHashesAndSizes;
        }
    }
    
}