using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit.Component
{
    [InitializeOnLoad]
    class TimeScaleSwitchTool : IDrawableComponent
    {
        record Option(string Title, float Value);

        readonly string headerPrefix = "TimeScale";
        readonly string manualSettingHeaderText = "Manual";
        readonly string manualSettingBtnText = "Apply";
        readonly string setPreviousValueBtnText = "Set previous value";

        GUILayoutOption[] toggleLayoutOptions;
        GUILayoutOption manualSettingFieldHeaderMaxWidth;
        GUILayoutOption manualSettingFieldMaxWidth;

        Style style;
        GUIStyle labelStyle;

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

        static TimeScaleSwitchTool()
        {
            ComponentContainer.Register(typeof(TimeScaleSwitchTool));
        }

        void IDrawableComponent.OnEnable()
        {
            style = ComponentContainer.StyleSheet;
            optionsTitle = defaultOptions.Select(el => el.Title).ToArray();
            SetTimeScale(Time.timeScale);
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

                var inUseTimeScale = Time.timeScale;
                if (inUseTimeScale != latestSetTimeScale)
                {
                    SetTimeScale(inUseTimeScale);
                }

                var selectedOptionIdx = GUILayout.Toolbar(GetSelectedIdxByTimeScale(latestSetTimeScale), optionsTitle, toggleLayoutOptions);
                if (selectedOptionIdx != -1 && defaultOptions[selectedOptionIdx].Value != latestSetTimeScale)
                {
                    SetTimeScale(defaultOptions[selectedOptionIdx].Value);
                }

                GUILayout.Space(1);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(manualSettingHeaderText, manualSettingFieldHeaderMaxWidth);
                    customTimeScale = EditorGUILayout.FloatField(customTimeScale, manualSettingFieldMaxWidth);

                    if (GUILayout.Button(manualSettingBtnText))
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
            previousValueTheaderText = $"{setPreviousValueBtnText} : {previousSetTimeScale}";

            Time.timeScale = value;
            latestSetTimeScale = value;
            headerText = $"{headerPrefix} : {value}";
        }
    }
}