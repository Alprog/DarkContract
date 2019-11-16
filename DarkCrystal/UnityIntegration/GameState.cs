
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using DarkCrystal.Serialization;
using System;
using System.IO;
using UnityEngine;
using static DarkCrystal.Serialization.Serializer;

namespace DarkCrystal.UnityIntegration
{
    [StaticDarkContract]
    public class GameState : Singleton<GameState>
    {
        private bool IsStatic => Serializer.Instance.State.Settings.Flags.HasFlag(SerializationFlags.Static);
        private bool IsOnePackage => Serializer.Instance.State.Settings.Flags.HasFlag(SerializationFlags.OnePackage);

        private ForwardDefinitionList ForwardDefinitionList;

        [Key(0)]
        private ForwardDefinitionList ForwardDefinitionListSerialization
        {
            get
            {
                if (IsStatic)
                {
                    return null;////IsOnePackage ? new ForwardDefinitionList(GuidStorage<GuidObject>.Items) : null;
                }
                else
                {
                    return ForwardDefinitionList;
                }
            }
            set
            {
                if (IsStatic && IsOnePackage)
                {
                    ForwardDefinitionList = value;
                }
            }
        }

        [Key(1)]
        public GameData GameData;

        public SerializerState OriginalState;
        public byte[] OriginalBytes;

        public bool IsValid => GameData != null;

        private GameState()
        {
            Clear();
        }

        public void SaveDatabase()
        {
            SaveInitialState();
        }

        public void ReloadDatabase()
        {
#if UNITY_EDITOR
            //DataExplorer.Instance.Unselect();
#endif
            LoadInitialState();
#if UNITY_EDITOR
            Utils.RefreshEditors();          
#endif
        }
        
        public void Clear()
        {
            //GuidObject.ReleaseAll();
            ForwardDefinitionList = null;
            GameData = null;
        }

        //------------------------------------

        public void SaveInitialState()
        {
            var settings = new SerializationSettings(SerializationMode.SaveStaticFolder);
            settings.DistributedFolder = Config.StaticDistributedFolder;
            Serializer.Instance.SerializeToFile(this, Config.StaticDistributedFile, settings);
#if UNITY_EDITOR
            EditorOptions.LastDataBaseSyncTime = DateTime.UtcNow;
#endif
        }

        public void SaveInitialStateToBuildPackage()
        {
            var settings = new SerializationSettings(SerializationMode.SaveStaticPackage);
            Serializer.Instance.SerializeToFile(this, Config.StaticBuildPackageFile, settings);
        }

        public void LoadInitialState()
        {
            Clear();

            GameData = new GameData();

            byte[] bytes;
            SerializationSettings settings;

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                bytes = File.ReadAllBytes(Config.TempEditorStateFile);
                settings = new SerializationSettings(SerializationMode.LoadStaticPackage);
            }
            else
            {
                bytes = File.ReadAllBytes(Config.StaticDistributedFile);
                settings = new SerializationSettings(SerializationMode.LoadStaticFolder);
                settings.DistributedFolder = Config.StaticDistributedFolder;
                EditorOptions.LastDataBaseSyncTime = DateTime.UtcNow;
            }
#else
            bytes = File.ReadAllBytes(StaticBuildPackageFile);
            settings = new SerializationSettings(SerializationMode.LoadStaticPackage);
#endif
            Serializer.Instance.Deserialize<GameState>(bytes, settings, KeepOriginalState);
        }

        //------------------------------------

        public void SaveTempEditorState()
        {
            var settings = new SerializationSettings(SerializationMode.SaveStaticPackage);
            Serializer.Instance.SerializeToFile(this, Config.TempEditorStateFile, settings);
        }

        public void LoadTempEditorState()
        {
            try
            {   
                Clear();
                GameData = new GameData();
                var bytes = File.ReadAllBytes(Config.TempEditorStateFile);
                var settings = new SerializationSettings(SerializationMode.LoadStaticPackage);
                Serializer.Instance.Deserialize<GameState>(bytes, settings, KeepOriginalState);
            }
            catch (Exception ex)
            {
                Debug.LogError("There was error when tried to load temp editor state! StackTrace: " + ex.Message + " " + ex.StackTrace);
                LoadInitialState();
            }
        }

#if UNITY_EDITOR
        public bool CheckChangesOnDisk()
        {
            if (FileSystemTracker.Instance.HasChanges)
            {
                if (UnityEditor.EditorUtility.DisplayDialog("Database Reload", "Disk Database changed. Do you want to reload it?", "Yes", "No"))
                {
                    ReloadDatabase();
                    return true;
                }
            }

            return false;
        }
#endif

        private void KeepOriginalState(SerializerState state, byte[] bytes)
        {
            if (this.OriginalState != null)
            {
                this.OriginalState.Release();
            }
            this.OriginalState = state;
            if (this.OriginalState != null)
            {
                this.OriginalState.Retain();
            }

            this.OriginalBytes = bytes;
        }
    }
}