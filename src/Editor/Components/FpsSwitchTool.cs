using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit.Component
{
    [InitializeOnLoad]
    // 由[李育杰]提出需求
    class FpsSwitchTool : IDrawableComponent
    {
        record Option(string Title, int Value);

        readonly string headerPrefix = "Target FPS";
        readonly string manualSettingHeaderText = "Manual";
        readonly string manualSettingBtnText = "Apply";
        readonly string setPreviousValueBtnText = "Set previous value";

        GUILayoutOption[] toggleLayoutOptions;
        GUILayoutOption manualSettingFieldHeaderMaxWidth;
        GUILayoutOption manualSettingFieldMaxWidth;

        Style style;
        GUIStyle labelStyle;

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


        static FpsSwitchTool()
        {
            ComponentContainer.Register(typeof(FpsSwitchTool));
        }

        void IDrawableComponent.OnEnable()
        {
            style = ComponentContainer.StyleSheet;
            optionsTitle = defaultOptions.Select(el => el.Title).ToArray();
            SetFps(Application.targetFrameRate);
        }

        void IDrawableComponent.OnDisable() { }

        void IDrawableComponent.OnUpdateFrame(Rect rect)
        {
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(EditorStyles.label);
                manualSettingFieldHeaderMaxWidth = GUILayout.MaxWidth(labelStyle.CalcSize(new GUIContent(manualSettingHeaderText)).x);
                manualSettingFieldMaxWidth = GUILayout.MaxWidth(labelStyle.CalcSize(new GUIContent("99999999")).x);
                toggleLayoutOptions = new GUILayoutOption[] { GUILayout.Height(22) };
            }

            using (new EditorGUILayout.VerticalScope(style.Block))
            {
                EditorGUILayout.LabelField(headerText, style.H1, style.Title_H1);

                var isUseFps = Application.targetFrameRate;
                if (isUseFps != latestSetFps)
                {
                    SetFps(isUseFps);
                }

                var selectedOptionIdx = GUILayout.Toolbar(GetSelectedIdxByTimeScale(latestSetFps), optionsTitle, toggleLayoutOptions);
                if (selectedOptionIdx != -1 && defaultOptions[selectedOptionIdx].Value != latestSetFps)
                {
                    SetFps(defaultOptions[selectedOptionIdx].Value);
                }

                GUILayout.Space(1);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(manualSettingHeaderText, manualSettingFieldHeaderMaxWidth);
                    customFps = EditorGUILayout.IntField(customFps, manualSettingFieldMaxWidth);

                    if (GUILayout.Button(manualSettingBtnText))
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
            previousValueTheaderText = $"{setPreviousValueBtnText} : {previousSetTimeScale}";

            Application.targetFrameRate = value;
            latestSetFps = value;
            headerText = $"{headerPrefix} : {(value == -1 ? "Unlimited" : value)}";
        }
    }
}