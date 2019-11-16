
namespace DarkCrystal.Serialization
{
    public class InlineDarkContractAttribute : DarkContractAttribute
    {
        public InlineDarkContractAttribute(): 
            base(TypeIndex.Invalid, DarkFlags.Inline | DarkFlags.Serializable)
        {
        }
    }
}