
namespace DarkCrystal.Serialization
{
    // efficient way to get typeindex
    public static class TypeIndex<T>
    {
        public readonly static TypeIndex Value = TypeIndex.Invalid;

        static TypeIndex()
        {
            Value = TypeRegistry.GetIndex(typeof(T));
        }
    }
}