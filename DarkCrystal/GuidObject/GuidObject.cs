
using System;
using System.Collections.Generic;
using DarkCrystal.FieldSystem;
using DarkCrystal.Serialization;

namespace DarkCrystal
{
    [AbstractDarkContract]
    public abstract partial class GuidObject
    {
        protected virtual bool Reg(bool state) => Registrator.SetObject(this, state);

        public static void ReleaseAll()
        {
            if (GuidStorage<GuidObject>.Count > 0)
            {
                var list = new List<GuidObject>();
                list.AddRange(GuidStorage<GuidObject>.Items);
                foreach (var guidObject in list)
                {
                    guidObject.Release();
                }
            }
        }

        public Guid Guid { get; protected set; }
        public Folder ParentFolder { get; protected set; }
        
        public abstract string ID { get; set; }

        public GuidObject(Guid guid)
        {
            this.Guid = guid;
            Reg(true);
        }

        private void SetParentFolder(Folder folder)
        {
            this.ParentFolder = folder;
        }

        public virtual void Release()
        {
            Reg(false);
            if (ParentFolder != null)
            {
                ParentFolder.Remove(this);
            }
        }

        public virtual string GetFileName()
        {
            return string.Format("{0}.{1}", ID, this.Guid.ToString());
        }

        public override string ToString()
        {
            return String.Format("{0} [{1}]", ID, Guid.ToString());
        }

        public GuidObject Clone(Guid newGuid, string newID)
        {
            var oldGuid = this.Guid;
            this.Guid = newGuid;
            GuidObject clone = null;
            try
            {
                SerializationSettings settings;
                if (UnityEngine.Application.isPlaying)
                {
                    settings = new SerializationSettings(SerializationFlags.Runtime | SerializationFlags.Binary);
                }
                else
                {
                    settings = new SerializationSettings(SerializationFlags.Static | SerializationFlags.Binary);
                }
                var bytes = Serializer.Instance.Serialize(this, settings);
                clone = Serializer.Instance.Deserialize<GuidObject>(bytes, settings);
                clone.ID = newID;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.ToString());
            }
            finally
            {
                this.Guid = oldGuid;
            }
            return clone;
        }

        public ByteSlice OriginalBytes;
    }
}