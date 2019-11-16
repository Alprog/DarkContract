
#if UNITY_EDITOR

using System;
using UnityEditorInternal;

namespace DarkCrystal.Encased.Core.Startup
{
    public static class EditorFocusHandler
    {
        public static bool IsFocused { get; private set; }
        private static bool CurrentFocusState => InternalEditorUtility.isApplicationActive;

        public static event Action<bool> OnFocusStateChanged;

        static EditorFocusHandler()
        {
            IsFocused = CurrentFocusState;
            UnityEditor.EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            var newFocusState = CurrentFocusState;
            if (newFocusState != IsFocused)
            {
                IsFocused = newFocusState;
                OnFocusStateChanged?.Invoke(newFocusState);
            }
        }

    }
}

#endif