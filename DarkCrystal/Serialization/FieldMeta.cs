
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using DarkCrystal.FieldSystem;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DarkCrystal.Serialization
{
    public abstract class FieldMeta
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private static Dictionary<FieldKey, FieldMeta> Metas;

        public FieldKey Key { get; private set; }
        public Type FieldType { get; private set; }

        public static IEnumerable<FieldKey> Keys => Metas.Keys;

        static FieldMeta()
        {
            Metas = new Dictionary<FieldKey, FieldMeta>();

            foreach (var type in Utils.GetAssemblyTypes())
            {
                var attribute = type.GetCustomAttribute<FieldTypeAttribute>();
                if (attribute != null)
                {
                    foreach (Enum value in type.GetEnumValues())
                    {
                        Register(value.ToFieldKey(), attribute.FieldType);
                    }
                }
            }
        }

        protected FieldMeta()
        {
        }

        public static void Register(FieldKey key, Type type)
        {
            FieldMeta meta;
            if (Metas.TryGetValue(key, out meta))
            {
                if (meta.FieldType == type)
                {
                    return;
                }
                else
                {
                    var format = "Attemp to register {0} field at {1} (already contains {2})";
                    throw new Exception(String.Format(format, type.Name, key, meta.FieldType.Name));
                }
            }
            else
            {
                CreateNew(key, type);
            }
        }

        private static void CreateNew(FieldKey key, Type type)
        {
            var generic = typeof(FieldMeta<>).MakeGenericType(type);
            var fieldMeta = generic.InvokeDefaultConstructor() as FieldMeta;
            fieldMeta.FieldType = type;
            fieldMeta.Key = key;
            Metas[key] = fieldMeta;
        }

        public static FieldMeta Get(FieldKey key)
        {
            FieldMeta meta;
            if (!Metas.TryGetValue(key, out meta))
            {
                throw new Exception(String.Format("Unknown field {0}", key));
            }
            return meta;
        }

        public override string ToString()
        {
            return String.Format("{0} FieldMeta ({1})", FieldType.Name, Key.ToString());
        }

        public abstract int Serialize(object instance, ref byte[] bytes, int offset, IFormatterResolver formatterResolver);
        public abstract object Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize);
    }

    public class FieldMeta<T> : FieldMeta
    {
        public override int Serialize(object instance, ref byte[] bytes, int offset, IFormatterResolver formatterResolver)
        {
            return formatterResolver.GetFormatter<T>().Serialize(ref bytes, offset, (T)instance, formatterResolver);
        }

        public override object Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            return formatterResolver.GetFormatter<T>().Deserialize(bytes, offset, formatterResolver, out readSize);
        }
    }
}