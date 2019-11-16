
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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