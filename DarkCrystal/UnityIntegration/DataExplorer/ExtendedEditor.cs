
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR

using UnityEditor;

namespace DarkCrystal.UnityIntegration
{
    public abstract class ExtendedEditor<T> : Editor where T : class
    {
        public T Target
        {
            get
            {
                return base.target as T;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            try
            {
                OnGUI();
            }
            catch (GUIExpireException exception)
            {
                exception.ToString();
                Utils.RefreshEditors();
            }
        }

        protected abstract void OnGUI();
    }
}

#endif