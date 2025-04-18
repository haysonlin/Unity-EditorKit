using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit
{
    public class ComponentContainer : EditorWindow, IHasCustomMenu
    {
        [MenuItem(MenuPath.EditorKitMenuPath, false, Priority.MainPanel)]
        static void CreateWindow()
        {
            var window = GetWindow<ComponentContainer>(false);
            window.titleContent = EditorGUIUtility.TrTextContent(MenuPath.EditorKitDisplayName);
            window.minSize = new(400, 300);
        }

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Open repository in GitHub"), false, () =>
            {
                Application.OpenURL(Config.RepositoryUrl);
            });
        }

        static readonly HashSet<Type> registedTypes = new();
        static readonly List<IDrawableComponent> components = new();
        static readonly Style stylesheet = new();

        Vector2 scrollPosition = Vector2.zero;

        public static Style StyleSheet => stylesheet;

        public static void Register(Type t)
        {
            registedTypes.Add(t);
        }

        void OnEnable()
        {
            components.Clear();

            // 預設工具集 
            // >Todo: 應由使用者自定義，相關功能待處理
            var defaultComps = new Type[] {
                typeof(Component.SpritePackerSwitchTool),
                typeof(Component.TimeScaleSwitchTool),
                typeof(Component.FpsSwitchTool),
                typeof(Component.CodeEditorTool),
                typeof(Component.ScreenShotTool),
            };
            foreach (var item in defaultComps)
            {
                var instance = (IDrawableComponent)Activator.CreateInstance(item);

                if (instance != null)
                {
                    components.Add(instance);
                    instance.OnEnable();
                }
                else
                {
                    Debug.LogError($"Failed to create instance of {item}");
                }
            }

            var customComps = registedTypes.Except(defaultComps);
            foreach (var item in customComps)
            {
                var instance = (IDrawableComponent)Activator.CreateInstance(item);

                if (instance != null)
                {
                    components.Add(instance);
                    instance.OnEnable();
                }
                else
                {
                    Debug.LogError($"Failed to create instance of {item}");
                }
            }
        }

        void OnDisable()
        {
            foreach (var comp in components)
            {
                comp.OnDisable();
            }
            components.Clear();
        }

        void OnGUI()
        {
            if (stylesheet.IsSetup == false)
            {
                stylesheet.Setup();
            }

            using (var view = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = view.scrollPosition;
                foreach (var comp in components)
                {
                    comp.OnUpdateFrame(position);
                }
            }
        }
    }
}