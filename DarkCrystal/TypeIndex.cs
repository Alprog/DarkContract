
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using DarkCrystal.Serialization;

namespace DarkCrystal
{
    [DarkContract(TypeIndex.TypeIndex)]
    public enum TypeIndex
    {
        Invalid = -1,

        TypeIndex = 0,
        GuidObject = 1,
        Entity = 2,


        // System
        SystemField = UInt16.MaxValue
    }
}