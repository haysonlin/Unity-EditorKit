using UnityEngine;

namespace EditorKit
{
    public interface IDrawableComponent
    {
        void OnEnable();
        void OnDisable();
        void OnUpdateFrame(Rect rect);
    }
}