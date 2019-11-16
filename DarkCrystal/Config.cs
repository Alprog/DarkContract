
using UnityEngine;

namespace DarkCrystal
{
    public class Config
    {
        public const string StaticDistributedFolder = "Assets/Content/GameData/";
        public const string StaticBuildPackageFileName = "StaticData.sav";
        public const string TempEditorStateFile = "EditorTempState.sav";

        public static string StaticDistributedFile = Utils.PathCombine(StaticDistributedFolder, "Main.sav");
        public static string StaticBuildPackageFile => Utils.PathCombine(Application.streamingAssetsPath, StaticBuildPackageFileName);
    }
}