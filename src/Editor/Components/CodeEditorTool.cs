using System.Collections.Generic;
using System.Linq;
using Unity.CodeEditor;
using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit.Component
{
    [InitializeOnLoad]
    class CodeEditorTool : IDrawableComponent
    {
        readonly string headerText = "CodeEditor Tool";
        readonly string selectorHeaderText = "Selected";
        readonly string domainReloadBtnText = "Reload Domain";
        readonly string openProjectBtnText = "Open C# Project";
        readonly string openProjectCommandPath = "Assets/Open C# Project";

        Style style;
        GUIStyle labelStyle;
        GUILayoutOption selectorHeaderMaxWidth;

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
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(EditorStyles.label);
                selectorHeaderMaxWidth = GUILayout.MaxWidth(labelStyle.CalcSize(new GUIContent(selectorHeaderText)).x);
            }

            using (new EditorGUILayout.VerticalScope(style.Block))
            {
                EditorGUILayout.LabelField(headerText, style.H1, style.Title_H1);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(selectorHeaderText, selectorHeaderMaxWidth);
                    var currCodeEditorName = codeEditor.CurrentInstallation.Name;
                    var selectedOptionIdx = EditorGUILayout.Popup(GetSelectedEditorIndex(currCodeEditorName), editorOptions);

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
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(openProjectBtnText))
                    {
                        AssetDatabase.Refresh();
                        CodeEditor.Editor.CurrentCodeEditor.SyncAll();
                        EditorApplication.ExecuteMenuItem(openProjectCommandPath);
                    }
                    if (GUILayout.Button(domainReloadBtnText))
                    {
                        EditorUtility.RequestScriptReload();
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
            return 0;
        }

        void SetEditor(string path)
        {
            if (latestSetEditor == path) return;

            latestSetEditor = path;
            CodeEditor.SetExternalScriptEditor(path);
        }
    }
}