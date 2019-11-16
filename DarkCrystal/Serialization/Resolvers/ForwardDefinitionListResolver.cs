
using MessagePack;
using MessagePack.Formatters;

namespace DarkCrystal.Serialization
{
    public class ForwardDefinitionListResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new ForwardDefinitionListResolver();

        ForwardDefinitionListResolver()
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
                if (typeof(T) == typeof(ForwardDefinitionList))
                {
                    formatter = new ForwardDefinitionListFormatter() as IMessagePackFormatter<T>;
                }
            }
        }
    }
}