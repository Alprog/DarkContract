
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace DarkCrystal.FieldSystem
{
    public class DefaultFieldInfo
    {
        public readonly Type BaseType;
        public readonly FieldKey Key;

        public DefaultFieldInfo(Type baseType, TypeIndex typeIndex)
        {
            this.BaseType = baseType;
            this.Key = new FieldKey(TypeIndex.TypeIndex, (int)typeIndex);
        }
    }
}