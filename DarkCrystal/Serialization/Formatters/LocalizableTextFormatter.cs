
using System;
using DarkCrystal.Localization;
using MessagePack;
using MessagePack.Formatters;

namespace DarkCrystal.Serialization
{
    public class LocalizableTextFormatter : IMessagePackFormatter<LocalizableText>
    {
        public int Serialize(ref byte[] bytes, int offset, LocalizableText value, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteString(ref bytes, offset, value?.InlineForm ?? String.Empty);
        }

        LocalizableText IMessagePackFormatter<LocalizableText>.Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            var @string = MessagePackBinary.ReadString(bytes, offset, out readSize);
            return new LocalizableText(@string);
        }
    }
}