using UnityEngine;

namespace Hayson.EditorKit
{
    ///<summary> Base class for components </summary>
    public abstract class ComponentBase : ScriptableObject
    {
        // 💡 You need to add static Info property manually.
        // The Info property is used to provide information about the component. 
        // You can use the following template to add the Info property:
#if false
        public static ComponentInfo Info => new("Component Name")
        {
            Description = "Description...",
            Author = "Author Name",
            Version = "1.0.0",
            ReadmePath = "https://readme.com"
        };
#endif

        protected void OnEnable() => OnEndEnable();
        protected void OnDisable() => OnEndDisable();

        protected virtual void OnEndEnable() { }

        protected virtual void OnEndDisable() { }

        public abstract void OnUpdateGUI(Rect rect);

        ///<summary> Get the perfer minimum size of the component </summary>
        public virtual Vector2 GetPreferMinSize() => Vector2.zero;
    }
}