using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit.Component
{
    class TimeScaleSwitchTool : ScriptableObject, IComponent
    {
        record Option(string Title, float Value);

        readonly string currentValueLabelPrefix = "Current";
        readonly string applyBtnText = "Apply";

        GUILayoutOption[] toggleLayoutOptions;
        GUILayoutOption applyBtnMaxWidth;
        GUILayoutOption manualValueInputerMaxWidth;

        GUIStyle labelStyle;

        string currentValueText;
        float previousSetTimeScale = 1;
        float latestSetTimeScale;
        float customTimeScale;

        readonly Option[] defaultOptions = new Option[] {
            new ("0", 0f ),
            new ("0.1", 0.1f ),
            new ("0.25", 0.25f ),
            new ("0.5", 0.5f ),
            new ("1", 1f ),
            new ("2", 2f ),
            new ("3", 3f ),
            new ("5", 5f ),
            new ("10", 10f ),
        };
        string[] optionsTitle;

        public static ComponentInfo Info => new("TimeScale Switcher")
        {
            Author = "林祐豪",
            Version = "1.0.0"
        };

        void IComponent.OnEnable()
        {
            optionsTitle = defaultOptions.Select(el => el.Title).ToArray();
            SetTimeScale(Time.timeScale);
        }

        void IComponent.OnGUI(Rect rect)
        {
            ValidateStyles();

            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(currentValueText))
                        SetTimeScale(previousSetTimeScale);

                    customTimeScale = EditorGUILayout.FloatField(customTimeScale, manualValueInputerMaxWidth);

                    if (GUILayout.Button(applyBtnText, applyBtnMaxWidth))
                        SetTimeScale(customTimeScale);
                }

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
            }
        }

        void ValidateStyles()
        {
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(EditorStyles.label);
            }
            if (applyBtnMaxWidth == null)
            {
                applyBtnMaxWidth = GUILayout.MaxWidth(labelStyle.CalcSize(new GUIContent($"__{applyBtnText}__")).x);
            }
            if (manualValueInputerMaxWidth == null)
            {
                manualValueInputerMaxWidth = GUILayout.MaxWidth(labelStyle.CalcSize(new GUIContent("__100_000__")).x);
            }
            if (toggleLayoutOptions == null)
            {
                toggleLayoutOptions = new GUILayoutOption[] { GUILayout.Height(22) };
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

            Time.timeScale = value;
            latestSetTimeScale = value;

            currentValueText = $"{currentValueLabelPrefix} : {latestSetTimeScale} (⇆ {previousSetTimeScale})";
        }
    }
}