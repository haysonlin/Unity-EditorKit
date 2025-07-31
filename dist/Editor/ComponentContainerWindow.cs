using UnityEditor;

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
            component.OnUpdateGUI(position);
        }
    }
}