
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DarkCrystal.Serialization
{
    public static class TypeRegistry
    {
        private static Dictionary<Type, TypeIndex> IndicesByType;
        private static Dictionary<TypeIndex, Type> TypesByIndex;

        static TypeRegistry()
        {
            IndicesByType = new Dictionary<Type, TypeIndex>();
            TypesByIndex = new Dictionary<TypeIndex, Type>();

            foreach (var type in Utils.GetAssemblyTypes())
            {
                var darkAttribute = type.GetCustomAttribute<DarkContractAttribute>(false);
                if (darkAttribute != null)
                {
                    if (darkAttribute.TypeIndex != TypeIndex.Invalid)
                    {
                        var index = darkAttribute.TypeIndex;  
                        IndicesByType[type] = index;
                        TypesByIndex[index] = type;
                    }
                }
            }
        }

        static public TypeIndex GetIndex(Type type)
        {
            if (!IndicesByType.TryGetValue(type, out TypeIndex index))
            {
                return TypeIndex.Invalid;
            }
            return index;
        }

        static public bool TryGetType(TypeIndex index, out Type type)
        {
            return TypesByIndex.TryGetValue(index, out type);
        }
        
        static public Type GetType(TypeIndex index)
        {
            if (!TypesByIndex.TryGetValue(index, out Type type))
            {
                throw new System.Exception("Type index is not set: " + index.ToString());
            }
            return type;
        }

        static public TypeIndex GetIndex<T>() => (TypeIndex)TypeIndex<T>.Value;
    }
}