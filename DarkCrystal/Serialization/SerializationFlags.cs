
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace DarkCrystal.Serialization
{
    public enum SerializationFlags
    {
        Default = 0,

        OnePackage = 1 << 0,
        Distributed = 1 << 1,

        Static = 1 << 2,
        Runtime = 1 << 3,

        Binary = 1 << 4,
        Text = 1 << 5,

        Save = 1 << 8,
        Load = 1 << 9,

        Annotations = 1 << 10,

        KeepOriginalBytes = 1 << 11,
        WriteOnlyChanged = 1 << 12,
    }

    public static class SerializationMode
    {
        private const SerializationFlags StaticPackage = SerializationFlags.Static | SerializationFlags.OnePackage | SerializationFlags.Binary;
        private const SerializationFlags StaticFolder = SerializationFlags.Static | SerializationFlags.Distributed | SerializationFlags.Text | SerializationFlags.Annotations;
        private const SerializationFlags RuntimePackage = SerializationFlags.Runtime | SerializationFlags.OnePackage | SerializationFlags.Binary;

        public const SerializationFlags SaveStaticPackage = StaticPackage | SerializationFlags.Save;
        public const SerializationFlags LoadStaticPackage = StaticPackage | SerializationFlags.Load | SerializationFlags.KeepOriginalBytes;
        public const SerializationFlags SaveStaticFolder = StaticFolder | SerializationFlags.Save;
        public const SerializationFlags LoadStaticFolder = StaticFolder | SerializationFlags.Load;
        public const SerializationFlags SaveRuntimePackage = RuntimePackage | SerializationFlags.Save | SerializationFlags.WriteOnlyChanged;
        public const SerializationFlags LoadRuntimePackage = RuntimePackage | SerializationFlags.Load; 

        public const SerializationFlags Locale = SerializationFlags.OnePackage | SerializationFlags.Binary;
    }
}