
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MessagePack;
using MessagePack.Formatters;
using System;

namespace DarkCrystal.Serialization
{
    public class DateTimeResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new DateTimeResolver();

        DateTimeResolver()
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
                if (typeof(T) == typeof(DateTime))
                {
                    formatter = new DateTimeFormatter() as IMessagePackFormatter<T>;
                }
            }
        }
    }
}