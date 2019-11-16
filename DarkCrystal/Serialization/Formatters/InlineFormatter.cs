
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using MessagePack;
using MessagePack.Formatters;

namespace DarkCrystal.Serialization
{
    public static class InlineFormatter
    {
        private static Func<object, object> CurrentGetter;
        private static object CurrentInstance;

        public static void SetInlineGetter(Func<object, object> getter, object instance)
        {
            CurrentGetter = getter;
            CurrentInstance = instance;
        }

        public static object CallInlineGetter()
        {
            return CurrentGetter(CurrentInstance);
        }
    }

    public class InlineFormatter<T> : IMessagePackFormatter<T>
    {
        public int Serialize(ref byte[] bytes, int offset, T instance, IFormatterResolver formatterResolver)
        {
            if (instance == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            return DarkMeta<T>.Value.WriteMembers(Serializer.Instance.State.Settings.Flags, instance, ref bytes, offset, formatterResolver, false);
        }

        public T Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            object instance = InlineFormatter.CallInlineGetter();

            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
            }
            else
            {
                if (instance == null)
                {
                    instance = Activator.CreateInstance<T>();
                }
                DarkMeta<T>.Value.ReadMembers(Serializer.Instance.State.Settings.Flags, ref instance, bytes, offset, formatterResolver, out readSize, false);
                DarkMeta<T>.Value.OnDeserialized(instance);
            }

            return (T)instance;
        }
    }
}