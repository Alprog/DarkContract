
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace DarkCrystal.UnityIntegration
{
    public class GUIExpireException : System.Exception
    {
        public void Process()
        {
#if UNITY_EDITOR
            Utils.RefreshEditors();
#endif
        }
    }
}