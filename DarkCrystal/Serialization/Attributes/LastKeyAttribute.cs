
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace DarkCrystal.Serialization
{
    public class LastKeyAttribute : Attribute
    {
        public int Value;

        public LastKeyAttribute(int value = 0)
        {
            this.Value = value;
        }
    }
}
