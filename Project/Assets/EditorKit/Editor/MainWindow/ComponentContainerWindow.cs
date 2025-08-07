using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit
{
    internal class ComponentContainerWindow : EditorWindow
    {
        public static ComponentContainerWindow Create(ComponentData instanceData)
        {
            var compName = instanceData.name;
            var window = CreateWindow<ComponentContainerWindow>(compName);
            window.Setup(instanceData);
            return window;
        }

        Vector2 scrollPosition = Vector2.zero;
        ScriptableObject componentObj;
        IComponent _subComponent;

        IComponent SubComponent
        {
            get
            {
                if (_subComponent == null)
                {
                    _subComponent = componentObj as IComponent;
                }
                return _subComponent;
            }
        }

        public void Setup(ComponentData compData)
        {
            componentObj = compData.instanceObj;
        }

        void OnEnable()
        {
            if (SubComponent != null)
            {
                SubComponent.OnEnable();
            }
        }

        void OnDisable()
        {
            if (SubComponent != null)
            {
                SubComponent.OnDisable();
            }
        }

        void OnDestroy()
        {
            if (componentObj != null)
            {
                DestroyImmediate(componentObj);
            }
        }

        void OnGUI()
        {
            using (var view = new EditorGUILayout.ScrollViewScope(scrollPosition, false, false))
            {
                scrollPosition = view.scrollPosition;
                SubComponent.OnGUI(position);
            }
        }
    }
}