
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using MessagePack.Unity;

namespace DarkCrystal.Serialization
{
    public class MainResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new MainResolver();

        static readonly IFormatterResolver[] resolvers = new[]
        {
            GuidResolver.Instance,
            LocalizableTextResolver.Instance,
            FieldListResolver.Instance,
            DateTimeResolver.Instance,
            StringHashResolver.Instance,
            BuiltinResolver.Instance,
            AttributeResolver.Instance,
            ForwardDefinitionListResolver.Instance,
            DarkResolver.Instance,
            UnityResolver.Instance,
            DynamicEnumResolver.Instance,
            DynamicGenericResolver.Instance,
            DynamicUnionResolver.Instance,
            DynamicObjectResolver.Instance,
            PrimitiveObjectResolver.Instance
        };

        MainResolver()
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
                foreach (var item in resolvers)
                {
                    var f = item.GetFormatter<T>();
                    if (f != null)
                    {
                        formatter = f;
                        return;
                    }
                }
            }
        }
    }
}