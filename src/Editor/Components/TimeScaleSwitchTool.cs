using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorKit.Componet
{
    [InitializeOnLoad]
    class TimeScaleSwitchTool : IDrawableComponent
    {
        record Option(string Title, float Value);

        readonly string headerPrefix = "TimeScale";
        readonly string customSettingHeaderText = "手動設定";
        readonly string customSettingButtonText = "設定";
        readonly string setPreviousValueText = "設回上次套用值";

        readonly GUILayoutOption[] quickActionBtn = new GUILayoutOption[] { GUILayout.Height(22) };
        GUILayoutOption maxWidth_4char;
        GUILayoutOption maxWidth_80px;
        readonly float fieldHeaderRightPadding = 6;
        readonly float widthPerChineseChar = 12;

        string previousValueTheaderText;
        string headerText;
        float previousSetTimeScale = 1;
        float latestSetTimeScale;
        float customTimeScale;

        readonly Option[] defaultOptions = new Option[] {
            new ("0",0f ),
            new ("0.1",0.1f ),
            new ("0.25",0.25f ),
            new ("0.5",0.5f ),
            new ("1",1f ),
            new ("2",2f ),
            new ("3",3f ),
            new ("5",5f ),
            new ("10",10f ),
        };
        string[] optionsTitle;

        Style style;

        static TimeScaleSwitchTool()
        {
            ComponentContainer.Register(typeof(TimeScaleSwitchTool));
        }

        void IDrawableComponent.OnEnable()
        {
            style = ComponentContainer.StyleSheet;
            optionsTitle = defaultOptions.Select(el => el.Title).ToArray();
            maxWidth_4char = GUILayout.MaxWidth((customSettingHeaderText.Length * widthPerChineseChar) + fieldHeaderRightPadding);
            maxWidth_80px = GUILayout.MaxWidth(80);
            SetTimeScale(Time.timeScale);
        }

        void IDrawableComponent.OnDisable() { }

        void IDrawableComponent.OnUpdateFrame(Rect rect)
        {
            using (new EditorGUILayout.VerticalScope(style.Block))
            {
                EditorGUILayout.LabelField(headerText, style.H1, style.Title_H1);

                var inUseTimeScale = Time.timeScale;
                if (inUseTimeScale != latestSetTimeScale)
                {
                    SetTimeScale(inUseTimeScale);
                }

                var selectedOptionIdx = GUILayout.Toolbar(GetSelectedIdxByTimeScale(latestSetTimeScale), optionsTitle, quickActionBtn);
                if (selectedOptionIdx != -1 && defaultOptions[selectedOptionIdx].Value != latestSetTimeScale)
                {
                    SetTimeScale(defaultOptions[selectedOptionIdx].Value);
                }

                GUILayout.Space(1);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(customSettingHeaderText, maxWidth_4char);
                    customTimeScale = EditorGUILayout.FloatField(customTimeScale, maxWidth_4char);

                    if (GUILayout.Button(customSettingButtonText, maxWidth_80px))
                        SetTimeScale(customTimeScale);

                    if (GUILayout.Button(previousValueTheaderText))
                        SetTimeScale(previousSetTimeScale);
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

        void SetTimeScale(float value)
        {
            previousSetTimeScale = latestSetTimeScale;
            previousValueTheaderText = $"{setPreviousValueText} : {previousSetTimeScale}x";

            Time.timeScale = value;
            latestSetTimeScale = value;
            headerText = $"{headerPrefix} : {value}x";
        }
    }
}