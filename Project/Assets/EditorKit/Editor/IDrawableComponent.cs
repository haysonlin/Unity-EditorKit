using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit
{
    public class ComponentBase : ScriptableObject
    {
        protected void OnEnable()
            => OnAfterEnable();

        protected void OnDisable()
            => OnAfterDisable();

        protected virtual void OnAfterEnable() { }
        protected virtual void OnAfterDisable() { }
        public virtual void OnUpdateGUI(Rect rect) { }
    }

    internal interface ISubPanel
    {
        void OnEnable();
        void OnDisable();
        void OnUpdateGUI(Rect rect);
        void AddItemsToMenu(GenericMenu menu);
    }
}