
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DarkCrystal.Serialization;

namespace DarkCrystal
{
    public abstract partial class GuidObject
    {
        [InlineDarkContract]
        public partial class Folder
        {
            public string Name { get; private set; }
            public Folder Parent { get; private set; }
            public Dictionary<string, Folder> SubFolders;
            private List<GuidObject> Items;

            public bool SkipCodegen;
            public bool FoldOut;

            public Folder(string name, Folder parent = null)
            {
                this.Name = name;
                this.Parent = parent;
                this.Items = new List<GuidObject>();
                this.SubFolders = new Dictionary<string, Folder>();
            }

            public string GetFullPath(string separator = "/")
            {
                if (Parent == null)
                {
                    return Name;
                }

                var parentPath = Parent.GetFullPath(separator);
                return String.Concat(parentPath, separator, Name);
            }

            public string GetScopePath(string separator = "/")
            {
                if (Parent == null)
                {
                    return Name;
                }

                var parentPath = Parent.GetScopePath(separator);
                if (SkipCodegen)
                {
                    return parentPath;
                }

                return String.Concat(parentPath, separator, Name);
            }

            public int OwnItemCount => Items.Count;

            public int ScopeItemCount => GetScopeItemCount(GetBaseFolderAtSameScope());

            public int FullItemCount
            {
                get
                {
                    var count = Items.Count;
                    if (SubFolders != null)
                    {
                        foreach (var folder in SubFolders.Values)
                        {
                            count += folder.FullItemCount;
                        }
                    }
                    return count;
                }
            }

            public IEnumerable<GuidObject> OwnItems => Items;

            public IEnumerable<GuidObject> ScopeItems
            {
                get
                {
                    var baseFolder = GetBaseFolderAtSameScope();
                    foreach (var item in GetScopeItems(baseFolder))
                    {
                        yield return item;
                    }
                }
            }

            public IEnumerable<GuidObject> AllItems
            {
                get
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        yield return Items[i];
                    }
                    foreach (var folder in SubFolders.Values)
                    {
                        foreach (var item in folder.AllItems)
                        {
                            yield return item;
                        }
                    }
                }
            }

            public GuidObject GetOwnItem(int index)
            {
                return Items[index];
            }

            public static IEnumerable<GuidObject> GetScopeItems(GuidObject.Folder folder)
            {
                foreach (var item in folder.Items)
                {
                    yield return item;
                }
                foreach (var subFolder in folder.SubFolders.Values)
                {
                    if (subFolder.SkipCodegen)
                    {
                        foreach (var item in GetScopeItems(subFolder))
                        {
                            yield return item;
                        }
                    }
                }
            }

            public GuidObject.Folder GetBaseFolderAtSameScope()
            {
                var baseFolder = this;
                while (baseFolder.SkipCodegen && baseFolder.Parent != null)
                {
                    baseFolder = baseFolder.Parent;
                }
                return baseFolder;
            }

            public void ChangeName(string newName)
            {
                if (Parent != null)
                {
                    Parent.SubFolders.Remove(Name);
                    Parent.SubFolders.Add(newName, this);
                }
                Name = newName;
            }

            private int GetScopeItemCount(GuidObject.Folder folder)
            {
                int count = folder.Items.Count;
                foreach (var subFolder in folder.SubFolders.Values)
                {
                    if (subFolder.SkipCodegen)
                    {
                        count += GetScopeItemCount(subFolder);
                    }
                }
                return count;
            }

            #region Serialization

            [Key(0, SerializationFlags.OnePackage)]
            private PlainForm NormalSerialization
            {
                get
                {
                    return new PlainForm(this);
                }
                set
                {
                    value.FillFields(this);
                }
            }

            [Key(0, SerializationFlags.Distributed)]
            private List<GuidObject> DistributedSerialization
            {
                get
                {
                    var folderPath = Serializer.Instance.State.Settings.DistributedFolder;
                    
                    var oldFiles = GetCurrentFileSet(folderPath); 
                    SaveToFolder(folderPath, oldFiles);
                    foreach (var oldFile in oldFiles)
                    {
                        File.Delete(oldFile);
                        File.Delete(oldFile + ".meta");
                    }
                    ClearRemovedFolders(this);

                    return null;
                }
                set
                {
                    var folderPath = Serializer.Instance.State.Settings.DistributedFolder;
                    if (Directory.Exists(folderPath))
                    {
                        LoadFromFolder(folderPath);
                    }
                }
            }

            private void ClearRemovedFolders(Folder folder)
            {
                var fullFolderPath = Utils.PathCombine(Config.StaticDistributedFolder, folder.GetFullPath());
                try
                {
                    var childFolders = Directory.GetDirectories(fullFolderPath);
                    var substringLength = fullFolderPath.Length + 1;
                    foreach (var folderPath in childFolders)
                    {
                        var folderName = folderPath.Substring(substringLength, folderPath.Length - substringLength);
                        if (!folder.ContainsFolder(folderName))
                        {
                            Directory.Delete(folderPath, true);
                            File.Delete(folderPath + ".meta");
                            UnityEngine.Debug.LogFormat("Directory removed: {0}, <b>{1}</b>", folderName, folderPath);
                        }
                    }
                    foreach (var subFolder in folder.SubFolders.Values)
                    {
                        ClearRemovedFolders(subFolder);
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(fullFolderPath + "   " + e.Message);
                }
            }

            private HashSet<string> GetCurrentFileSet(string folderPath)
            {
                var set = new HashSet<string>();
                foreach (var filePath in Directory.GetFiles(folderPath, "*.sav", SearchOption.AllDirectories))
                {
                    set.Add(filePath.Replace('\\', '/').ToLowerInvariant());
                }
                return set;
            }

            private void SaveToFolder(string folderPath, HashSet<string> oldFiles)
            {
                Utils.EnsureDirectory(folderPath);

                var flags = Serializer.Instance.State.Settings.Flags;

                var count = Items.Count;

                Parallel.ForEach(Items, (GuidObject item) =>
                {
                    var path = Utils.PathCombine(folderPath, item.GetFileName() + ".sav");
                    var settings = new SerializationSettings(flags, item);
                    var bytes = Serializer.Instance.Serialize(item, settings);
                    FileSystemCache.WriteAllBytesAsync(path, bytes);

                    oldFiles.Remove(path.ToLowerInvariant());
                });

                var settingsPath = Utils.PathCombine(folderPath, "settings.ini");
                SaveSettings(settingsPath);

                foreach (var subFolder in SubFolders.Values)
                {
                    subFolder.SaveToFolder(Utils.PathCombine(folderPath, subFolder.Name), oldFiles);
                }
            }

            private void LoadFromFolder(string folderPath)
            {
                var dirInfo = new DirectoryInfo(folderPath);
                var items = new List<GuidObject>();
                var settings = Serializer.Instance.State.Settings;

                var files = dirInfo.GetFiles("*.sav");
                foreach (var fileInfo in files)
                {
                    var path = Utils.PathCombine(folderPath, fileInfo.Name);
                    var bytes = FileSystemCache.ReadAllBytes(path);
                    var item = Serializer.Instance.Deserialize<GuidObject>(bytes, settings);
                    item.SetParentFolder(this);
                    lock (items)
                    {
                        items.Add(item);
                    }
                }
                JoinItems(items);

                var settingsPath = Utils.PathCombine(folderPath, "settings.ini");
                LoadSettings(settingsPath);

                foreach (var subDirInfo in dirInfo.GetDirectories())
                {
                    var subFolder = new Folder(subDirInfo.Name, this);
                    subFolder.LoadFromFolder(Utils.PathCombine(folderPath, subDirInfo.Name));
                    SubFolders[subDirInfo.Name] = subFolder;
                }
            }

            private string CutPath(string folderPath)
            {
                const string subString = "GameData/";

                var index = folderPath.IndexOf(subString);
                if (index >= 0)
                {
                    return folderPath.Substring(index + subString.Length);
                }
                else
                {
                    return folderPath;
                }
            }

            private void JoinItems(List<GuidObject> items)
            {
                var set = new HashSet<GuidObject>();
                foreach (var item in this.Items)
                {
                    set.Add(item);
                }

                foreach (var item in items)
                {
                    if (!set.Contains(item))
                    {
                        Items.Add(item);
                    }
                }
            }

            #endregion

            public bool Contains(GuidObject item, bool recursive = false)
            {
                IEnumerable<GuidObject> enumerable = recursive ? AllItems : Items;
                foreach (var value in enumerable)
                {
                    if (item == value)
                    {
                        return true;
                    }
                }
                return false;
            }

            public bool Contains(Folder folder, bool recursive = false)
            {
                if (folder.Parent != null)
                {
                    if (folder.Parent == this)
                    {
                        return true;
                    }
                    else if (recursive)
                    {
                        return Contains(folder.Parent, true);
                    }
                }
                return false;
            }

            public bool FindFolderRecursive(string name, out Folder folder, bool ignoreCase = true)
            {
                folder = null;
                var comparisonType = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
                if (String.Equals(Name, name, comparisonType))
                {
                    folder = this;
                    return true;
                }
                else
                {
                    foreach (var subFolder in SubFolders.Values)
                    {
                        if (subFolder.FindFolderRecursive(name, out folder, ignoreCase))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public bool ContainsFolder(string name, bool ignoreCase = true)
            {
                var comparisonType = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
                if (String.Equals(Name, name, comparisonType))
                {
                    return true;
                }
                else
                {
                    foreach (var subFolder in SubFolders.Values)
                    {
                        if (String.Equals(subFolder.Name, name, comparisonType))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public void Reparent(Folder newParent)
            {
                if (this.Contains(newParent, true))
                {
                    throw new Exception("Can't reparent folder to its child");
                }

                if (newParent.SubFolders.ContainsKey(this.Name))
                {
                    throw new Exception(String.Format("Destination folder already has subfolder {0}", this.Name));
                }

                if (this.Parent != null)
                {
                    Parent.SubFolders.Remove(this.Name);
                }
                this.Parent = newParent;
                if (this.Parent != null)
                {
                    Parent.SubFolders[this.Name] = this;
                }
            }

            public bool Replace(GuidObject oldItem, GuidObject newItem)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i] == oldItem)
                    {
                        oldItem.SetParentFolder(null);
                        Items[i] = newItem;
                        newItem.SetParentFolder(this);
                        return true;
                    }
                }

                return false;
            }

            public void Sort()
            {
                Items.Sort((GuidObject a, GuidObject b) =>
                {
                    return String.Compare(a.ID, b.ID);
                });
            }

            public Folder AddSubFolder(string name)
            {
                var subFolder = new Folder(name, this);
                SubFolders.Add(name, subFolder);
                return subFolder;
            }

            public void Add(GuidObject item)
            {
                if (item.ParentFolder != null)
                {
                    throw new Exception("Item already added in another folder");
                }
                Items.Add(item);
                item.SetParentFolder(this);
            }

            public void AddRange(IEnumerable<GuidObject> items)
            {
                foreach (var item in items)
                {
                    Add(item);
                }
            }

            public GuidObject AddNew(Type type)
            {
                var item = type.InvokeDefaultConstructor() as GuidObject;
                item.ID = GetUniqueID(String.Format("New{0}", type.Name));
                item.SetParentFolder(this);
                Items.Add(item);
                return item;
            }

            public T AddNew<T>() where T : GuidObject
            {
                return AddNew(typeof(T)) as T;
            }

            public bool Remove(GuidObject guidObject)
            {
                if (guidObject.ParentFolder == this)
                {
                    if (Items.Remove(guidObject))
                    {
                        guidObject.SetParentFolder(null);
                        return true;
                    }
                }
                return false;
            }

            public GuidObject Find(Predicate<GuidObject> predicate, bool recursive = false)
            {
                IEnumerable<GuidObject> enumerable = recursive ? AllItems : Items;
                foreach (var item in enumerable)
                {
                    if (predicate(item))
                    {
                        return item;
                    }
                }

                return null;
            }

            public GuidObject FindObjectByID(string id, bool recursive = false)
            {
                IEnumerable<GuidObject> enumerable = recursive ? AllItems : Items;
                foreach (var item in enumerable)
                {
                    if (item.ID == id)
                    {
                        return item;
                    }
                }
                return null;
            }

            public string GetUniqueID(string baseId)
            {
                var newId = baseId;
                var index = 2;
                while (FindObjectByID(newId) != null)
                {
                    newId = string.Format("{0}_{1}", baseId, index++);
                }
                return newId;
            }

            public List<string> GetSubfolderNames()
            {
                var list = new List<string>();
                foreach (var subfolder in SubFolders.Values)
                {
                    list.Add(subfolder.Name);
                }
                return list;
            }

            public List<string> GetProhibitedFolderNamesInThisScope(GuidObject.Folder exceptFolder = null)
            {
                var list = new List<string>();
                var baseFolder = GetBaseFolderAtSameScope();
                list.Add(baseFolder.Name);
                list.AddRange(baseFolder.GetSubfolderNamesInChildScope(exceptFolder));
                return list;
            }

            public List<string> GetProhibitedFolderNamesForNewChild(GuidObject.Folder exceptFolder = null)
            {
                var list = GetProhibitedFolderNamesInThisScope(exceptFolder);
                if (!list.Contains(Name))
                {
                    list.Add(Name);
                }
                foreach (var subfolder in SubFolders.Values)
                {
                    if (subfolder != exceptFolder && !list.Contains(subfolder.Name))
                    {
                        list.Add(subfolder.Name);
                    }
                }
                return list;
            }

            public List<string> GetSubfolderNamesInChildScope(GuidObject.Folder exceptFolder = null)
            {
                var list = new List<string>();
                foreach (var subfolder in SubFolders.Values)
                {
                    if (subfolder == exceptFolder)
                    {
                        continue;
                    }
                    if (subfolder.SkipCodegen)
                    {
                        list.AddRange(subfolder.GetSubfolderNamesInChildScope(exceptFolder));
                    }
                    else
                    {
                        list.Add(subfolder.Name);
                    }
                }
                return list;
            }

            public List<string> GetScopeIds(GuidObject guidObject)
            {
                return GetScopeIds(guidObject?.ID);
            }

            public List<string> GetScopeIds(string exceptId = null)
            {
                var list = new List<string>();
                foreach (var item in ScopeItems)
                {
                    if (item.ID != exceptId)
                    {
                        list.Add(item.ID);
                    }
                }
                return list;
            }

            public override string ToString()
            {
                return Name;
            }

            public void FoldOutTree()
            {
                var folder = this;
                do
                {
                    folder.FoldOut = true;
                    folder = folder.Parent;
                }
                while (folder != null);
            }

            public void FoldInRecursive()
            {
                foreach (var folder in SubFolders.Values)
                {
                    folder.FoldOut = false;
                    folder.FoldInRecursive();
                }
            }

            public void SaveSettings(string filePath)
            {
                var builder = new StringBuilder();
                if (SkipCodegen) 
                {
                    builder.AppendLine("SkipCodegen");
                }

                var bytes = Encoding.ASCII.GetBytes(builder.ToString());
                FileSystemCache.WriteAllBytesAsync(filePath, bytes);
            }

            public void LoadSettings(string filePath)
            {
                if (File.Exists(filePath))
                {
                    var text = FileSystemCache.ReadAllText(filePath);
                    foreach (var line in text.Split('\r'))
                    {
                        if (line == "SkipCodegen")
                        {
                            this.SkipCodegen = true;
                        }
                    }
                }
            }
        }
    }
}