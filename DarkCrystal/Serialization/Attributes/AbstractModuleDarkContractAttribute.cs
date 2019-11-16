
namespace DarkCrystal.Serialization
{
    public class AbstractModuleDarkContractAttribute : DarkContractAttribute
    {
        public AbstractModuleDarkContractAttribute(TypeIndex typeIndex, bool isSerializable = false):
            base(typeIndex, GetFlags(isSerializable))
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