using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hayson.EditorKit
{
    static internal class ComponentInstanceCreator
    {
        public static ScriptableObject Create(Type type)
        {
            if (
                typeof(ScriptableObject).IsAssignableFrom(type) &&
                typeof(IComponent).IsAssignableFrom(type)
                )
            {
                var instance = ScriptableObject.CreateInstance(type);
                instance.hideFlags = HideFlags.HideAndDontSave;

                var comp = instance as IComponent;
                comp.OnInstance();

                return instance;
            }
            else
            {
                Debug.LogError($"Failed to create instance of {type.FullName}");
                return null;
            }
        }
    }
}