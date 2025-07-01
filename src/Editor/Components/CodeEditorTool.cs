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
        readonly string headerText = "CodeEditor";
        readonly string selectorHeaderText = "Options";
        readonly string domainReloadBtnText = "Reload Domain";
        readonly string openProjectBtnText = "Open C# Project";
        readonly string openProjectCommandPath = "Assets/Open C# Project";

        string headerTextWithEditorName;

        Style style;
        GUIStyle labelStyle;
        GUILayoutOption selectorHeaderMaxWidth;

        CodeEditor codeEditor;
        string[] editorOptionsPath;
        string[] editorOptionsName;

        int selectedOptionIdx;

        static CodeEditorTool()
        {
            ComponentConfig config = new(nameof(CodeEditorTool));
            ComponentContainer.Register<CodeEditorTool>(config);
        }

        void IDrawableComponent.OnEnable()
        {
            style = ComponentContainer.styleSheet;
            codeEditor = CodeEditor.Editor;
            var editorOptionsNameDict = codeEditor.GetFoundScriptEditorPaths();
            editorOptionsPath = editorOptionsNameDict.Keys.ToArray();
            editorOptionsName = editorOptionsNameDict.Values.ToArray();
            SetEditor(GetEditorIndexByName(codeEditor.CurrentInstallation.Name));
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
                EditorGUILayout.LabelField(headerTextWithEditorName, style.H1, style.Title_H1);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(selectorHeaderText, selectorHeaderMaxWidth);
                    selectedOptionIdx = EditorGUILayout.Popup(selectedOptionIdx, editorOptionsName);

                    if (GUILayout.Button("Apply"))
                    {
                        SetEditor(selectedOptionIdx);
                    }
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(openProjectBtnText))
                    {
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

        int GetEditorIndexByName(string editorName)
        {
            for (int i = 0; i < editorOptionsName.Length; i++)
            {
                if (editorOptionsName[i] == editorName)
                    return i;
            }
            return -1;
        }

        void SetEditor(int selectedOptionIdx)
        {
            if (selectedOptionIdx == -1 || selectedOptionIdx >= editorOptionsName.Length)
            {
                headerTextWithEditorName = $"{headerText} : Missing";
            }
            else
            {
                var targetName = editorOptionsName[selectedOptionIdx];
                var targetPath = editorOptionsPath[selectedOptionIdx];
                headerTextWithEditorName = $"{headerText} : {targetName}";
                CodeEditor.SetExternalScriptEditor(targetPath);
            }
            this.selectedOptionIdx = selectedOptionIdx;
        }
    }
}