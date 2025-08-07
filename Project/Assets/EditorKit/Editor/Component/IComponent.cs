using UnityEngine;

namespace Hayson.EditorKit
{
    ///<summary>
    /// <para>📢 NOTICE !</para>
    /// <para>Some rules for component, please read the following in the source code</para>
    ///</summary>
    public interface IComponent
    {
        
        // 📢 NOTICE !
        // 1. Your component needs to inherit from ScriptableObject.
        // 2. You need to add static Info property manually.
        //    The Info property is used to provide information about the component.
        //    You can use the following template to add the Info property:
#if false
        public static ComponentInfo Info => new("Component Name")
        {
            Description = "Description...",
            Author = "Author Name",
            Version = "1.0.0",
            ReadmePath = "https://readme.com"
        };
#endif

        void OnInstance() { }

        void OnEnable() { }

        void OnDisable() { }

        void OnGUI(Rect rect);

        ///<summary> Get the perfer minimum size of the component </summary>
        Vector2 GetPreferMinSize() => Vector2.zero;
    }
}