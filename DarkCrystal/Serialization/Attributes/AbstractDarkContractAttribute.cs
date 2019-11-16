
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace DarkCrystal.Serialization
{
    public class AbstractDarkContractAttribute : DarkContractAttribute
    {
        public AbstractDarkContractAttribute(bool isSerializable = false):
            base(TypeIndex.Invalid, GetFlags(isSerializable))
        {            
        }

        private static DarkFlags GetFlags(bool isSerializable)
        {
            var flags = DarkFlags.Abstract;
            if (isSerializable)
            {
                flags |= DarkFlags.Serializable;
            }
            return flags;
        }
    }
}