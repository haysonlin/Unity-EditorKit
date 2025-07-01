using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit
{
    class ComponentStore
    {
        static readonly List<Type> compsType = new();
        static readonly List<ComponentConfig> compsConfig = new();
        readonly List<IDrawableComponent> comps = new();

        public IReadOnlyList<Type> CompsType => compsType;
        public IReadOnlyList<IDrawableComponent> Comps => comps;

        public void Clear()
        {
            compsType.Clear();
            compsConfig.Clear();
            comps.Clear();
        }

        public void RegisterComp<T>(ComponentConfig config) where T : IDrawableComponent
        {
            if (compsType.Contains(typeof(T)))
            {
                Debug.LogError($"Component {typeof(T)} is already registered");
                return;
            }

            compsType.Add(typeof(T));
            compsConfig.Add(config);
        }

        public void InstanceComp(IDrawableComponent comp)
        {
            comps.Add(comp);
        }
    }

    public class ComponentContainer : EditorWindow, IHasCustomMenu
    {
        [MenuItem(MenuPath.EditorKitMenuPath, false, Priority.MainPanel)]
        static void CreateWindow()
        {
            var window = GetWindow<ComponentContainer>(false);
            window.titleContent = EditorGUIUtility.TrTextContent(MenuPath.EditorKitDisplayName);
            window.minSize = new(400, 300);
        }

        public static readonly Style styleSheet = new();
        static readonly ComponentStore useComponentStore = new();

        public static void Register<T>(ComponentConfig config) where T : IDrawableComponent
        {
            useComponentStore.RegisterComp<T>(config);
        }

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("🌎 Open repository in GitHub"), false, () => Application.OpenURL(Config.RepositoryUrl));
        }

        Vector2 scrollPosition = Vector2.zero;

        void OnEnable()
        {
            useComponentStore.Clear();

            // 預設工具集 
            // >Todo: 應由使用者自定義，相關功能待處理
            var defaultComps = new Type[] {
                typeof(Component.TimeScaleSwitchTool),
                typeof(Component.FpsSwitchTool),
                typeof(Component.SpritePackerSwitchTool),
                typeof(Component.CodeEditorTool),
            };
            InstanceComps(defaultComps);

            var customComps = useComponentStore.CompsType.Except(defaultComps);
            InstanceComps(customComps);

            foreach (var comp in useComponentStore.Comps)
            {
                comp.OnEnable();
            }
        }

        void OnDisable()
        {
            foreach (var comp in useComponentStore.Comps)
            {
                comp.OnDisable();
            }
            useComponentStore.Clear();
        }

        void OnGUI()
        {
            if (styleSheet.IsSetup == false)
            {
                styleSheet.Setup();
            }

            // >Todo: 改為由主容器統一繪製子容器框與標題
            using (var view = new EditorGUILayout.ScrollViewScope(scrollPosition, false, false))
            {
                scrollPosition = view.scrollPosition;
                using (new EditorGUILayout.VerticalScope())
                {
                    foreach (var comp in useComponentStore.Comps)
                    {
                        comp.OnUpdateFrame(position);
                    }
                }
            }
        }

        void InstanceComps(IEnumerable<Type> compList)
        {
            foreach (var el in compList)
            {
                if (Activator.CreateInstance(el) is IDrawableComponent instance)
                {
                    useComponentStore.InstanceComp(instance);
                }
                else
                {
                    Debug.LogError($"Failed to create instance of {el}");
                }
            }
        }
    }
}