using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EditorKit.Component
{
    [InitializeOnLoad]
    // 由[李家駿]製作
    class ScreenShotTool : IDrawableComponent
    {
        Style style;

        private string path;

        static ScreenShotTool()
        {
            ComponentContainer.Register(typeof(ScreenShotTool));
        }

        void IDrawableComponent.OnEnable()
        {
            style = ComponentContainer.StyleSheet;
        }

        void IDrawableComponent.OnDisable() { }

        void IDrawableComponent.OnUpdateFrame(Rect rect)
        {
            GUILayout.BeginVertical(style.Block);
            {
                EditorGUILayout.LabelField("截圖工具", style.H1, style.Title_H1);

                GUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("儲存路徑", GUILayout.Width(54));
                        path = EditorGUILayout.TextField(path);

                        if (GUILayout.Button("選擇", style.Button_md))
                        {
                            path = EditorUtility.OpenFolderPanel("選擇儲存路徑", path, "");
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("一鍵截圖", style.Button))
                    {
                        var imagePath = Path.Combine(path, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");
                        ScreenCapture.CaptureScreenshot(imagePath);
                        Debug.Log($"截圖儲存至 : {imagePath}");
                    }
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}