
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace DarkCrystal.UnityIntegration
{
    public partial class DataExplorer : Singleton<DataExplorer>
    {
        public const int FoldOffset = 12;
        public const int IconWidth = 17;
        private const int PickerWidth = 11;
        private const int AddComboboxWidth = 25;

        private GameData GameData;

        public DataSelector Selector { get; private set; }
        private Vector2 ScrollPosition;

        private static DataExplorer Current;

        private object HoveredObject;
        private bool IsMouseUp;
        
        private Rect currentRect;

        private DataExplorer()
        {
            Selector = ScriptableObject.CreateInstance<DataSelector>();
        }

        private bool IsMouseOnRect(Vector2 pos)
        {
            return currentRect.Contains(pos);
        }

        public void Select(GuidObject guidObject)
        {
            SelectGuidObject(guidObject);
        }

        public void OnGUI(Func<Rect> drawAction)
        {
            IsMouseUp = Event.current.type == EventType.MouseUp;
            HoveredObject = null;

            try
            {
                currentRect = drawAction();
            }
            catch (GUIExpireException exception)
            {
                GUIClipStack.PopAll();
                exception.Process();
            }
        }

        public void FullDraw()
        {
            GUIStyles.OnGUI();

            if (!Selector)
            {
                Selector = ScriptableObject.CreateInstance<DataSelector>();
            }

            if (EditorUtility.scriptCompilationFailed)
            {
                EditorGUILayout.HelpBox("Has compilation errors", MessageType.Error);
            }

            if (FileSystemTracker.Instance.HasChanges)
            {
                EditorGUILayout.HelpBox("Has changes on disk", MessageType.Warning);
            }

            UnityEngine.GUI.enabled = !Application.isPlaying && !EditorUtility.scriptCompilationFailed;
            Layout.BeginHorizontal();
            Layout.Button("Save", GameState.Instance.SaveDatabase, GUILayout.Width(100));
            Layout.Button("Load", GameState.Instance.ReloadDatabase, GUILayout.Width(100));
            Layout.EndHorizontal();
            UnityEngine.GUI.enabled = true;

            ScrollPosition = GUIClipStack.BeginScroll(ScrollPosition, GUIStyles.DarkGrayBackground);

            var gameData = GameState.Instance.GameData;
            if (gameData != null)
            {
                DrawFolderContent(gameData.RootFolder);
            }
            GUIClipStack.End();
        }

        public void Unselect()
        {
            Selector.Unselect();
        }

        public void DrawFolderContent(GuidObject.Folder folder, bool allowAdding = true)
        {
            Layout.BeginHorizontal();
            DrawFolderHeaderItem(folder);
            Layout.EndHorizontal();

            if (folder.FoldOut)
            {
                Layout.BeginOffset(FoldOffset);

                // subfolders
                if (folder.SubFolders != null)
                {
                    foreach (var subFolder in folder.SubFolders.Values)
                    {
                        DrawFolderContent(subFolder, allowAdding);
                    }
                }

                // items
                if (folder.FullItemCount == 0)
                {
                    Layout.Label("<empty>");
                }
                else
                {
                    foreach (var guidObject in folder.OwnItems)
                    {
                        DrawObjectItem(guidObject);
                    }
                }

                Layout.EndOffset();
            }
        }
        
        private void DrawFolderHeaderItem(GuidObject.Folder folder)
        {
            var caption = String.Format("{0} ({1})", folder.Name, folder.FullItemCount);
            if (folder.SkipCodegen)
            {
                caption += " skip";
            }
            Layout.FoldLine(ref folder.FoldOut, caption, null);

            var rect = GUILayoutUtility.GetLastRect();
            if (rect.Contains(Event.current.mousePosition))
            {
                HoveredObject = folder;
            }
        }

        private void DrawObjectItem(GuidObject guidObject)
        {
            var caption = guidObject.ToString();
          
            var selected = guidObject == Selector.SelectedObject && UnityEditor.Selection.activeObject == Selector;
            var style = selected ? GUIStyles.BlueListItem : GUIStyles.ListItem;
            Layout.BeginHorizontal(style);
            Layout.Label(caption, GUIStyles.TightLabel);

            Layout.FlexibleSpace();
            Layout.EndHorizontal();

            var rect = GUILayoutUtility.GetLastRect();
            if (rect.Contains(Event.current.mousePosition))
            {
                HoveredObject = guidObject;

                if (IsMouseUp)
                {
                    SelectGuidObject(guidObject);
                }
            }
        }
        
        private void SelectGuidObject(GuidObject guidObject)
        {
            Selector.Select(guidObject);
        }
    }
}

#endif