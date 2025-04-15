using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorKit
{
    public class ComponentContainer : EditorWindow
    {
        [MenuItem(MenuPath.Editor工具箱路徑, false, Priority.MainConsole)]
        static void CreateWindow()
        {
            var window = GetWindow<ComponentContainer>(false);
            window.titleContent = EditorGUIUtility.TrTextContent(MenuPath.Editor工具箱);
            window.minSize = new(400, 300);
        }

        static readonly HashSet<Type> registedTypes = new();
        static readonly List<IDrawableComponent> components = new();
        static readonly Style stylesheet = new();

        public static Style StyleSheet => stylesheet;

        public static void Register(Type t)
        {
            registedTypes.Add(t);
        }

        void OnEnable()
        {
            components.Clear();
            foreach (var item in registedTypes)
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

            foreach (var comp in components)
                comp.OnUpdateFrame(position);
        }
    }

    public class Style
    {
        public bool IsSetup { get; private set; }

        public GUIStyle H1 { get; private set; }
        public GUILayoutOption[] Title_H1 { get; private set; }
        public GUILayoutOption[] FieldLabel_md { get; private set; }
        public GUIStyle Block { get; private set; }
        public GUIStyle Button { get; private set; }
        public GUILayoutOption Button_md { get; private set; }
        public GUILayoutOption Button_lg { get; private set; }
        public GUILayoutOption Button_xl { get; private set; }
        public GUILayoutOption Button_min_md { get; private set; }

        public void Setup()
        {
            H1 = new(GUI.skin.label) { fontSize = 14 };
            Title_H1 = new[] { GUILayout.Height(22) };
            FieldLabel_md = new[] { GUILayout.Width(100) };
            Block = new(EditorStyles.helpBox) { padding = new RectOffset(5, 5, 5, 5) };
            Button = new(GUI.skin.button) { fontSize = 12 };
            Button_md = GUILayout.Width(100);
            Button_lg = GUILayout.Width(200);
            Button_xl = GUILayout.Width(300);
            Button_min_md = GUILayout.MinWidth(100);
            IsSetup = true;
        }
    }
}