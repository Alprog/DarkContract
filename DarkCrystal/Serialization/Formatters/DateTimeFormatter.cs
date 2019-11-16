
using System;
using MessagePack;
using MessagePack.Formatters;

namespace DarkCrystal.Serialization
{
    public class DateTimeFormatter : IMessagePackFormatter<DateTime>
    {
        public int Serialize(ref byte[] bytes, int offset, DateTime dateTime, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteInt64(ref bytes, offset, dateTime.Ticks);
        }

        public DateTime Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            var ticks = MessagePackBinary.ReadInt64(bytes, offset, out readSize);
            return new DateTime(ticks);
        }
    }
}