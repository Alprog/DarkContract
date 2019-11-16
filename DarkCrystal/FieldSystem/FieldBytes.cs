
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace DarkCrystal.FieldSystem
{
    public struct FieldBytes : IComparable<FieldBytes>
    {
        public FieldKey Key;
        public ByteSlice ByteSlice;

        public FieldBytes(FieldKey key, ByteSlice byteSlice)
        {
            this.Key = key;
            this.ByteSlice = byteSlice;
        }

        public FieldBytes(FieldKey key, int offset, int length)
        {
            this.Key = key;
            this.ByteSlice = new ByteSlice(offset, length);
        }

        public int CompareTo(FieldBytes other)
        {
            return Key.CompareTo(other.Key);
        }
    }
}