
namespace DarkCrystal.Serialization
{
    public class StaticKeyAttribute : KeyAttribute
    {
        public StaticKeyAttribute(int value, string distributedSubPath = null):
            base(value, distributedSubPath, SerializationFlags.Static)
        {
        }
    }
}