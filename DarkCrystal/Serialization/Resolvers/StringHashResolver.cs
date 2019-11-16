
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using MessagePack;
using MessagePack.Formatters;

namespace DarkCrystal.Serialization
{
    public class StringHashResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new StringHashResolver();

        StringHashResolver()
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
                if (typeof(T) == typeof(StringHash))
                {
                    formatter = new StringHashFormatter() as IMessagePackFormatter<T>;
                }
            }
        }
    }
}