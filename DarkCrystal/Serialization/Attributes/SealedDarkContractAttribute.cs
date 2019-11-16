
namespace DarkCrystal.Serialization
{
    public class SealedDarkContractAttribute : DarkContractAttribute
    {
        public SealedDarkContractAttribute(TypeIndex typeIndex):
            base(typeIndex, DarkFlags.Sealed | DarkFlags.Serializable)
        {
        }
    }
}