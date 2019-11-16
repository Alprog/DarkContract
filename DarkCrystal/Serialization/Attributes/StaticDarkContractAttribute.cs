
namespace DarkCrystal.Serialization
{
    public class StaticDarkContractAttribute : DarkContractAttribute
    {
        public StaticDarkContractAttribute(): 
            base(TypeIndex.Invalid, DarkFlags.Static | DarkFlags.Serializable)
        {
        }
    }
}