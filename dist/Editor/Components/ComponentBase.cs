using UnityEngine;

namespace Hayson.EditorKit
{
    ///<summary> Base class for components </summary>
    public abstract class ComponentBase : ScriptableObject
    {
        // 💡 You need to add static Info property manually.
        // e.g.
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
        protected virtual void OnEndEnable() { }

        protected void OnDisable() => OnEndDisable();
        protected virtual void OnEndDisable() { }

        public abstract void OnUpdateGUI(Rect rect);

        public virtual Vector2 GetPreferSize() => Vector2.zero;
    }
}