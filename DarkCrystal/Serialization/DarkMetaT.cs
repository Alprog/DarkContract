
namespace DarkCrystal.Serialization
{
    // effective way to get darkmeta
    public static class DarkMeta<T>
    {
        public readonly static DarkMeta Value = null;

        static DarkMeta()
        {
            Value = DarkMeta.Get(typeof(T));
        }
    }
}