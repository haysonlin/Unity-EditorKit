using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit.Component
{
    [InitializeOnLoad]
    class SpritePackerSwitchTool : IDrawableComponent
    {
        readonly string toolHeader = "SpritePacker Mode";
        readonly string[] optionsTitle = new string[] { "Disable", "V1", "V2" };

        Style style;
        int currStateIndex;

        static SpritePackerSwitchTool()
        {
            ComponentContainer.Register(typeof(SpritePackerSwitchTool));
        }

        void IDrawableComponent.OnEnable()
        {
            style = ComponentContainer.StyleSheet;
            currStateIndex = GetCurrentPackerStateIndex();
        }

        void IDrawableComponent.OnDisable() { }

        void IDrawableComponent.OnUpdateFrame(Rect rect)
        {
            using (new EditorGUILayout.VerticalScope(style.Block))
            {
                EditorGUILayout.LabelField(toolHeader, style.H1, style.Title_H1);

                var selectedOptionIdx = GUILayout.Toolbar(GetCurrentPackerStateIndex(), optionsTitle);

                if (selectedOptionIdx != -1 && selectedOptionIdx != currStateIndex)
                {
                    switch (selectedOptionIdx)
                    {
                        case 0: SetPackerMode(SpritePackerMode.Disabled); break;
                        case 1: SetPackerMode(SpritePackerMode.AlwaysOnAtlas); break;
                        case 2: SetPackerMode(SpritePackerMode.SpriteAtlasV2); break;
                    }
                    currStateIndex = selectedOptionIdx;
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
