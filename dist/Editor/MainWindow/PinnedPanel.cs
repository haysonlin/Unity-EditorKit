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

        IReadOnlyList<InstanceData> comps;

        public event Action<InstanceData> OnRequestPinComp;
        public event Action<InstanceData> OnRequestUnpinComp;
        public event Action<InstanceData> OnRequestPopupComp;

        public void Init(IPublicFeature publicFeature, IReadOnlyList<InstanceData> comps)
        {
            this.comps = comps;
            style = StyleSheet.Instance;
        }

        void ISubPanel.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Unpin All Components"), false, UnpinAll);
            menu.AddSeparator(string.Empty);
        }

        public void OnEnable()
        {
            emptyMinHeight ??= GUILayout.MinHeight(0);
        }

        public void OnDisable() { }
        public void OnUpdateGUI(Rect rect)
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
            List<InstanceData> tempComps = new(comps);
            foreach (var el in tempComps)
            {
                OnRequestUnpinComp?.Invoke(el);
            }
        }

        void DrawEmptyCompHint()
        {
            EditorGUILayout.HelpBox("No pinned components\nYou can pin components from the [Browse] panel", MessageType.Info);
        }

        void DrawComponentCard(Rect rect)
        {
            foreach (var el in comps)
            {
                if (el.instanceTarget == null)
                {
                    var compInstance = ComponentManager.InstanceComp(el.type);
                    el.SetInstanceTarget(compInstance);
                }

                var elPerferSize = el.instanceTarget.GetPreferMinSize();
                var minHeightOption = elPerferSize.y > 0 ? GUILayout.MinHeight(elPerferSize.y) : emptyMinHeight;
                using (new EditorGUILayout.VerticalScope(style.Block, minHeightOption))
                {
                    DrawCompHeader(el);
                    el.instanceTarget.OnUpdateGUI(rect);
                }
            }
        }

        void DrawCompHeader(InstanceData data)
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

        void ShowContextMenu(InstanceData data)
        {
            GenericMenu menu = new();
            menu.AddItem(new GUIContent("Unpin"), false, () => UnpinHandler(data));
            menu.AddItem(new GUIContent("Popup"), false, () => PopupHandler(data));
            menu.ShowAsContext();

            void UnpinHandler(InstanceData data)
            {
                OnRequestUnpinComp?.Invoke(data);
            }

            void PopupHandler(InstanceData data)
            {
                OnRequestPopupComp?.Invoke(data);
            }
        }
    }
}