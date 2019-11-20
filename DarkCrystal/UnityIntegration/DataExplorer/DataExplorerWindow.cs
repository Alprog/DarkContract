
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace DarkCrystal.UnityIntegration
{
    public class DataExplorerWindow : EditorWindow
    {
        [MenuItem("DarkContractExample/DataExplorer")]
        public static DataExplorerWindow ShowWindow()
        {
            var window = EditorWindow.GetWindow(typeof(DataExplorerWindow)) as DataExplorerWindow;
            window.titleContent = new GUIContent("DataExplorer");
            return window;
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        public void OnGUI()
        {
            DataExplorer.Instance.OnGUI(FullDraw);
        }

        private Rect FullDraw()
        {
            DataExplorer.Instance.FullDraw();
            return new Rect(Vector2.zero, position.size);
        }
    }
}

#endif