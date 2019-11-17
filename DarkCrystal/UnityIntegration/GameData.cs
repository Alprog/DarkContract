
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using DarkCrystal.Serialization;

namespace DarkCrystal
{
    [InlineDarkContract]
    public class GameData
    {
        [Key(0, "RootFolder")]
        public GuidObject.Folder Entities;
        
        public GameData()
        {
            Entities = new GuidObject.Folder("RootFolder");
        }
    }
}