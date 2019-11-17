
#if UNITY_EDITOR

using System;
using System.Collections.Generic;
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

        public static bool LinkButton(string name, System.Action action)
        {
            bool result = false;
            var text = "<color=#0000FF>" + name + "</color>";
            var width = GUIStyles.Link.CalcSize(new GUIContent(text)).x;
            if (GUILayout.Button(text, GUIStyles.Link, GUILayout.Width(width)))
            {
                action();
                result = true;
            }

            var rect = GUILayoutUtility.GetLastRect();
            rect.width = width;
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
            return result;
        }

        public static void ReadonlyTextField(string label, string text, params GUILayoutOption[] options)
        {
            var backgrounColor = GUI.backgroundColor;
            GUI.backgroundColor = GUIStyles.ReadOnlyBackgroundColor;
            EditorGUILayout.TextField(label, text, options);
            GUI.backgroundColor = backgrounColor;
        }

        public static void ReadonlyTextArea(string text, params GUILayoutOption[] options)
        {
            var enabledState = GUI.enabled;
            GUI.enabled = false;

            var backgrounColor = GUI.backgroundColor;
            GUI.backgroundColor = GUIStyles.ReadOnlyBackgroundColor;
            EditorGUILayout.TextArea(text, options);
            GUI.backgroundColor = backgrounColor;

            GUI.enabled = enabledState;
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

        public static bool TinyToggle(string label, ref bool value, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            GUIStyle.none.CalcMinMaxWidth(new GUIContent(label), out var minWidth, out var maxWidth);
            EditorGUILayout.LabelField(label, Layout.Width(minWidth));
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

        public static T ObjectField<T>(T value, params GUILayoutOption[] options) where T : UnityEngine.Object
        {
            return EditorGUILayout.ObjectField(value, typeof(T), false, options) as T;
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
        
        public static bool TextArea(ref string value, GUIStyle style, Color backgroundColor, params GUILayoutOption[] options)
        {
            var defaultGuiColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundColor;
            var wasChanged = TextArea(ref value, style, options);
            GUI.backgroundColor = defaultGuiColor;
            return wasChanged;
        }

        public static bool TextArea(ref string value, GUIStyle style, params GUILayoutOption[] options)
        {
            if (value == null)
            {
                value = String.Empty;
            }
            var newValue = GUILayout.TextArea(value, style, options);
            if (value != newValue)
            {
                value = newValue;
                return true;
            }
            return false;
        }
        
        public static bool ObjectField<T>(string label, ref T value, params GUILayoutOption[] options) where T : UnityEngine.Object
        {
            int instanceId = value == null ? 0 : value.GetInstanceID();
            var newValue = EditorGUILayout.ObjectField(label, value, typeof(T), true, options) as T;
            if (instanceId != (newValue == null ? 0 : newValue.GetInstanceID()))
            {
                value = newValue;
                return true;
            }
            return false;
        }

        public static bool ObjectField<T>(ref T value, params GUILayoutOption[] options) where T : UnityEngine.Object
        {
            int instanceId = value == null ? 0 : value.GetInstanceID();
            var newValue = EditorGUILayout.ObjectField(value, typeof(T), true, options) as T;
            if (instanceId != (newValue == null ? 0 : newValue.GetInstanceID()))
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

        public static bool ColorField(string name, ref Color value, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.ColorField(name, value, options);
            if (value != newValue)
            {
                value = newValue;
                return true;
            }
            return false;
        }

        public static bool ShowConfirmation(string message)
        {
            return EditorUtility.DisplayDialog("Are you sure?", message, "yes", "no");
        }

        public static void FoldButton(ref bool foldOut)
        {
            var text = foldOut ? "▼" : "►";
            if (Layout.Button(text, UnityEngine.GUI.skin.label))
            {
                foldOut = !foldOut;
            }
        }

        public static void LinePanel(string caption, Texture2D icon = null, GUIStyle labelStyle = null)
        {
            LinePanel(caption, Color.white, icon, GUIStyles.Panel, labelStyle);
        }

        public static void LinePanel(string caption, Color guiColor, Texture2D icon = null, GUIStyle panelStyle = null, GUIStyle labelStyle = null)
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

        public static void FoldLine(ref bool foldOut, Color guiColor, string caption = null, Texture2D icon = null, GUIStyle panelStyle = null, GUIStyle labelStyle = null)
        {
            if (FoldLine(foldOut, guiColor, caption, icon, panelStyle, labelStyle))
            {
                foldOut = !foldOut;
                GUIClipStack.PopAll();
                Utils.RefreshEditors();
                GUIUtility.ExitGUI();
            }
        }

        public static void ShurikenLine(int indent = 0)
        {
            GUILayout.Space(4);
            if (indent > 0)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(indent);

                GUILayout.Box(String.Empty, "IN Title", GUILayout.ExpandWidth(true), GUILayout.Height(1));

                GUILayout.Space(indent);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Box(String.Empty, "IN Title", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            }
            GUILayout.Space(4);
        }
        
        public static void LabelField(string text, string stylisedText, GUIStyle guiStyle, params GUILayoutOption[] layoutOptions)
        {
            EditorGUILayout.BeginHorizontal();
            if (layoutOptions == null)
            {
                layoutOptions = new GUILayoutOption[0];
            }

            float lineHeight = Math.Max(guiStyle.lineHeight, guiStyle.fixedHeight);
            GUILayoutOption[] layoutOptionsModified = new GUILayoutOption[layoutOptions.Length + 2];
            layoutOptions.CopyTo(layoutOptionsModified, 0);

            layoutOptionsModified[layoutOptionsModified.Length - 1] = GUILayout.Width(text.Length * 8f);
            layoutOptionsModified[layoutOptionsModified.Length - 2] = GUILayout.Height(lineHeight);

            EditorGUILayout.LabelField(text, layoutOptionsModified);

            layoutOptionsModified = new GUILayoutOption[layoutOptions.Length + 1];
            layoutOptions.CopyTo(layoutOptionsModified, 0);
            layoutOptionsModified[layoutOptionsModified.Length - 1] = GUILayout.Height(lineHeight);

            EditorGUILayout.LabelField(stylisedText, guiStyle, layoutOptionsModified);
            EditorGUILayout.EndHorizontal();
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
                
        public delegate void DrawListElementDelegate<TRef>(ref TRef data);
        public static void DrawList<TRef>(DrawListElementDelegate<TRef> elementDrawFunc, ref List<TRef> data) where TRef : new()
        {
            if (data == null)
            {
                data = new List<TRef>();
            }

            var buttonStyle = GUIStyles.ButtonWithLabelMarginAndPadding;
            buttonStyle.fixedWidth = buttonStyle.fixedHeight = 20;
            for (int i = 0; i < data.Count; i++)
            {
                Layout.BeginHorizontal();
                var elem = data[i];
                elementDrawFunc(ref elem);
                data[i] = elem;
                if (Layout.Button("X", buttonStyle))
                {
                    data.RemoveAt(i);
                    throw new GUIExpireException();
                }
                Layout.EndHorizontal();
            }
            if (Layout.Button("Add"))
            {
                data.Add(new TRef());
                throw new GUIExpireException();
            }
        }
    }

    // shortcut
    public class Layout : ExtendedGUILayout
    {
    }
}

#endif