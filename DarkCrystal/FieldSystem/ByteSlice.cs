
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace DarkCrystal.FieldSystem
{
    public struct ByteSlice
    {
        public static byte[] SharedBytes;

        public int Offset;
        public int Length;

        public byte[] GetBytes() => SharedBytes;

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