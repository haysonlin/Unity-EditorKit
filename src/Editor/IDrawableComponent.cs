using UnityEngine;

namespace Hayson.EditorKit
{
    public interface IDrawableComponent
    {
        void OnEnable();
        void OnDisable();
        void OnUpdateFrame(Rect rect);
    }
}