
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace DarkCrystal
{
    public static class GUIClipStack
    {
        public static Stack<Action> Stack = new Stack<Action>();

        public static void BeginArea(Rect rect)
        {
            Stack.Push(GUILayout.EndArea);
            GUILayout.BeginArea(rect);
        }

        public static Vector2 BeginScroll(Vector2 ScrollPosition, GUIStyle style = null)
        {
            Stack.Push(EditorGUILayout.EndScrollView);
            if (style != null)
            {
                return EditorGUILayout.BeginScrollView(ScrollPosition, style);
            }
            return EditorGUILayout.BeginScrollView(ScrollPosition);
        }

        public static void End()
        {
            Stack.Pop()();
        }

        public static void PopAll()
        {
            while (Stack.Count > 0)
            {
                End();
            }
        }
    }
}

#endif