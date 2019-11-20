
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace DarkCrystal
{
    public class GUIStyles
    {
        public static Color ReadOnlyBackgroundColor = new Color(0.8f, 0.8f, 0.8f);
        public static Color DropAreaBackgroundColor = new Color(0.9f, 0.9f, 0.6f);

        public static GUIStyle Link;
        public static GUIStyle RightAlignment;
        public static GUIStyle RightAlignmentButton;
        public static GUIStyle ReadOnly;
        public static GUIStyle Panel;
        public static GUIStyle TightLabel;
        public static GUIStyle ListItem;
        public static GUIStyle BlueListItem;
        public static GUIStyle AssetPreview;
        public static GUIStyle CenteredLabel;
        public static GUIStyle BoldCenteredLabel;
        public static GUIStyle BoldLabel;
        public static GUIStyle RichLabel;
        public static GUIStyle ButtonWithLabelMarginAndPadding;
        public static GUIStyle HeightStretchButton;
        public static GUIStyle HeightStretchWithoutWidthStretchButton;
        public static GUIStyle PressedButtonGUIStyle;
        public static GUIStyle DarkGrayBackground;

        public static GUIStyle TextFieldWithWordWrap;
        public static GUIStyle TextFieldWithBoldText;

        public static Texture2D LightGrayTexture;
        public static Texture2D GrayTexture;
        public static Texture2D DarkGrayTexture;
        public static Texture2D BlueTexture;
        public static Texture2D UnityErrorIconSmall;

        static GUIStyles()
        {
            Reinit();
        }

        public static void OnGUI()
        {
            if (!GrayTexture || !BlueTexture)
            {
                Reinit();
            }
        }

        public static void Reinit()
        {
            LightGrayTexture = MakeTexture(1, 1, new Color(0.8f, 0.8f, 0.8f));
            GrayTexture = MakeTexture(1, 1, new Color(0.6f, 0.6f, 0.6f));
            DarkGrayTexture = MakeTexture(1, 1, new Color(0.2f, 0.2f, 0.2f));
            BlueTexture = MakeTexture(1, 1, new Color(0.24f, 0.48f, 0.9f));

            Link = new GUIStyle(UnityEngine.GUI.skin.label);
            Link.richText = true;

            RightAlignment = new GUIStyle(UnityEngine.GUI.skin.label);
            RightAlignment.alignment = TextAnchor.MiddleRight;

            RightAlignmentButton = new GUIStyle(UnityEngine.GUI.skin.button);
            RightAlignmentButton.alignment = TextAnchor.MiddleRight;

            ReadOnly = new GUIStyle(UnityEngine.GUI.skin.textField);
            ReadOnly.normal.background = GrayTexture;
            ReadOnly.focused.background = GrayTexture;

            DarkGrayBackground = new GUIStyle(UnityEngine.GUI.skin.box);
            DarkGrayBackground.normal.background = DarkGrayTexture;
            SetMarginAndPadding(DarkGrayBackground, 0, 0);

            Panel = new GUIStyle(UnityEngine.GUI.skin.box);
            SetMarginAndPadding(Panel, 0, 2);
            TightLabel = new GUIStyle(UnityEngine.GUI.skin.label);
            SetMarginAndPadding(TightLabel, 0, 0);

            ListItem = new GUIStyle(UnityEngine.GUI.skin.label);
            ListItem.normal.background = GrayTexture;
            SetMarginAndPadding(ListItem, 0, 1);

            BlueListItem = new GUIStyle(ListItem);
            BlueListItem.normal.textColor = Color.white;
            BlueListItem.normal.background = BlueTexture;

            AssetPreview = new GUIStyle();
            AssetPreview.normal.background = GUI.skin.box.normal.background;
            AssetPreview.border = GUI.skin.box.border;
            AssetPreview.alignment = TextAnchor.MiddleCenter;
            AssetPreview.padding = GUI.skin.box.padding;

            CenteredLabel = new GUIStyle(UnityEngine.GUI.skin.label);
            CenteredLabel.alignment = TextAnchor.MiddleCenter;

            BoldLabel = new GUIStyle(UnityEngine.GUI.skin.label);
            BoldLabel.fontStyle = FontStyle.Bold;

            RichLabel = new GUIStyle(UnityEngine.GUI.skin.label);
            RichLabel.richText = true;
            RichLabel.wordWrap = true;

            BoldCenteredLabel = new GUIStyle(BoldLabel);
            BoldCenteredLabel.alignment = TextAnchor.MiddleCenter;

            ButtonWithLabelMarginAndPadding = new GUIStyle(UnityEngine.GUI.skin.button);
            ButtonWithLabelMarginAndPadding.margin = UnityEngine.GUI.skin.label.margin;
            ButtonWithLabelMarginAndPadding.padding = UnityEngine.GUI.skin.label.padding;

            TextFieldWithWordWrap = new GUIStyle(UnityEngine.GUI.skin.textField);
            TextFieldWithWordWrap.wordWrap = true;

            TextFieldWithBoldText = new GUIStyle(UnityEngine.GUI.skin.textField);
            TextFieldWithBoldText.fontStyle = FontStyle.Bold;

            HeightStretchButton = new GUIStyle(UnityEngine.GUI.skin.button);
            HeightStretchButton.margin = new RectOffset(HeightStretchButton.margin.left, HeightStretchButton.margin.right, 1, 1);

            HeightStretchWithoutWidthStretchButton = new GUIStyle(HeightStretchButton);
            HeightStretchWithoutWidthStretchButton.stretchHeight = true;
            HeightStretchWithoutWidthStretchButton.stretchWidth = false;

            PressedButtonGUIStyle = new GUIStyle(UnityEngine.GUI.skin.button);
            PressedButtonGUIStyle.normal.background = PressedButtonGUIStyle.active.background;

#if UNITY_EDITOR
            UnityErrorIconSmall = UnityEditor.EditorGUIUtility.Load("icons/d_console.erroricon.sml.png") as Texture2D;
#endif
        }

        private static void SetMarginAndPadding(GUIStyle style, int margin, int padding)
        {
            style.margin = new RectOffset(margin, margin, margin, margin);
            style.padding = new RectOffset(padding, padding, padding, padding);
        }

        private static Texture2D MakeTexture(int width, int height, Color color)
        {
            var pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = color;
            }

            var result = new Texture2D(width, height, TextureFormat.RGBA32, false, true);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}