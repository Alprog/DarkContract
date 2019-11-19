
using System;

namespace DarkCrystal.FieldSystem
{
    public class FieldTypeAttribute : Attribute
    {
        public readonly Type FieldType;

        public FieldTypeAttribute(Type type)
        {
            this.FieldType = type;
        }
    }
}