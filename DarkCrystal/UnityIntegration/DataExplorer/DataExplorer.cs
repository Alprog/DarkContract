
#if UNITY_EDITOR

using System;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DarkCrystal.UnityIntegration
{
    public partial class DataExplorer : Singleton<DataExplorer>
    {
        private const string CheckBoxTrueSymbol = "☑";
        private const string CheckBoxFalseSymbol ="☒";

        private const string SelectedViewTypePrefsKey = "DataExplorerViewType";
        private const string SelectedTileSizePrefsKey = "DataExplorerTileSize";

        public const int FoldOffset = 12;
        public const int IconWidth = 17;
        private const int PickerWidth = 11;
        private const int AddComboboxWidth = 25;
        private const float MinTileSize = 50f;
        private const float MaxTileSize = 100f;
        private const float MinTileFontSize = 8f;
        private const float MaxTileFontSize = 12f;

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

            if ((Application.isPlaying && Event.current.type != EventType.ContextClick) || !IsMouseOnRect(Event.current.mousePosition) || !UnityEngine.GUI.enabled)
            {
                return;
            }

            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (Event.current.clickCount == 1)
                    {
                        DragAndDrop.PrepareStartDrag(); // Clear
                    }
                    else if (Event.current.clickCount == 2)
                    {
                        if (HoveredObject is GuidObject)
                        {
                            (HoveredObject as GuidObject).OnDoubleClicked();
                            Event.current.Use();
                        }
                    }
                    break;
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
                DrawFolderContent(gameData.Entities);
                DrawFolderContent(gameData.Locations);
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
                    DrawContentAsList(folder);
                }

                Layout.EndOffset();
            }
        }
        
        private void DrawContentAsList(GuidObject.Folder folder)
        {
            foreach (var guidObject in folder.OwnItems)
            {
                DrawObjectItem(guidObject);
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

        private void ProcessObjectDrop(GuidObject guidObject, GuidObject.Folder destinationFolder)
        {
            if (guidObject.ParentFolder == null)
            {
                throw new Exception(String.Format("Can't find source folder of {0}", guidObject));
            }
        }
        
        private void DisplayErrorDialogAboutDuplicatedIDs(string messageFormat, List<string> duplicatedIDs)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(messageFormat, duplicatedIDs.Count);
            var maxLines = 20;
            var linesNumber = Mathf.Min(duplicatedIDs.Count, maxLines);
            for (int i = 0; i < linesNumber; i++)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(duplicatedIDs[i]);
            }
            if (duplicatedIDs.Count > maxLines)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append("...");
            }
            EditorUtility.DisplayDialog("Error - Operation Aborted", stringBuilder.ToString(), "Ok");
        }
    }
}

#endif