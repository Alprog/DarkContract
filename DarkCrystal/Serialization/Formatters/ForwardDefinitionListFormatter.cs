
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MessagePack;
using MessagePack.Formatters;

namespace DarkCrystal.Serialization
{
    public class ForwardDefinitionListFormatter : IMessagePackFormatter<ForwardDefinitionList>
    {
        public static GuidObjectFormatter<GuidObject> GuidObjectFormatter = new GuidObjectFormatter<GuidObject>();

        public int Serialize(ref byte[] bytes, int offset, ForwardDefinitionList definitionList, IFormatterResolver formatterResolver)
        {
            if (definitionList == null)
            {
                MessagePackBinary.WriteNil(ref bytes, offset);
                return 1;
            }

            var startOffset = offset;
            var itemsCount = definitionList.Count * 2;
            offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, itemsCount);

            var state = Serializer.Instance.State;

            // declarations
            foreach (var instance in definitionList)
            {
                state.RegNewObjectReference(instance);
                offset += GuidObjectFormatter.SerializeBody(ref bytes, offset, instance, formatterResolver, false, out bool dataWritten);
            }
            
            // definitions
            foreach (var instance in definitionList)
            {
                state.ObjectInstances.Add(null);
                var writtenCount = GuidObjectFormatter.SerializeBody(ref bytes, offset, instance, formatterResolver, true, out bool dataWritten);
                if (dataWritten)
                {
                    offset += writtenCount;
                }
                else
                {
                    state.ObjectInstances.RemoveAt(state.ObjectInstances.Count - 1);
                    itemsCount--;
                }
            }

            MessagePackBinary.ReWriteArrayHeaderDownwards(ref bytes, startOffset, itemsCount);

            return offset - startOffset;
        }

        public ForwardDefinitionList Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            if (MessagePackBinary.IsNil(bytes, offset))
            {
                readSize = 1;
                return null; 
            }

            var definitionList = new ForwardDefinitionList();

            var startOffset = offset;
            var elementsCount = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
            offset += readSize;

            // read declarations
            for (int i = 0; i < elementsCount; i++)
            {
                var guidObject = GuidObjectFormatter.Deserialize(bytes, offset, formatterResolver, out readSize);
                definitionList.Add(guidObject);
                offset += readSize;
            }

            readSize = offset - startOffset;

            return definitionList;
        }
    }
}