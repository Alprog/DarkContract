
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MessagePack;
using System;
using System.Reflection;

namespace DarkCrystal.Serialization
{
    public partial class DarkMeta
    {
        public abstract class MemberFormatter
        {
            public string MemberName { get; protected set; }

            public Func<object, object> Getter { get; protected set; }
            public Action<object, object> Setter { get; protected set; }

            public SerializationFlags Flags { get; protected set; }

            public static MemberFormatter Create(MemberInfo memberInfo, SerializationFlags flags)
            {
                var dataType = GetDataType(memberInfo);
                var memberFormatter = typeof(MemberFormatter<>).MakeGenericType(dataType);
                var ctor = memberFormatter.GetConstructor(new Type[] { memberInfo.GetType(), typeof(SerializationFlags) });
                return ctor.Invoke(new object[] { memberInfo, flags }) as MemberFormatter;
            }

            public static MemberFormatter CreateDistributed(MemberInfo memberInfo, SerializationFlags flags, string subPath)
            {
                var dataType = GetDataType(memberInfo);
                var memberFormatter = typeof(DistributedMemberFormatter<>).MakeGenericType(dataType);
                var ctor = memberFormatter.GetConstructor(new Type[] { memberInfo.GetType(), typeof(SerializationFlags), typeof(string) });
                return ctor.Invoke(new object[] { memberInfo, flags, subPath }) as MemberFormatter;
            }

            private static Type GetDataType(MemberInfo info)
            {
                var fileInfo = info as FieldInfo;
                if (fileInfo != null)
                {
                    return fileInfo.FieldType;
                }
                else
                {
                    return (info as PropertyInfo).PropertyType;
                }
            }

            public abstract Type DataType { get; }

            public abstract int Write(object instance, ref byte[] bytes, int offset, IFormatterResolver formatterResolver);
            public abstract void Read(ref object instance, byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize);
        }

        public class MemberFormatter<T> : MemberFormatter
        {
            public override Type DataType => typeof(T);

            public MemberFormatter(FieldInfo fieldInfo, SerializationFlags flags)
            {
                this.Flags = flags;
                this.MemberName = fieldInfo.Name;
                this.Getter = fieldInfo.GetValue;
                this.Setter = fieldInfo.SetValue;
            }

            public MemberFormatter(PropertyInfo propertyInfo, SerializationFlags flags)
            {
                this.Flags = flags;
                MemberName = propertyInfo.Name;
                Getter = propertyInfo.GetValue;
                Setter = propertyInfo.SetValue;
            }

            public override int Write(object instance, ref byte[] bytes, int offset, IFormatterResolver formatterResolver)
            {
                var value = (T)Getter(instance);
                try
                {
                    return formatterResolver.GetFormatter<T>().Serialize(ref bytes, offset, value, formatterResolver);
                }
                catch (Exception exception)
                {
                    var formatText = "Exception on write member\nType: {0}\nInstance:{1}";
                    Logger.Instance.Print(String.Format(formatText, typeof(T).Name, instance?.ToString() ?? "NULL"));
                    throw exception;
                }
            }

            public override void Read(ref object instance, byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
            {
                InlineFormatter.SetInlineGetter(Getter, instance);

                T value = formatterResolver.GetFormatter<T>().Deserialize(bytes, offset, formatterResolver, out readSize);
                Setter(instance, value);
            }
        }
    }
}