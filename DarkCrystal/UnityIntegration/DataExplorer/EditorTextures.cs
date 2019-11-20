
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;

namespace DarkCrystal.UnityIntegration
{
    public class EditorTextures : Singleton<EditorTextures>
    {
        public Texture2D GreenTexture { get; private set; }
        public Texture2D PurpleTexture { get; private set; }
        public Texture2D TransparentTexture { get; private set; }
        public Texture2D GreenTextureWithBlackBorder { get; private set; }
        public Texture2D BlueTextureWithBlackBorder { get; private set; }
        public Texture2D PurpleTextureWithBlackBorder { get; private set; }
        
        private EditorTextures()
        {
            GreenTexture = Utils.MakeTexture(1, 1, new Color(0f, 0.8f, 0f, 0.5f));
            PurpleTexture = Utils.MakeTexture(1, 1, new Color(0.73f, 0.4f, 0.73f, 0.5f));
            TransparentTexture = Utils.MakeTexture(1, 1, new Color(1f, 1f, 1f, 0f));
            GreenTextureWithBlackBorder = Utils.MakeBorderedTexture(4, 4, 1, new Color(0f, 0f, 0f, 1f), new Color(0f, 0.8f, 0f, 1f));
            BlueTextureWithBlackBorder = Utils.MakeBorderedTexture(4, 4, 1, new Color(0f, 0f, 0f, 1f), new Color(0.24f, 0.48f, 0.9f, 1f));
            PurpleTextureWithBlackBorder = Utils.MakeBorderedTexture(4, 4, 1, new Color(0f, 0f, 0f, 1f), new Color(0.73f, 0.4f, 0.73f, 1f));
        }
    }
}

#endif