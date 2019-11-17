
#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;

namespace DarkCrystal.UnityIntegration
{
    public partial class ExtendedGUILayout : GUILayout
    {
        private const float FButtonWidth = 14.0f;
        
        public static bool Button(GUIContent guiContent, System.Action action, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(guiContent, options))
            {
                action();
                return true;
            }
            return false;
        }

        public static bool Button(GUIContent guiContent, System.Action action, GUIStyle style, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(guiContent, style, options))
            {
                action();
                return true;
            }
            return false;
        }

        public static bool Button(string name, System.Action action, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(name, options))
            {
                action();
                return true;
            }
            return false;
        }

        public static bool Button(string name, System.Action action, GUIStyle style, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(name, style, options))
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

        public static bool Toggle(string label, ref bool value, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUIStyles.RightAlignment, GUILayout.Width(EditorGUIUtility.labelWidth - 12));
            var newValue = EditorGUILayout.Toggle(value, options);
            EditorGUILayout.EndHorizontal();
            if (value != newValue)
            {
                value = newValue;
                return true;
            }
            return false;
        }
        
        public static bool ToggleLeft(string label, ref bool value, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.ToggleLeft(label, value, options);
            if (value != newValue)
            {
                value = newValue;
                return true;
            }
            return false;
        }

        public static bool ToggleLeft(string label, bool value, Action<bool> onValueChanged, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.ToggleLeft(label, value, options);
            if (value != newValue)
            {
                onValueChanged(newValue);
                return true;
            }
            return false;
        }
        
        public static bool Vector2Field(string label, ref Vector2 value)
        {
            var newValue = EditorGUILayout.Vector2Field(label, value);
            if (value != newValue)
            {
                value = newValue;
                return true;
            }
            return false;
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
        
        public static bool Vector2IntField(string label, ref Vector2Int value, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.Vector2IntField(label, value, options);
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

        public static bool TextField(ref string value, GUIStyle style, params GUILayoutOption[] options)
        {
            var newValue = GUILayout.TextField(value, style, options);
            if (value != newValue)
            {
                value = newValue;
                return true;
            }
            return false;
        }
        
        public static bool IntField(string caption, ref int value, int startFrom = 0, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.IntField(caption, value, options);
            if (value != newValue || value < startFrom)
            {
                if (newValue < startFrom)
                {
                    newValue = startFrom;
                }
                value = newValue;
                return true;
            }
            return false;
        }

        public static bool IntField(string caption, ref int value, GUIStyle guiStyle, int startFrom = 0, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.IntField(caption, value, guiStyle, options);
            if (value != newValue || value < startFrom)
            {
                if (newValue < startFrom)
                {
                    newValue = startFrom;
                }
                value = newValue;
                return true;
            }
            return false;
        }

        public static bool IntField(ref int value, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.IntField(value, options);
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

        public static bool FloatField(ref float value, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.FloatField(value, options);
            if (value != newValue)
            {
                value = newValue;
                return true;
            }
            return false;
        }

        public static bool FloatField(string name, ref float value, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.FloatField(name, value, options);
            if (value != newValue)
            {
                value = newValue;
                return true;
            }
            return false;
        }

        public static bool FloatField(string name, ref float value, GUIStyle guiStyle, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.FloatField(name, value, guiStyle, options);
            if (value != newValue)
            {
                value = newValue;
                return true;
            }
            return false;
        }

        public static bool FloatField(string caption, ref float value, float startFrom = 0, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.FloatField(caption, value, options);
            if (value != newValue || value < startFrom)
            {
                if (newValue < startFrom)
                {
                    newValue = startFrom;
                }
                value = newValue;
                return true;
            }
            return false;
        }

        public static void FoldButton(ref bool foldOut)
        {
            var text = foldOut ? "▼" : "►";
            if (Layout.Button(text, UnityEngine.GUI.skin.label))
            {
                foldOut = !foldOut;
            }
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

        public static void HorizontalLine()
        {
            EditorGUILayout.LabelField(String.Empty, GUI.skin.horizontalSlider);
        }

        public static void HorizontalLine(float topMargin, float borMargin)
        {
            Layout.Space(topMargin);
            EditorGUILayout.LabelField(String.Empty, GUI.skin.horizontalSlider);
            Layout.Space(borMargin);
        }
    }

    // shortcut
    public class Layout : ExtendedGUILayout
    {
    }
}

#endif