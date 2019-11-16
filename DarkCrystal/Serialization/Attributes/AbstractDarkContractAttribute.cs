
namespace DarkCrystal.Serialization
{
    public class AbstractDarkContractAttribute : DarkContractAttribute
    {
        public AbstractDarkContractAttribute(bool isSerializable = false):
            base(TypeIndex.Invalid, GetFlags(isSerializable))
        {            
        }

        private static DarkFlags GetFlags(bool isSerializable)
        {
            var flags = DarkFlags.Abstract;
            if (isSerializable)
            {
                flags |= DarkFlags.Serializable;
            }
            return flags;
        }
    }
}