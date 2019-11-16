
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;

namespace DarkCrystal.Encased.Core.Startup
{
    public partial class EditorOptions : EditorWindow
    {
        public static DateTime LastDataBaseSyncTime
        {
            get => new DateTime(Int64.Parse(EditorPrefs.GetString("LastDataBaseSyncTime", "0")));
            set => EditorPrefs.SetString("LastDataBaseSyncTime", value.Ticks.ToString());
        }
    }
}

#endif