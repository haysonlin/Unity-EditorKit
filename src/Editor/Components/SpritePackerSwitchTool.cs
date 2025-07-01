using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit.Component
{
    [InitializeOnLoad]
    class SpritePackerSwitchTool : IDrawableComponent
    {
        readonly string toolHeader = "SpritePacker :";
        readonly string selectorHeaderText = "Options";
        readonly string[] optionsTitle = new string[] { "Disable", "V1", "V2" };

        Style style;
        GUIStyle labelStyle;
        GUILayoutOption selectorHeaderMaxWidth;

        int currStateIndex;
        int selectedStateIndex;

        static SpritePackerSwitchTool()
        {
            ComponentConfig config = new(nameof(SpritePackerSwitchTool));
            ComponentContainer.Register<SpritePackerSwitchTool>(config);
        }

        void IDrawableComponent.OnEnable()
        {
            style = ComponentContainer.styleSheet;
            currStateIndex = GetCurrentPackerStateIndex();
        }

        void IDrawableComponent.OnDisable() { }

        void IDrawableComponent.OnUpdateFrame(Rect rect)
        {
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(EditorStyles.label);
                selectorHeaderMaxWidth = GUILayout.MaxWidth(labelStyle.CalcSize(new GUIContent(selectorHeaderText)).x);
            }

            using (new EditorGUILayout.VerticalScope(style.Block))
            {
                EditorGUILayout.LabelField($"{toolHeader} {optionsTitle[GetCurrentPackerStateIndex()]}", style.H1, style.Title_H1);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(selectorHeaderText, selectorHeaderMaxWidth);
                    selectedStateIndex = EditorGUILayout.Popup(selectedStateIndex, optionsTitle);

                    if (GUILayout.Button("Apply"))
                    {
                        currStateIndex = selectedStateIndex;
                        switch (currStateIndex)
                        {
                            case 0: SetPackerMode(SpritePackerMode.Disabled); break;
                            case 1: SetPackerMode(SpritePackerMode.AlwaysOnAtlas); break;
                            case 2: SetPackerMode(SpritePackerMode.SpriteAtlasV2); break;
                        }
                    }
                }
            }
        }

        int GetCurrentPackerStateIndex() => EditorSettings.spritePackerMode switch
        {
            SpritePackerMode.Disabled => 0,
            SpritePackerMode.AlwaysOnAtlas => 1,
            SpritePackerMode.SpriteAtlasV2 => 2,
            _ => -1
        };

        void SetPackerMode(SpritePackerMode mode)
        {
            EditorSettings.spritePackerMode = mode;
            AssetDatabase.SaveAssets();
        }
    }
}
