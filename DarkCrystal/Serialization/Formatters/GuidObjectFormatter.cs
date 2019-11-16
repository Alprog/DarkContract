
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MessagePack;
using System;

namespace DarkCrystal.Serialization
{
    public class GuidObjectFormatter<T> : ClassFormatter<T> where T : GuidObject
    {
        protected override int SerializeBody(ref byte[] bytes, int offset, T instance, IFormatterResolver formatterResolver)
        {
            bool fullSerialization = Serializer.Instance.IsInternal(instance);
            return SerializeBody(ref bytes, offset, instance, formatterResolver, fullSerialization, out bool dataWritten);
        }

        public int SerializeBody(ref byte[] bytes, int offset, T instance, IFormatterResolver formatterResolver, bool fullSerialization, out bool dataWritten)
        {
            var startOffset = offset;

            var meta = DarkMeta.Get(instance.GetType());

            var count = 2;
            if (fullSerialization)
            {
                count += meta.DeepLevel;
                if (!meta.Flags.IsSerializable())
                {
                    count--;
                }
            }
            offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, count);
            offset += MessagePackBinary.WriteInt32(ref bytes, offset, (int)meta.TypeIndex);
            offset += formatterResolver.GetFormatter<Guid>().Serialize(ref bytes, offset, instance.Guid, formatterResolver);

            dataWritten = false;

            if (fullSerialization)
            {
                var state = Serializer.Instance.State;
                state.StartLocalGroup(out int previousOffset);
                var writtenSize = meta.WriteMembers(state.Settings.Flags, instance, ref bytes, offset, formatterResolver);

                bool skipData = false;
                if (state.WriteOnlyChanged)
                {
                    if (meta.Type == typeof(Entity) && FieldListFormatter.LastFieldWritten == 0)
                    {
                        var isArray16 = bytes[offset + writtenSize - 2] == MessagePackCode.Array16;
                        var length = isArray16 ? writtenSize - 3 : writtenSize - 1;
                        skipData = instance.OriginalBytes.BeginingEquals(bytes, offset, length);
                    }
                    else
                    {
                        skipData = instance.OriginalBytes.Equals(bytes, offset, writtenSize);
                    }
                }

                if (skipData)
                {
                    state.UnrollLocalGroup();
                    MessagePackBinary.ReWriteArrayHeaderDownwards(ref bytes, startOffset, count - 1);
                }
                else
                {
                    offset += writtenSize;
                    dataWritten = writtenSize > 1;
                }

                state.RestoreLocalGroup(previousOffset);
            }

            return offset - startOffset;
        }

        protected override T DeserializeBody(Type type, int elementsCount, byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            var startOffset = offset;
            
            var guid = formatterResolver.GetFormatter<Guid>().Deserialize(bytes, offset, formatterResolver, out readSize);
            offset += readSize;

            DarkMeta meta = null;

            object instance = GuidStorage.Get(guid);
            if (instance == null)
            {
                meta = DarkMeta.Get(type);
                instance = meta.CreateInstance(guid);
            }
            Serializer.Instance.State.ObjectInstances.Add(instance);

            if (elementsCount > 1)
            {
                // full deserialization (only in one file)
                var state = Serializer.Instance.State;
                state.StartLocalGroup(out int previousOffset);

                meta = meta ?? DarkMeta.Get(type);
                meta.ReadMembers(state.Settings.Flags, ref instance, bytes, offset, formatterResolver, out readSize);
                (instance as GuidObject).OriginalBytes = new ByteSlice(offset, readSize);

                offset += readSize;

                state.RestoreLocalGroup(previousOffset);
            }

            readSize = offset - startOffset;

            return instance as T;
        }
    }
}