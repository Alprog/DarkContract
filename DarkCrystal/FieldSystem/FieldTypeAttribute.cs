
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace DarkCrystal.FieldSystem
{
    public class FieldTypeAttribute : Attribute
    {
        public readonly Type FieldType;

        public FieldTypeAttribute(Type type)
        {
            this.FieldType = type;
        }
    }
}