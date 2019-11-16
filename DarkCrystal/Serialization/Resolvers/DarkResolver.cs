
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using MessagePack;
using MessagePack.Formatters;

namespace DarkCrystal.Serialization
{
    public class DarkResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new DarkResolver();

        DarkResolver()
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
                var type = typeof(T);

                if (type.IsEnum)
                {
                    return;
                }

                bool nullable = false;
                var attribute = type.GetCustomAttribute<DarkContractAttribute>(false);
                if (attribute == null)
                {
                    type = Nullable.GetUnderlyingType(type);
                    if (type != null)
                    {
                        nullable = true;
                        attribute = type.GetCustomAttribute<DarkContractAttribute>(false);
                    }

                    if (attribute == null)
                    {
                        return;
                    }
                }

                Type formatterType;
                if (type.IsValueType)
                {
                    if (nullable)
                    {
                        formatterType = typeof(NullableFormatter<>);
                    }
                    else
                    {
                        formatterType = typeof(StructFormatter<>);
                    }
                }
                else
                {
                    if (attribute.Flags.IsInline())
                    {
                        formatterType = typeof(InlineFormatter<>);
                    }
                    else if (type == typeof(GuidObject) || type.IsSubclassOf(typeof(GuidObject)))
                    {
                        formatterType = typeof(GuidObjectFormatter<>);
                    }
                    else if (type.IsSubclassOf(typeof(ISingleton)))
                    {
                        formatterType = typeof(SingletonFormatter<>);
                    }
                    else
                    {
                        formatterType = typeof(ClassFormatter<>);
                    }
                }

                formatter = formatterType.MakeGenericType(type).InvokeDefaultConstructor() as IMessagePackFormatter<T>;
            }
        }
    }
}