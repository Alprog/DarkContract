
using System.Collections.Generic;
using DarkCrystal.Serialization;

namespace DarkCrystal
{
    public abstract partial class GuidObject
    {
        public partial class Folder
        {
            [StaticDarkContract]
            public class PlainForm
            {
                [Key(0)] public string Name;
                [Key(1)] public List<GuidObject> Items;
                [Key(2)] public List<PlainForm> SubFolders;
                [Key(3)] public bool SkipCodegen;
                [Key(4)] public bool FoldOut;

                private PlainForm() { }

                public PlainForm(Folder folder)
                {
                    this.Name = folder.Name;
                    this.Items = folder.Items;
                    this.SubFolders = new List<PlainForm>(folder.SubFolders.Count);
                    foreach (var subfolder in folder.SubFolders.Values)
                    {
                        this.SubFolders.Add(new PlainForm(subfolder));
                    }
                    this.SkipCodegen = folder.SkipCodegen;
                    this.FoldOut = folder.FoldOut;
                }

                public void FillFields(Folder folder)
                {
                    folder.Name = this.Name;
                    folder.Items = this.Items;
                    folder.SubFolders = new Dictionary<string, Folder>();

                    foreach (var item in Items)
                    {
                        item.SetParentFolder(folder);
                    }
                     
                    foreach (var plainSubfolder in SubFolders)
                    {
                        var subfolder = new Folder(plainSubfolder.Name, folder);
                        plainSubfolder.FillFields(subfolder);
                        folder.SubFolders[plainSubfolder.Name] = subfolder;
                    }

                    folder.SkipCodegen = this.SkipCodegen;
                    folder.FoldOut = this.FoldOut;
                }
            }
        }
    }
}