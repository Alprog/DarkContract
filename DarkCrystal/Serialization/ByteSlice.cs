
using DarkCrystal.Encased;
using System;

namespace DarkCrystal.Serialization
{
    public struct ByteSlice
    {
        public int Offset;
        public int Length;

        public byte[] GetBytes() => The.GameState.OriginalBytes;

        public ByteSlice(int offset, int length)
        {
            this.Offset = offset;
            this.Length = length;
        }

        public bool Equals(byte[] bytes, int offset, int length)
        {
            if (Length != length)
            {
                return false;
            }
            else
            {
                var originalBytes = GetBytes();
                for (int i = 0; i < length; i++)
                {
                    if (originalBytes[Offset + i] != bytes[offset + i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public bool BeginingEquals(byte[] bytes, int offset, int length)
        {
            if (Length < length)
            {
                return false;
            }
            var originalBytes = GetBytes();
            for (int i = 0; i < length; i++)
            {
                if (originalBytes[Offset + i] != bytes[offset + i])
                {
                    return false;
                }
            }
            return true;
        }

        public ArraySegment<byte> ToArraySegment()
        {
            return new ArraySegment<byte>(GetBytes(), Offset, Length);
        }
    }
}