
using System;
using DarkCrystal.FieldSystem;
using DarkCrystal.Serialization;

namespace DarkCrystal
{
    [AbstractDarkContract]
    public abstract partial class GuidObject
    {
        public Guid Guid { get; protected set; }

        public Folder ParentFolder { get; protected set; }

        public abstract string ID { get; set; }

        public virtual Type SubclassType => GetType();

        public GuidObject(Guid guid)
        {
            this.Guid = guid;
        }

        public string FullPath
        {
            get
            {
                var currentFolder = ParentFolder;
                if (currentFolder == null)
                {
                    return String.Empty;
                }

                string path = currentFolder.Name;
                while ((currentFolder = currentFolder.Parent) != null)
                {
                    path = currentFolder.Name + "/" + path;
                }
                return path;
            }
        }

        public void Reparent(Folder newParentFolder)
        {
            if (this.ParentFolder != null)
            {
                this.ParentFolder.Remove(this);
            }
            newParentFolder.Add(this);
        }

        private void SetParentFolder(Folder folder)
        {
            this.ParentFolder = folder;
        }

        public virtual void Release(bool releaseAll = false)
        {
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
            return String.Format("GuidObject [{0}]", Guid.ToString());
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

        public virtual void OnDoubleClicked() { }

        public ByteSlice OriginalBytes;
    }
}