namespace Cache
{
    public class FileFragment
    {
        public int StartByte { get; set; }
        public byte[] Data { get; set; }

        public int Size => Data.Length;
    }
}