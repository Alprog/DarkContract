
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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