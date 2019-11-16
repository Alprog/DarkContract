
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using MessagePack;
using MessagePack.Formatters;

namespace DarkCrystal.Serialization
{
    public class StructFormatter<T> : IMessagePackFormatter<T> where T : struct
    {
        public int Serialize(ref byte[] bytes, int offset, T instance, IFormatterResolver formatterResolver)
        {
            return DarkMeta<T>.Value.WriteMembers(Serializer.Instance.State.Settings.Flags, instance, ref bytes, offset, formatterResolver, false);
        }

        public T Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            object instance = Activator.CreateInstance<T>();
            DarkMeta<T>.Value.ReadMembers(Serializer.Instance.State.Settings.Flags, ref instance, bytes, offset, formatterResolver, out readSize, false);
            return (T)instance;
        }
    }
}