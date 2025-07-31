using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit
{
    internal interface ISubPanel
    {
        void OnEnable();
        void OnDisable();
        void OnUpdateGUI(Rect rect);
        void AddItemsToMenu(GenericMenu menu);
    }
}