
using System;

namespace DarkCrystal.Serialization
{
    public class DarkContractAttribute : Attribute
    {
        public readonly TypeIndex TypeIndex;
        public readonly DarkFlags Flags;

        public DarkContractAttribute(TypeIndex typeIndex, DarkFlags flags = DarkFlags.Serializable)
        {
            this.TypeIndex = typeIndex;
            this.Flags = flags;
        }
    }
}