
using DarkCrystal.Serialization;
using System.Collections.Generic;

namespace DarkCrystal.FieldSystem
{
    public struct FieldComparer
    {
        private int Index;
        private List<FieldBytes> OriginalBytes;
        public List<FieldKey> DeletedKeys { get; private set; }

        public FieldComparer(List<FieldBytes> originalBytes)
        {
            this.Index = 0;
            this.OriginalBytes = originalBytes;
            this.DeletedKeys = null;
        }

        // work only on sorted fieldKeys calls
        public bool IsChanged(FieldKey key, byte[] bytes, int offset, int length)
        {
            if (OriginalBytes != null)
            {
                while (Index < OriginalBytes.Count)
                {
                    var result = key.CompareTo(OriginalBytes[Index].Key);
                    if (result < 0)
                    {
                        return true; // new field was inserted between keys
                    }
                    else if (result == 0)
                    {
                        return !OriginalBytes[Index++].ByteSlice.Equals(bytes, offset, length); // check equality of same field
                    }
                    else
                    {
                        AddDeletedKey(OriginalBytes[Index++].Key); // old field was deleted
                    }
                }
            }
            return true; // new field was added in the end
        }

        public void RollToTheEnd()
        {
            if (OriginalBytes != null)
            {
                while (Index < OriginalBytes.Count)
                {
                    AddDeletedKey(OriginalBytes[Index++].Key);
                }
            }
        }

        private void AddDeletedKey(FieldKey key)
        {
            if (DeletedKeys == null)
            {
                DeletedKeys = new List<FieldKey>() { key };
            }
            else
            {
                DeletedKeys.Add(key);
            }
        }
    }
}