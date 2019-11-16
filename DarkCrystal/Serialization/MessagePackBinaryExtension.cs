
using MessagePack;
using System.Collections.Generic;
using System.Text;

namespace DarkCrystal.Serialization
{
    public static class MessagePackBinaryExtension
    {
        public const int CommentTypeCode = 7;

        public static int WriteComment(ref byte[] bytes, int offset, string comment)
        {
            var commentBytes = Encoding.ASCII.GetBytes(comment);
            return MessagePackBinary.WriteExtensionFormat(ref bytes, offset, CommentTypeCode, commentBytes);
        }

        public static string ReadComment(byte[] bytes, int offset, out int readSize)
        {
            var result = MessagePackBinary.ReadExtensionFormat(bytes, offset, out readSize);
            return Encoding.ASCII.GetString(result.Data);
        }
    }
}