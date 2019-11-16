
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace DarkCrystal
{
    public static class Utils
    {
        public static Type GetAssemblyType(string name)
        {
            return Assembly.GetAssembly(typeof(Utils)).GetType(name);
        }

        public static Type[] GetAssemblyTypes()
        {
            return Assembly.GetAssembly(typeof(Utils)).GetTypes();
        }

        public static string PathCombine(string a, string b)
        {
            return ToUnixPath(Path.Combine(a, b));
        }

        public static string PathCombine(string a, string b, string c)
        {
            return ToUnixPath(Path.Combine(a, b, c));
        }

        public static string ResolvePath(string a, string b)
        {
            return ToUnixPath(Path.GetFullPath(Path.Combine(a, b)));
        }

        public static string ToUnixPath(string path)
        {
            return path.Replace('\\', '/');
        }

        public static void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static bool IsNullOrEmpty<T>(this List<T> array)
        {
            return array == null || array.Count == 0;
        }

        public static DateTime GetLastWriteTimeInDirectory(string path)
        {
            var maxTime = DateTime.MinValue;
            foreach (var filePath in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            {
                if (filePath.EndsWith(".meta"))
                {
                    continue;
                }
                var time = File.GetLastWriteTimeUtc(filePath);
                if (time > maxTime)
                {
                    maxTime = time;
                }
            }
            return maxTime;
        }

#if UNITY_EDITOR
        public static void RefreshEditors()
        {
            var editors = Resources.FindObjectsOfTypeAll<UnityEditor.Editor>();
            foreach (var editor in editors)
            {
                editor.Repaint();
            }

            var windows = Resources.FindObjectsOfTypeAll<UnityEditor.EditorWindow>();
            foreach (var window in windows)
            {
                window.Repaint();
            }
        }
#endif
    }
}


