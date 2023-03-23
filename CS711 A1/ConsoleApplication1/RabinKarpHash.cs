namespace ConsoleApplication1
{
    public class RabinKarpHash
    {
        private const ulong PrimeBase = 256;
        private const ulong PrimeMod = 16777619;
        private ulong _hash;
        private ulong _highestBase;
        private int _count;

        public RabinKarpHash(int size)
        {
            _highestBase = 1;
            for (int i = 0; i < size - 1; i++)
            {
                _highestBase = (_highestBase * PrimeBase) % PrimeMod;
            }
        }

        public void Add(byte b)
        {
            _hash = (_hash * PrimeBase + b) % PrimeMod;
            _count++;
        }

        public void Remove(byte b)
        {
            _hash = (_hash + PrimeMod - (_highestBase * b) % PrimeMod) % PrimeMod;
            _count--;
        }

        public ulong Hash => _hash;
        public int Count => _count;
    }
}