
using System;

namespace DarkCrystal.Serialization
{
    public class DefaultFieldInfo
    {
        public readonly Type BaseType;
        public readonly FieldKey Key;

        public DefaultFieldInfo(Type baseType, TypeIndex typeIndex)
        {
            this.BaseType = baseType;
            this.Key = new FieldKey(TypeIndex.TypeIndex, (int)typeIndex);
        }
    }
}