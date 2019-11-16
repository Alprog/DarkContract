using System;

namespace DarkCrystal.Serialization
{
    public class LastKeyAttribute : Attribute
    {
        public int Value;

        public LastKeyAttribute(int value = 0)
        {
            this.Value = value;
        }
    }
}
