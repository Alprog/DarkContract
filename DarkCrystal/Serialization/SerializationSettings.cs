
using System.Collections.Generic;

namespace DarkCrystal.Serialization
{
    public struct SerializationSettings
    {
        public SerializationFlags Flags;
        public bool PartialSerialization;
        public HashSet<GuidObject> InternalObjects;
        public string DistributedFolder;

        public SerializationSettings(SerializationFlags flags = 0)
        {
            this.Flags = flags;
            this.PartialSerialization = false;
            this.InternalObjects = null;
            this.DistributedFolder = string.Empty;
        }

        public SerializationSettings(SerializationFlags flags, IEnumerable<GuidObject> internalObjects)
        {
            this.Flags = flags;
            this.PartialSerialization = true;
            this.InternalObjects = new HashSet<GuidObject>();
            foreach (var guidObject in internalObjects)
            {
                this.InternalObjects.Add(guidObject);
            }
            this.DistributedFolder = string.Empty;
        }

        public SerializationSettings(SerializationFlags flags, GuidObject internalObject)
        {
            this.Flags = flags;
            this.PartialSerialization = true;
            this.InternalObjects = new HashSet<GuidObject>();
            this.InternalObjects.Add(internalObject);
            this.DistributedFolder = string.Empty;
        }
    }
}