using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit.MainWindow
{
    [Serializable]
    class PinnedPanel : ISubPanel
    {
        StyleSheet style;

        GUIStyle dotMenuBtnStyle;
        GUIContent cornerBtnGuiContent;
        Vector2 scrollPosition = Vector2.zero;

        GUILayoutOption emptyMinHeight;

        IReadOnlyList<ComponentData> comps;
        Action requestLoadDefaultComps;

        public event Action<ComponentData> OnRequestUnpinComp;
        public event Action<ComponentData> OnRequestPopupComp;

        public void Init(IReadOnlyList<ComponentData> comps, Action requestLoadDefaultComps)
        {
            this.comps = comps;
            this.requestLoadDefaultComps = requestLoadDefaultComps;
            style = StyleSheet.Instance;
        }

        void ISubPanel.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Unpin All Components"), false, UnpinAll);
            menu.AddSeparator(string.Empty);
        }

        void ISubPanel.OnEnable()
        {
            emptyMinHeight ??= GUILayout.MinHeight(0);

            foreach (var el in comps)
            {
                if (el.instanceObj != null)
                {
                    el.Component.OnEnable();
                }
            }
        }

        void ISubPanel.OnGUI(Rect rect)
        {
            ValidateStyles();

            using (var view = new EditorGUILayout.ScrollViewScope(scrollPosition, false, false))
            {
                scrollPosition = view.scrollPosition;
                if (comps.Count == 0)
                {
                    DrawEmptyCompHint();
                }
                else
                {
                    DrawComponentCard(rect);
                }
            }
        }

        void ValidateStyles()
        {
            if (dotMenuBtnStyle == null)
            {
                dotMenuBtnStyle = new("SearchModeFilter") { stretchWidth = false };
                dotMenuBtnStyle.fixedHeight = 20;
                dotMenuBtnStyle.margin = new(0, 0, 2, 0);
                dotMenuBtnStyle.padding = new(3, 3, 0, 0);
            }
            if (cornerBtnGuiContent == null)
            {
                cornerBtnGuiContent = EditorGUIUtility.IconContent("_Menu");
            }
        }

        void UnpinAll()
        {
            List<ComponentData> tempComps = new(comps);
            foreach (var el in tempComps)
            {
                OnRequestUnpinComp?.Invoke(el);
            }
        }

        void DrawEmptyCompHint()
        {
            EditorGUILayout.HelpBox("No pinned components\nYou can pin components from the [Browse] panel", MessageType.Info);
            using (new EditorGUILayout.VerticalScope(style.Block))
            {
                if (GUILayout.Button("Load preset components"))
                {
                    requestLoadDefaultComps?.Invoke();
                }
            }
        }

        void DrawComponentCard(Rect rect)
        {
            foreach (var el in comps)
            {
                if (el.instanceObj == null)
                {
                    var instanceObj = ComponentInstanceCreator.Create(el.type);
                    el.SetInstanceObj(instanceObj);
                    el.Component.OnEnable();
                }

                var elPerferSize = el.Component.GetPreferMinSize();
                var minHeightOption = elPerferSize.y > 0 ? GUILayout.MinHeight(elPerferSize.y) : emptyMinHeight;
                using (new EditorGUILayout.VerticalScope(style.Block, minHeightOption))
                {
                    DrawCompHeader(el);
                    el.Component.OnGUI(rect);
                }
            }
        }

        void DrawCompHeader(ComponentData data)
        {
            GUILayout.Space(2);
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(data.name, style.H1, style.Title_H1);

                if (GUILayout.Button(cornerBtnGuiContent, dotMenuBtnStyle))
                {
                    ShowContextMenu(data);
                }
            }
            GUILayout.Space(4);
        }

        void ShowContextMenu(ComponentData data)
        {
            GenericMenu menu = new();
            menu.AddItem(new GUIContent("Unpin"), false, () => UnpinHandler(data));
            menu.AddItem(new GUIContent("Popup"), false, () => PopupHandler(data));
            menu.ShowAsContext();

            void UnpinHandler(ComponentData data)
            {
                OnRequestUnpinComp?.Invoke(data);
            }

            void PopupHandler(ComponentData data)
            {
                OnRequestPopupComp?.Invoke(data);
            }
        }
    }
}