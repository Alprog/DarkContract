
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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