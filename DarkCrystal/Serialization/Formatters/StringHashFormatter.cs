
using MessagePack;
using MessagePack.Formatters;

namespace DarkCrystal.Serialization
{
    public class StringHashFormatter : IMessagePackFormatter<StringHash>
    {
        public int Serialize(ref byte[] bytes, int offset, StringHash hash, IFormatterResolver formatterResolver)
        {
            return MessagePackBinary.WriteInt32(ref bytes, offset, hash.Value);
        }

        public StringHash Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            var intValue = MessagePackBinary.ReadInt32(bytes, offset, out readSize);
            return new StringHash(intValue);
        }
    }
}