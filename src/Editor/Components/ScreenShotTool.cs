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
        private Camera mainCamera;
        private string path;

        static ScreenShotTool()
        {
            ComponentContainer.Register(typeof(ScreenShotTool));
        }

        void IDrawableComponent.OnEnable()
        {
            style = ComponentContainer.StyleSheet;
        }

        void IDrawableComponent.OnDisable()
        {
        }

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
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Camera", headerTextWidth);
                        mainCamera = (Camera)EditorGUILayout.ObjectField(mainCamera, typeof(Camera), true);
                    }

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
                    if (GUILayout.Button("Execute", style.Button) && mainCamera != null)
                    {
                        SaveTextureAsPNG(GetCameraTexture());
                    }

                    if (GUILayout.Button("CopyToMem", style.Button) && mainCamera != null)
                    {
                        UnityClipboardHelper.CopyTextureToClipboard(GetCameraTexture());
                    }
                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        public string SaveTextureAsPNG(Texture2D texture)
        {
            if (texture == null)
            {
                Debug.LogError("要儲存的 Texture2D 為空！");
                return string.Empty;
            }

            string fullPath = Path.Combine(Application.persistentDataPath, path);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
                Debug.Log($"已創建儲存資料夾: {fullPath}");
            }

            string filePathAndName = Path.Combine(fullPath, GetImageName());
            byte[] bytes = texture.EncodeToPNG();
            try
            {
                File.WriteAllBytes(filePathAndName, bytes);
                Debug.Log($"成功儲存圖片到: {filePathAndName}");
                return filePathAndName;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"儲存圖片失敗: {e.Message}");
                return string.Empty;
            }
        }

        private static string GetImageName()
        {
            return $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
        }

        private Texture2D GetCameraTexture()
        {
            RenderTexture rt = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, 24);
            RenderTexture originalTargetTexture = mainCamera.targetTexture;
            mainCamera.targetTexture = rt;
            mainCamera.Render();

            RenderTexture.active = rt;
            Texture2D screenshot = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            screenshot.Apply();

            mainCamera.targetTexture = originalTargetTexture;
            RenderTexture.active = null;

            rt.Release();
            UnityEngine.Object.DestroyImmediate(rt); // 在 Editor 模式下用 DestroyImmediate
            var image = screenshot;
            return image;
        }
    }
}