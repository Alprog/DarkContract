
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;
using UnityScene = UnityEngine.SceneManagement.Scene;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace DarkCrystal.UnityIntegration
{
    /*
     * This class is used to localize and wrap dissipated situations that occurs with scripts during Unity editor lifetime.
     * 
     * It contains entry point of all scripts. At startup it tries to figurate current mode and do some corresponding actions, such as 
     * restoring game state or changing scene. It also listen various events to be able serialize current state before it destruction and
     * make some clues about current mode for future startups.
     */

    [InitializeOnLoad]
    public static class EditorStartup
    {
        public static bool IsCompiling { get; private set; }
        
        static EditorStartup()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.update += OnEditorUpdate;
            EditorApplication.quitting += OnEditorQuitting;
            AssemblyReloadEvents.beforeAssemblyReload += BeforeReloadAssembly;
            EditorFocusHandler.OnFocusStateChanged += CheckDatabaseForReload;

            var startupType = GetStartupType();
            SetStartupType(StartupType.Unknown);

            try
            {
                switch (startupType)
                {
                    case StartupType.EditorNormal:
                        GameState.Instance.LoadInitialState();
                        break;

                    case StartupType.EditorHotReload:
                        if (IsTempMemoryKeptOnHotReload())
                        {
                            GameState.Instance.LoadTempEditorState();
                        }
                        else
                        {
                            GameState.Instance.LoadInitialState();
                        }
                        break;

                    case StartupType.PlayTest:
                        UnitySceneManager.sceneLoaded += OnPlayTestSceneLoaded;
                        break;

                    default:
                        throw new System.Exception("Unknown startup type");
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                Debug.LogError("Can't startup properly");
            }
            FileSystemTracker.Instance.Run(Config.StaticDistributedFolder);
        }

        private static StartupType GetStartupType()
        {
            return (StartupType)PlayerPrefs.GetInt("StartupType");
        }

        private static void SetStartupType(StartupType type)
        {
            PlayerPrefs.SetInt("StartupType", (int)type);
        }

        private static void CheckDatabaseForReload(bool focus)
        {
            if (focus && !EditorApplication.isCompiling && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                GameState.Instance.CheckChangesOnDisk();
            }
        }

        private static void OnEditorUpdate()
        {
            IsCompiling = EditorApplication.isCompiling;
        }

        private static void BeforeReloadAssembly()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode && GameState.Instance.IsValid)
            {
                GameState.Instance.SaveTempEditorState();
                SetStartupType(StartupType.EditorHotReload);
            }
        }

        private static void OnPlayModeChanged(PlayModeStateChange @object)
        {
            if (@object == PlayModeStateChange.ExitingEditMode)
            {
                GameState.Instance.SaveTempEditorState();
                SetStartupType(StartupType.PlayTest);
            }
            else if (@object == PlayModeStateChange.EnteredEditMode)
            {
                GameState.Instance.Clear();
                GameState.Instance.LoadTempEditorState();
            }
        }

        private static void OnEditorQuitting()
        {
            GameState.Instance.SaveTempEditorState();
        }

        private static void OnPlayTestSceneLoaded(UnityScene playTestScene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            UnitySceneManager.sceneLoaded -= OnPlayTestSceneLoaded;
            Debug.Log("Started");
        }

        private static bool IsTempMemoryKeptOnHotReload()
        {
            var directoryTime = Utils.GetLastWriteTimeInDirectory(Config.StaticDistributedFolder);
            return EditorOptions.LastDataBaseSyncTime >= directoryTime;
        }
    }
}

#endif