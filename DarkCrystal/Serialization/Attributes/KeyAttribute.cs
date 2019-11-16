
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace DarkCrystal.Serialization
{
    public class KeyAttribute : Attribute
    {
        public int Value;
        public string DistributedSubPath;
        public SerializationFlags Flags;

        public KeyAttribute(int value, SerializationFlags flags = SerializationFlags.Default)
        {
            this.Value = value;
            this.DistributedSubPath = null;
            this.Flags = flags;
        }

        public KeyAttribute(int value, string distributedSubPath, SerializationFlags flags = SerializationFlags.Default)
        {
            this.Value = value;
            this.DistributedSubPath = distributedSubPath;
            this.Flags = flags;
        }
    }
}