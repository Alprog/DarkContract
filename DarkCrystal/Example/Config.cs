
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace DarkCrystal
{
    public class Config
    {
        public const string StaticDistributedFolder = "Assets/GameData/";
        public const string StaticBuildPackageFileName = "StaticData.sav";
        public const string TempEditorStateFile = "EditorTempState.sav";

        public static string StaticDistributedFile = Utils.PathCombine(StaticDistributedFolder, "Main.sav");
        public static string StaticBuildPackageFile => Utils.PathCombine(Application.streamingAssetsPath, StaticBuildPackageFileName);
    }
}