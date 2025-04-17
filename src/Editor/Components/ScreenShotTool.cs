using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit.Component
{
    [InitializeOnLoad]
    // 由[李家駿]製作
    class ScreenShotTool : IDrawableComponent
    {
        Style style;
        GUIStyle labelStyle;

        GUILayoutOption headerTextWidth;

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
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(EditorStyles.label);
                headerTextWidth = GUILayout.MaxWidth(labelStyle.CalcSize(new GUIContent("Save Path")).x);
            }

            GUILayout.BeginVertical(style.Block);
            {
                EditorGUILayout.LabelField("GameView Screenshot Tool", style.H1, style.Title_H1);
                GUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Save Path", headerTextWidth);
                        path = EditorGUILayout.TextField(path);

                        if (GUILayout.Button("Select", style.Button_md))
                        {
                            path = EditorUtility.OpenFolderPanel("Select Save Path", path, "");
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Execute", style.Button))
                    {
                        var imagePath = Path.Combine(path, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");
                        ScreenCapture.CaptureScreenshot(imagePath);
                        Debug.Log($"Screenshot saved to : {imagePath}");
                    }
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}