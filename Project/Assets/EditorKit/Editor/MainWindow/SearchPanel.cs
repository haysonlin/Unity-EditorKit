using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Hayson.EditorKit.MainWindow
{
    [Serializable]
    class SearchPanel : ISubPanel
    {
        ComponentManager compMgr;
        ISearcher searcher;
        StyleSheet style;

        readonly Color gray75 = new(0.75f, 0.75f, 0.75f, 1);
        GUIStyle labelStyle;

        Texture2D infoIcon;
        Texture2D openIcon;
        Texture2D keepIcon;

        GUIContent infoIconContent;
        GUIContent openIconContent;
        GUIContent keepIconContent;

        // 工作階段數據
        [SerializeField] string searchingString = "";
        IEnumerable<ComponentConfig> searchResult = Array.Empty<ComponentConfig>();
        Vector2 scrollPosition = Vector2.zero;

        public event Action<ComponentConfig> OnRequestPinComp;
        public event Action<ComponentConfig> OnRequestOpenComp;

        void ISubPanel.AddItemsToMenu(GenericMenu menu)
        {
        }

        public void Init(ComponentManager compMgr)
        {
            this.compMgr = compMgr;
            searcher = SearcherManager.GetSearcher();
            searchResult = compMgr.CompsConfigs;
        }

        public void OnEnable()
        {
            style = StyleSheet.Instance;

            infoIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/EditorKit/Editor/Icons/info_16dp.png");
            openIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/EditorKit/Editor/Icons/open_in_new_16dp.png");
            keepIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/EditorKit/Editor/Icons/keep_16dp.png");

            infoIconContent = new GUIContent(infoIcon, "Open help");
            openIconContent = new GUIContent(openIcon, "Open in new window");
            keepIconContent = new GUIContent(keepIcon, "Pin it");
        }

        public void OnDisable()
        {
        }

        public void OnUpdateGUI(Rect rect)
        {
            ValidateStyles();
            DrawSearchBar();

            using (var view = new EditorGUILayout.ScrollViewScope(scrollPosition, false, false))
            {
                scrollPosition = view.scrollPosition;
                foreach (var el in searchResult)
                {
                    DrawComponentCard(el);
                }
            }
        }

        void ValidateStyles()
        {
            if (labelStyle == null)
            {
                labelStyle = new GUIStyle(EditorStyles.label);
            }
        }

        void DrawSearchBar()
        {
            EditorGUI.BeginChangeCheck();
            searchingString = EditorGUILayout.TextField(string.Empty, searchingString);
            if (EditorGUI.EndChangeCheck())
            {
                var tempSearchString = searchingString;
                if (string.IsNullOrEmpty(tempSearchString))
                {
                    searchResult = compMgr.CompsConfigs;
                }
                else
                {
                    searcher.Search(tempSearchString, result =>
                    {
                        searchResult = result.Select(el => compMgr.CompsConfigs.First(item => item.type.FullName == el)).ToList();
                    });
                }
            }
        }

        void DrawComponentCard(ComponentConfig compConfig)
        {
            var info = compConfig.info;

            using (new EditorGUILayout.VerticalScope(style.Block))
            {
                EditorGUILayout.LabelField(info.Name, style.H1, style.Title_H1);

                if (!string.IsNullOrEmpty(info.Description))
                {
                    EditorGUILayout.LabelField(info.Description, EditorStyles.wordWrappedLabel);
                }

                Color originalColor = GUI.contentColor;
                GUI.contentColor = gray75;
                EditorGUILayout.LabelField($"Author  : {info.Author}", GUILayout.ExpandWidth(false));
                EditorGUILayout.LabelField($"Version : {info.Version}", GUILayout.ExpandWidth(false));
                GUI.contentColor = originalColor;

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    GUI.enabled = !string.IsNullOrEmpty(info.ReadmePath);
                    if (GUILayout.Button(infoIconContent, GUILayout.Width(36), GUILayout.ExpandWidth(false)))
                    {
                        Application.OpenURL(info.ReadmePath);
                    }
                    GUI.enabled = true;

                    if (GUILayout.Button(openIconContent, GUILayout.Width(36), GUILayout.ExpandWidth(false)))
                    {
                        OnRequestOpenComp?.Invoke(compConfig);
                    }

                    if (GUILayout.Button(keepIconContent, GUILayout.Width(36), GUILayout.ExpandWidth(false)))
                    {
                        OnRequestPinComp?.Invoke(compConfig);
                    }
                }
            }
        }
    }
}