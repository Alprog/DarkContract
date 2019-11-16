
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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