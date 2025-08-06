using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit
{
    internal class ComponentContainerWindow : EditorWindow
    {
        public static ComponentContainerWindow Create(InstanceData instanceData)
        {
            var compName = instanceData.name;
            var window = CreateWindow<ComponentContainerWindow>(compName);
            window.Setup(instanceData);
            return window;
        }

        Vector2 scrollPosition = Vector2.zero;
        ComponentBase component;

        public void Setup(InstanceData compData)
        {
            component = compData.instanceTarget;
        }

        void OnDestroy()
        {
            DestroyImmediate(component);
        }

        void OnGUI()
        {
            using (var view = new EditorGUILayout.ScrollViewScope(scrollPosition, false, false))
            {
                scrollPosition = view.scrollPosition;
                component.OnUpdateGUI(position);
            }
        }
    }
}