
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;

namespace DarkCrystal.UnityIntegration
{
    public partial class ExtendedGUILayout : GUILayout
    {
        public static bool Button(string name, System.Action action, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(name, options))
            {
                action();
                return true;
            }
            return false;
        }

        public static void ReadonlyTextField(string label, string text, params GUILayoutOption[] options)
        {
            var backgrounColor = GUI.backgroundColor;
            GUI.backgroundColor = GUIStyles.ReadOnlyBackgroundColor;
            EditorGUILayout.TextField(label, text, options);
            GUI.backgroundColor = backgrounColor;
        }

        public static bool Vector3Field(string label, ref Vector3 value)
        {
            var newValue = EditorGUILayout.Vector3Field(label, value);
            if (value != newValue)
            {
                value = newValue;
                return true;
            }
            return false;
        }

        public static bool TextField(string caption, ref string value, params GUILayoutOption[] options)
        {
            if (value == null)
            {
                value = String.Empty;
            }
            Layout.BeginHorizontal();
            var newValue = EditorGUILayout.TextField(caption, value, options);
            Layout.EndHorizontal();
            if (value != newValue)
            {
                value = newValue;
                return true;
            }
            return false;
        }

        public static bool IntField(string name, ref int value, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.IntField(name, value, options);
            if (value != newValue)
            {
                value = newValue;
                return true;
            }
            return false;
        }
        
        public static bool FoldLine(bool foldOut, Color guiColor, string caption = null, Texture2D icon = null, GUIStyle panelStyle = null, GUIStyle labelStyle = null)
        {
            var defaultGUIColor = GUI.color;
            GUI.color = guiColor;

            if (panelStyle == null)
            {
                panelStyle = GUIStyles.Panel;
            }
            if (labelStyle == null)
            {
                labelStyle = GUIStyles.TightLabel;
            }
            GUILayout.BeginHorizontal(panelStyle);
            Layout.Label(foldOut ? "▼" : "►", labelStyle);
            if (icon != null)
            {
                Layout.Label(new GUIContent(icon), labelStyle);
            }
            if (caption != null)
            {
                Layout.Label(caption, labelStyle);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.color = defaultGUIColor;

            if (Event.current.type == EventType.MouseUp)
            {
                var rect = GUILayoutUtility.GetLastRect();
                if (rect.Contains(Event.current.mousePosition))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool FoldLine(bool foldOut, string caption = null, Texture2D icon = null)
        {
            return FoldLine(foldOut, Color.white, caption, icon, GUIStyles.Panel);
        }

        public static void FoldLine(ref bool foldOut, string caption = null, Texture2D icon = null)
        {
            if (FoldLine(foldOut, caption, icon))
            {
                foldOut = !foldOut;
                throw new GUIExpireException();
            }
        }
        
        public static void BeginOffset(int width)
        {
            BeginHorizontal();
            Space(width);
            BeginVertical();
        }

        public static void EndOffset()
        {
            EndVertical();
            EndHorizontal();
        }
    }

    // shortcut
    public class Layout : ExtendedGUILayout
    {
    }
}

#endif