using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit
{
    internal interface ISubPanel
    {
        void OnEnable() { }
        void OnDisable() { }
        void OnGUI(Rect rect);
        void AddItemsToMenu(GenericMenu menu) { }
    }
}