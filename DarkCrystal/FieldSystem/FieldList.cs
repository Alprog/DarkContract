
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using DarkCrystal.Serialization;
using System.Collections.Generic;

namespace DarkCrystal.FieldSystem
{
    public class FieldList : SortedList<FieldKey, object>
    {
        public FieldList() : base()
        {
        }

        public FieldList(int capacity) : base(capacity)
        {
        }

        public void AddFieldBytes(FieldBytes fieldBytes)
        {
            if (OriginalBytes == null)
            {
                OriginalBytes = new List<FieldBytes>();
            }
            OriginalBytes.Add(fieldBytes);
        }

        public List<FieldBytes> OriginalBytes;

        public void Merge(FieldList other)
        {
            var fieldKey = SystemField.DeletedKeys.ToFieldKey();
            if (other.TryGetValue(fieldKey, out object value))
            {
                var deletedKeys = value as List<FieldKey>;
                if (deletedKeys != null)
                {
                    foreach (var key in deletedKeys)
                    {
                        this.Remove(key);
                    }
                }
                other.Remove(fieldKey);
            }

            foreach (var pair in other)
            {
                this[pair.Key] = pair.Value;
            }
        }
    }
}