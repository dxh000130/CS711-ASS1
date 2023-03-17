using System;
using System.Collections.Generic;
using System.IO;


namespace Cache
{
    public class FileCache
    {
        private Dictionary<string, List<FileFragment>> _cache;

        public FileCache()
        {
            _cache = new Dictionary<string, List<FileFragment>>();
        }

        public byte[] GetFileFragment(string fileName, int startByte, int fragmentSize)
        {
            if (!_cache.ContainsKey(fileName))
            {
                return null;
            }

            foreach (var fragment in _cache[fileName])
            {
                if (fragment.StartByte == startByte && fragment.Size == fragmentSize)
                {
                    return fragment.Data;
                }
            }

            return null;
        }

        public void AddFileFragment(string fileName, int startByte, byte[] data)
        {
            if (!_cache.ContainsKey(fileName))
            {
                _cache[fileName] = new List<FileFragment>();
            }

            var fragment = new FileFragment
            {
                StartByte = startByte,
                Data = data
            };

            _cache[fileName].Add(fragment);
        }

        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}