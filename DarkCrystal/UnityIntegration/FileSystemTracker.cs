
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;

namespace DarkCrystal
{
    public class FileSystemTracker : Singleton<FileSystemTracker>
    {
        public bool HasChanges { get; private set; }

        public void CommitChanges()
        {
            HasChanges = false;
        }

#if UNITY_EDITOR
        private Dictionary<string, FileSystemWatcher> Watchers;
        
        public void Run(string path)
        {
            Stop();

            var dirInfo = new DirectoryInfo(path);

            Watchers = new Dictionary<string, FileSystemWatcher>();
            foreach (var subDirectory in dirInfo.EnumerateDirectories("*", SearchOption.AllDirectories))
            {
                Watch(subDirectory.FullName);
            }
        }
         
        public void Stop()
        {
            if (Watchers != null)
            {
                foreach (var watcher in Watchers.Values)
                {
                    watcher.Dispose();
                }
                Watchers = null;
            }
        }

        private void Watch(string path)
        {
            var watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Changed += OnChanged;           
            watcher.Renamed += OnRenamed;
            watcher.EnableRaisingEvents = true;
            Watchers[path] = watcher;
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            if (Directory.Exists(e.FullPath))
            {
                Watch(e.FullPath);
            }
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            if (Watchers.TryGetValue(e.FullPath, out var watcher))
            {
                watcher.Dispose();
                Watchers[e.FullPath] = null;
            }

            OnFSChanged(e.FullPath);
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            OnFSChanged(e.FullPath);
        }

        private void OnRenamed(object source, FileSystemEventArgs e)
        {
            OnFSChanged(e.FullPath);
        }

        private void OnFSChanged(string path)
        {
            if (!path.EndsWith(".meta"))
            {
                HasChanges = true;
            }
        }
#endif
    }
}

