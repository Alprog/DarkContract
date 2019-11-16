
using System.Reflection;
using MessagePack;
using MessagePack.Formatters;

namespace DarkCrystal.Serialization
{
    public class SingletonFormatter<T> : IMessagePackFormatter<T> where T : Singleton<T>
    {
        private const BindingFlags Flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;

        public int Serialize(ref byte[] bytes, int offset, T instance, IFormatterResolver formatterResolver)
        {
            return DarkMeta<T>.Value.WriteMembers(Serializer.Instance.State.Settings.Flags, instance, ref bytes, offset, formatterResolver, false);
        }

        public T Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            var field = typeof(T).GetField("Instance", Flags);
            object instance = field.GetValue(null);
            DarkMeta<T>.Value.ReadMembers(Serializer.Instance.State.Settings.Flags, ref instance, bytes, offset, formatterResolver, out readSize, false);
            return (T)instance;
        }
    }
}