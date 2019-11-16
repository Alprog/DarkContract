
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using MessagePack;

namespace DarkCrystal.Serialization
{
    public partial class DarkMeta
    {
        public class DistributedMemberFormatter<T> : MemberFormatter
        {
            private string SubPath;

            public override Type DataType => typeof(T);

            public DistributedMemberFormatter(FieldInfo fieldInfo, SerializationFlags flags, string subPath)
            {
                this.Flags = flags;
                this.Getter = fieldInfo.GetValue;
                this.Setter = fieldInfo.SetValue;
                this.MemberName = fieldInfo.Name;
                this.SubPath = subPath;
            }

            public DistributedMemberFormatter(PropertyInfo propertyInfo, SerializationFlags flags, string subPath)
            {
                this.Flags = flags;
                this.Getter = propertyInfo.GetValue;
                this.Setter = propertyInfo.SetValue;
                this.MemberName = propertyInfo.Name;
                this.SubPath = subPath;
            }

            private SerializationSettings GetSubSettings()
            {
                var settings = Serializer.Instance.State.Settings;
                settings.DistributedFolder = Utils.PathCombine(settings.DistributedFolder, SubPath);
                settings.InternalObjects = null;
                return settings;
            }

            public override int Write(object instance, ref byte[] bytes, int offset, IFormatterResolver formatterResolver)
            {
                var value = (T)Getter(instance);
                var settings = GetSubSettings();
                var filePath = settings.DistributedFolder + ".sav";
                Serializer.Instance.SerializeToFile(value, filePath, settings);
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }

            public override void Read(ref object instance, byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
            {
                InlineFormatter.SetInlineGetter(Getter, instance);

                var settings = GetSubSettings();
                var filePath = settings.DistributedFolder + ".sav";
                T value = Serializer.Instance.DeserializeFromFile<T>(filePath, settings);
                Setter(instance, value);
                MessagePackBinary.ReadNil(bytes, offset, out readSize);
            }
        }
    }
}