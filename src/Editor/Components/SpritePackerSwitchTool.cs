using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit.Component
{
    [InitializeOnLoad]
    class SpritePackerSwitchTool : IDrawableComponent
    {
        readonly string[] toolHeader = new string[] { "SpritePacker : On", "SpritePacker : Off", };
        readonly string[] optionsTitle = new string[] { "Enable", "Disable", };

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
                EditorGUILayout.LabelField(GetToolHeader(), style.H1, style.Title_H1);

                var selectedOptionIdx = GUILayout.Toolbar(GetCurrentPackerStateIndex(), optionsTitle);

                if (selectedOptionIdx != -1 && selectedOptionIdx != currStateIndex)
                {
                    switch (selectedOptionIdx)
                    {
                        case 0: EnablePacker(); break;
                        case 1: DisablePacker(); break;
                    }
                    currStateIndex = selectedOptionIdx;
                }
            }
        }

        string GetToolHeader() => currStateIndex switch
        {
            0 => toolHeader[0],
            _ => toolHeader[1],
        };

        int GetCurrentPackerStateIndex() => EditorSettings.spritePackerMode switch
        {
            SpritePackerMode.AlwaysOnAtlas => 0,
            SpritePackerMode.Disabled => 1,
            _ => -1
        };

        void EnablePacker()
        {
            EditorSettings.spritePackerMode = SpritePackerMode.AlwaysOnAtlas;
            AssetDatabase.SaveAssets();
        }

        void DisablePacker()
        {
            EditorSettings.spritePackerMode = SpritePackerMode.Disabled;
            AssetDatabase.SaveAssets();
        }
    }
}
