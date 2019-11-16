
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MessagePack;
using MessagePack.Formatters;
using System.Collections.Generic;

namespace DarkCrystal.Serialization
{
    public class FieldListFormatter : IMessagePackFormatter<FieldList>
    {
        public static int LastFieldWritten = 0;

        public int Serialize(ref byte[] bytes, int offset, FieldList list, IFormatterResolver formatterResolver)
        {
            if (list == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            var startOffset = offset;
            offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, (list.Count + 1) * 3); // plus one in case of DeletedKeys

            var state = Serializer.Instance.State;
            var previousOffset = state.LocalOffset;

            var comparer = new FieldComparer(list.OriginalBytes);

            var itemCount = 0;
            foreach (var pair in list)
            {
                bool isValueProducer = pair.Value != null && pair.Value is ValueProducer;
                if (isValueProducer && pair.Value == LazyProducer.Instance)
                {
                    continue;
                }

                state.StartLocalGroup();                
                var groupStartOffset = offset;

                var typeId = (int)pair.Key.TypeId;
                offset += MessagePackBinary.WriteInt32(ref bytes, offset, isValueProducer ? -typeId : typeId);
                offset += MessagePackBinary.WriteInt32(ref bytes, offset, pair.Key.MemberId);

                var bodyLength = 0;
                if (isValueProducer)
                {
                    bodyLength = formatterResolver.GetFormatter<ValueProducer>().Serialize(ref bytes, offset, (ValueProducer)pair.Value, formatterResolver);
                }
                else
                {
                    bodyLength = FieldMeta.Get(pair.Key).Serialize(pair.Value, ref bytes, offset, formatterResolver);
                }

                if (state.WriteOnlyChanged && !comparer.IsChanged(pair.Key, bytes, offset, bodyLength))
                {
                    state.UnrollLocalGroup();
                    offset = groupStartOffset;
                }
                else
                {
                    offset += bodyLength;
                    itemCount++;
                }
            }

            if (state.WriteOnlyChanged)
            {
                comparer.RollToTheEnd();
                if (comparer.DeletedKeys != null)
                {
                    offset += MessagePackBinary.WriteInt32(ref bytes, offset, (int)TypeIndex.SystemField);
                    offset += MessagePackBinary.WriteInt32(ref bytes, offset, (int)SystemField.DeletedKeys);
                    offset += formatterResolver.GetFormatter<List<FieldKey>>().Serialize(ref bytes, offset, comparer.DeletedKeys, formatterResolver);
                    itemCount++;
                }
            }

            LastFieldWritten = itemCount;
            MessagePackBinary.ReWriteArrayHeaderDownwards(ref bytes, startOffset, itemCount * 3); // write real count

            state.RestoreLocalGroup(previousOffset);

            return offset - startOffset;
        }
        
        FieldList IMessagePackFormatter<FieldList>.Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null;
            }

            var startOffset = offset;
            var elementsCount = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize) / 3;
            offset += readSize;

            var list = new FieldList(elementsCount);

            var state = Serializer.Instance.State;
            if (state.Settings.Flags.HasFlag(SerializationFlags.KeepOriginalBytes))
            {
                list.OriginalBytes = new List<FieldBytes>();
            }

            var previousOffset = state.LocalOffset;
            for (int i = 0; i < elementsCount; i++)
            {
                state.StartLocalGroup();

                var typeId = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                offset += readSize;

                var memberId = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
                offset += readSize;
                
                if (typeId >= 0)
                {
                    // normal
                    var fieldKey = new FieldKey((TypeIndex)typeId, memberId);
                    list[fieldKey] = FieldMeta.Get(fieldKey).Deserialize(bytes, offset, formatterResolver, out readSize);
                    list.OriginalBytes?.Add(new FieldBytes(fieldKey, offset, readSize));
                    offset += readSize;
                }
                else
                {
                    // ValueProducer
                    var fieldKey = new FieldKey((TypeIndex)(-typeId), memberId);
                    list[fieldKey] = formatterResolver.GetFormatter<ValueProducer>().Deserialize(bytes, offset, formatterResolver, out readSize);
                    list.OriginalBytes?.Add(new FieldBytes(fieldKey, offset, readSize));
                    offset += readSize;
                }
            }
            state.RestoreLocalGroup(previousOffset);

            readSize = offset - startOffset;

            return list;
        }
    }
}