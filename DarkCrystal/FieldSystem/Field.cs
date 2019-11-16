
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace DarkCrystal.FieldSystem
{
    public struct Field
    {
        public readonly FieldKey Key;
        public readonly object Value;

        public Field(FieldKey key, object value)
        {
            this.Key = key;
            this.Value = value;
        }

        public static implicit operator Field(KeyValuePair<FieldKey, object> pair)
        {
            return new Field(pair.Key, pair.Value);
        }
    }
}