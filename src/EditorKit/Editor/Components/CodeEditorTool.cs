using System.Collections.Generic;
using System.Linq;
using Unity.CodeEditor;
using UnityEditor;
using UnityEngine;

namespace EditorKit.Component
{
    [InitializeOnLoad]
    class CodeEditorTool : IDrawableComponent
    {
        readonly string headerText = "CodeEditor Switcher";

        Style style;
        string[] editorOptions = null;
        Dictionary<string, string> editorOptionsDict = null;
        CodeEditor codeEditor = null;
        string latestSetEditor = string.Empty;

        static CodeEditorTool()
        {
            ComponentContainer.Register(typeof(CodeEditorTool));
        }

        void IDrawableComponent.OnEnable()
        {
            style = ComponentContainer.StyleSheet;
            codeEditor = CodeEditor.Editor;
            editorOptionsDict = codeEditor.GetFoundScriptEditorPaths();
            editorOptions = editorOptionsDict.Values.ToArray();
        }

        void IDrawableComponent.OnDisable() { }

        void IDrawableComponent.OnUpdateFrame(Rect rect)
        {
            using (new EditorGUILayout.VerticalScope(style.Block))
            {
                EditorGUILayout.LabelField(headerText, style.H1, style.Title_H1);

                var currCodeEditorName = codeEditor.CurrentInstallation.Name;
                var selectedOptionIdx = EditorGUILayout.Popup("選擇編輯器 :", GetSelectedEditorIndex(currCodeEditorName), editorOptions);

                if (selectedOptionIdx != -1 && editorOptions[selectedOptionIdx] != latestSetEditor)
                {
                    var paths = editorOptionsDict.Keys;
                    var i = 0;
                    foreach (var path in paths)
                    {
                        if (i == selectedOptionIdx)
                        {
                            SetEditor(path);
                            break;
                        }
                        i++;
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("重載腳本域"))
                    {
                        EditorUtility.RequestScriptReload();
                    }
                    if (GUILayout.Button("重建專案文件"))
                    {
                        AssetDatabase.Refresh();
                        CodeEditor.Editor.CurrentCodeEditor.SyncAll();
                    }
                    if (GUILayout.Button("開啟C#專案"))
                    {
                        EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
                    }
                }


            }
        }

        int GetSelectedEditorIndex(string selectedEditorPath)
        {
            for (int i = 0; i < editorOptions.Length; i++)
            {
                if (editorOptions[i] == selectedEditorPath)
                    return i;
            }
            return -1;
        }

        void SetEditor(string path)
        {
            if (latestSetEditor == path) return;

            latestSetEditor = path;
            CodeEditor.SetExternalScriptEditor(path);
        }
    }
}