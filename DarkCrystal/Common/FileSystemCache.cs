﻿
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace DarkCrystal
{
    public static class FileSystemCache
    {
        private static Dictionary<string, FileCache> FileCaches = new Dictionary<string, FileCache>();

        private static ConcurrentQueue<System.Action> AsyncQueue = new ConcurrentQueue<System.Action>();
        private static Thread Thread;
        private static bool ThreadRunning;

        public static byte[] ReadAllBytes(string path) => GetFileCache(path)?.Bytes;
        public static string ReadAllText(string path) => GetFileCache(path)?.Text;
        
        public static FileCache GetFileCache(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var time = File.GetLastWriteTimeUtc(path);

            if (FileCaches.TryGetValue(path, out var fileCache))
            {
                if (time == fileCache.LastWriteTime)
                {
                    return fileCache;
                }
            }

            var bytes = File.ReadAllBytes(path);
            fileCache = new FileCache(time, bytes);
            FileCaches[path] = fileCache;
            return fileCache;
        }

        public static void WriteAllText(string path, string text)
        {
            var currentText = ReadAllText(path);
            if (currentText == text)
            {
                return;
            }

            File.WriteAllText(path, text);
            var time = File.GetLastWriteTimeUtc(path);
            FileCaches[path] = new FileCache(time, null, text);
        }

        public static void WriteAllBytes(string path, byte[] bytes)
        {
            string text = null;
            var currentText = ReadAllText(path);
            if (currentText != null)
            {
                text = Encoding.UTF8.GetString(bytes);
                if (currentText == text)
                {
                    return;
                }
            }
            
            File.WriteAllBytes(path, bytes);
            var time = File.GetLastWriteTimeUtc(path);
            FileCaches[path] = new FileCache(time, bytes, text);
        }
    }
}