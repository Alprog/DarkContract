
using MessagePack;
using MessagePack.Formatters;

namespace DarkCrystal.Serialization
{
    public class FieldListResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new FieldListResolver();

        FieldListResolver()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                if (typeof(T) == typeof(FieldList))
                {
                    formatter = new FieldListFormatter() as IMessagePackFormatter<T>;
                }
            }
        }
    }
}