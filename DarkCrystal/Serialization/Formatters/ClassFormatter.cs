
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using DarkCrystal.Encased.Core.ModuleSystem;
using MessagePack;
using MessagePack.Formatters;
using System;

namespace DarkCrystal.Serialization
{
    public class ClassFormatter<T> : IMessagePackFormatter<T> where T : class
    {
        public int Serialize(ref byte[] bytes, int offset, T instance, IFormatterResolver formatterResolver)
        {
            if (instance == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var startOffset = offset;

            if (Serializer.Instance.State.RegObjectReference(instance, out int reference))
            {
                // full serialization
                offset += SerializeBody(ref bytes, offset, instance, formatterResolver);
            }
            else
            {
                // next time (reference):
                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, 2);
                offset += MessagePackBinary.WriteNil(ref bytes, offset);
                offset += MessagePackBinary.WriteInt32(ref bytes, offset, reference);
            }

            return offset - startOffset;
        }

        public T Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }

            var startOffset = offset;
            var elementsCount = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
            offset += readSize;

            object instance;
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                // reference:
                MessagePackBinary.ReadNil(bytes, offset, out readSize);
                offset += readSize;
                var reference = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                offset += readSize;

                instance = Serializer.Instance.State.GetInstance(reference);
            }
            else
            {
                // full deserialization:
                var meta = DarkMeta<T>.Value;
                if (!meta.Flags.IsStatic())
                {
                    var typeIndex = (TypeIndex)MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                    offset += readSize;
                    if (typeIndex != meta.TypeIndex)
                    {
                        meta = DarkMeta.Get(typeIndex);
                        if (meta == null)
                        {
                            Logger.Logger.Instance.Warning(String.Format("Can't find typeIndex {0}", typeIndex));
                            for (int i = 0; i < elementsCount - 1; i++)
                            {
                                offset += MessagePackBinary.ReadNextBlock(bytes, offset);
                            }
                            readSize = offset - startOffset;
                            return null;
                        }
                    }
                }
                
                instance = DeserializeBody(meta.Type, elementsCount - 1, bytes, offset, formatterResolver, out readSize);

                offset += readSize;
                meta.OnDeserialized(instance);
            }

            readSize = offset - startOffset;

            return instance as T;
        }

        protected virtual int SerializeBody(ref byte[] bytes, int offset, T instance, IFormatterResolver formatterResolver)
        {
            // [typeIndex, [members], [members], [members]]
            var startOffset = offset;

            var meta = DarkMeta.Get(instance.GetType());
            if (meta.Flags.IsStatic())
            {
                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, meta.DeepLevel);
            }
            else
            {
                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, meta.DeepLevel + 1);
                offset += MessagePackBinary.WriteInt32(ref bytes, offset, (int)meta.TypeIndex);

                if (meta.TypeIndex == TypeIndex.Invalid)
                {
                    var message = String.Format("Type index is invalid at {0} ", instance.ToString());
                    Logger.Logger.Instance.Error(message);
                }
            }
            offset += meta.WriteMembers(Serializer.Instance.State.Settings.Flags, instance, ref bytes, offset, formatterResolver);
            return offset - startOffset;
        }

        protected virtual T DeserializeBody(Type type, int elementsCount, byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            // ...[members], [members], [members]...
            var meta = DarkMeta.Get(type);
            var instance = meta.CreateInstance();
            Serializer.Instance.State.ObjectInstances.Add(instance);
            meta.ReadMembers(Serializer.Instance.State.Settings.Flags, ref instance, bytes, offset, formatterResolver, out readSize);
            return (T)instance;
        }
    }
}