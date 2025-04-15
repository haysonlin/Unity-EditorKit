using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorKit.Component
{
    [InitializeOnLoad]
    // 由[李育杰]提出需求
    class FpsSwitchTool : IDrawableComponent
    {
        record Option(string Title, int Value);

        readonly string headerPrefix = "FPS";
        readonly string customSettingHeaderText = "手動設定";
        readonly string customSettingButtonText = "設定";
        readonly string setPreviousValueText = "設回上次套用值";

        readonly GUILayoutOption[] style_QuickActionBtn = new GUILayoutOption[] { GUILayout.Height(22) };
        GUILayoutOption maxWidth_4char;
        GUILayoutOption maxWidth_80px;
        readonly float fieldHeaderRightPadding = 6;
        readonly float widthPerChineseChar = 12;

        string previousValueTheaderText;
        string headerText = string.Empty;
        int previousSetTimeScale = 1;
        int latestSetFps;
        int customFps;

        readonly Option[] defaultOptions = new Option[] {
            new ( "0", 0 ),
            new ( "30", 30 ),
            new ( "60", 60 ),
            new ( "120", 120 ),
            new ( "240", 240 ),
        };
        string[] optionsTitle;

        Style style;

        static FpsSwitchTool()
        {
            ComponentContainer.Register(typeof(FpsSwitchTool));
        }

        void IDrawableComponent.OnEnable()
        {
            style = ComponentContainer.StyleSheet;
            optionsTitle = defaultOptions.Select(el => el.Title).ToArray();
            maxWidth_4char = GUILayout.MaxWidth((customSettingHeaderText.Length * widthPerChineseChar) + fieldHeaderRightPadding);
            maxWidth_80px = GUILayout.MaxWidth(80);
            SetFps(Application.targetFrameRate);
        }

        void IDrawableComponent.OnDisable() { }

        void IDrawableComponent.OnUpdateFrame(Rect rect)
        {
            using (new EditorGUILayout.VerticalScope(style.Block))
            {
                EditorGUILayout.LabelField(headerText, style.H1, style.Title_H1);

                var isUseFps = Application.targetFrameRate;
                if (isUseFps != latestSetFps)
                {
                    SetFps(isUseFps);
                }

                var selectedOptionIdx = GUILayout.Toolbar(GetSelectedIdxByTimeScale(latestSetFps), optionsTitle, style_QuickActionBtn);
                if (selectedOptionIdx != -1 && defaultOptions[selectedOptionIdx].Value != latestSetFps)
                {
                    SetFps(defaultOptions[selectedOptionIdx].Value);
                }

                GUILayout.Space(1);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(customSettingHeaderText, maxWidth_4char);
                    customFps = EditorGUILayout.IntField(customFps, maxWidth_4char);

                    if (GUILayout.Button(customSettingButtonText, maxWidth_80px))
                        SetFps(customFps);

                    if (GUILayout.Button(previousValueTheaderText))
                        SetFps(previousSetTimeScale);
                }
            }
        }

        int GetSelectedIdxByTimeScale(float timeScale)
        {
            for (int i = 0; i < defaultOptions.Length; i++)
            {
                if (defaultOptions[i].Value == timeScale)
                    return i;
            }
            return -1;
        }

        void SetFps(int value)
        {
            previousSetTimeScale = latestSetFps;
            previousValueTheaderText = $"{setPreviousValueText} : {previousSetTimeScale}x";

            Application.targetFrameRate = value;
            latestSetFps = value;
            headerText = $"{headerPrefix} : {(value == -1 ? "無限" : value)}";
        }
    }
}