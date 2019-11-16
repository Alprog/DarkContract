
namespace DarkCrystal.Serialization
{
    public class RuntimeKeyAttribute : KeyAttribute
    {
        public RuntimeKeyAttribute(int value, string distributedSubPath = null):
            base(value, distributedSubPath, SerializationFlags.Runtime)
        {
        }
    }
}