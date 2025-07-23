using System;
using System.Collections.Generic;
using System.Linq;
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

        IReadOnlyList<InstanceData> comps;

        public event Action<InstanceData> OnRequestPinComp;
        public event Action<InstanceData> OnRequestUnpinComp;
        public event Action<InstanceData> OnRequestPopupComp;

        public void Init(List<ComponentConfig> configs, IReadOnlyList<InstanceData> comps)
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
            InstancePinnedComp();
        }

        public void OnDisable()
        {
            foreach (var comp in comps)
            {
                if (comp.instanceTarget != null)
                {
                    // comp.instanceTarget.OnDisable();
                }
            }
        }

        public void OnUpdateGUI(Rect rect)
        {
            ValidateStyles();

            using (var view = new EditorGUILayout.ScrollViewScope(scrollPosition, false, false))
            {
                
                scrollPosition = view.scrollPosition;
                foreach (var el in comps)
                {
                    if (el.instanceTarget == null)
                    {
                        var instanceTarget = ComponentManager.InstanceComp(el.type);
                        el.SetInstanceTarget(instanceTarget);
                        // instanceTarget.OnEnable();
                    }
                    using (new EditorGUILayout.VerticalScope(style.Block))
                    {
                        DrawCompHeader(el);
                        el.instanceTarget.OnUpdateGUI(rect);
                    }
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

        void InstancePinnedComp()
        {
            // foreach (var el in ComponentReadWriter.LoadComps())
            // {
            //     var config = configs.FirstOrDefault(item => item.type.FullName == el);
            //     if (config == null)
            //     {
            //         Debug.LogError($"Failed to find type for {el}");
            //         continue;
            //     }

            //     var instance = ComponentManager.InstanceComp(config);
            //     var instanceData = new InstanceData();
            //     instanceData.SetCompInfo(config);
            //     instanceData.SetInstanceTarget(instance);
            //     instance.OnEnable();
            // }
        }

        void UnpinAll()
        {
        }
    }
}