
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace DarkCrystal.UnityIntegration
{
    public partial class DataExplorer
    {
        public class DataSelector : ScriptableObject
        {
            public GuidObject SelectedObject { get; private set; }

            public void Select(GuidObject guidObject)
            {
                this.SelectedObject = guidObject;
                Selection.activeObject = this;
                Utils.RefreshEditors();
            }

            public void Unselect()
            {
                SelectedObject = null;
            }
        }
    }
}

#endif