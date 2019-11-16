
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace DarkCrystal.Serialization
{
    public class RuntimeKeyAttribute : KeyAttribute
    {
        public RuntimeKeyAttribute(int value, string distributedSubPath = null):
            base(value, distributedSubPath, SerializationFlags.Runtime)
        {
        }
    }
}