
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace DarkCrystal.Serialization
{
    public class SealedDarkContractAttribute : DarkContractAttribute
    {
        public SealedDarkContractAttribute(TypeIndex typeIndex):
            base(typeIndex, DarkFlags.Sealed | DarkFlags.Serializable)
        {
        }
    }
}