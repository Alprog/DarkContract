
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR

using UnityEditor;
using System;

namespace DarkCrystal.UnityIntegration
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