using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit.Component
{
    class SpritePackerSwitchTool : ComponentBase
    {
        readonly string[] optionsTitle = new string[] { "Disable", "V1", "V2" };

        int usingStateIndex;

        public static ComponentInfo Info => new("SpritePacker Switcher")
        {
            Author = "林祐豪",
            Version = "1.0.0"
        };

        protected override void OnEndEnable()
        {
            usingStateIndex = GetCurrentPackerStateIndex();
        }

        public override void OnUpdateGUI(Rect rect)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var tempSelectedStateIndex = GUILayout.Toolbar(usingStateIndex, optionsTitle);

                if (tempSelectedStateIndex != usingStateIndex)
                {
                    bool confirmed = EditorUtility.DisplayDialog(
                        "Confirm",
                        $"You are about to change the SpritePacker mode to [{optionsTitle[tempSelectedStateIndex]}]. Are you sure?",
                        "Confirm",
                        "Cancel"
                    );

                    if (confirmed)
                    {
                        usingStateIndex = tempSelectedStateIndex;
                        switch (usingStateIndex)
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
