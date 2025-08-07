using System;
using UnityEngine;

namespace Hayson.EditorKit
{
    [Serializable]
    public class ComponentData
    {
        [field: SerializeField] public string typeFullName { get; private set; }
        [field: SerializeField] public string name { get; private set; }
        [field: SerializeField] public string description { get; private set; }
        [field: SerializeField] public string author { get; private set; }
        [field: SerializeField] public string version { get; private set; }
        [field: SerializeField] public string readmePath { get; private set; }
        [field: SerializeField] public ScriptableObject instanceObj { get; private set; }

        IComponent _component;
        Type _type;

        public IComponent Component
        {
            get
            {
                if (_component == null)
                {
                    _component = instanceObj as IComponent;
                }
                return _component;
            }
        }

        public Type type
        {
            get
            {
                if (_type == null)
                {
                    _type = Type.GetType(typeFullName);
                }
                return _type;
            }
        }

        public void SetCompInfo(ComponentConfig compConfig)
        {
            _type = compConfig.type;
            typeFullName = compConfig.type.FullName;
            name = compConfig.info.Name;
            description = compConfig.info.Description;
            author = compConfig.info.Author;
            version = compConfig.info.Version;
            readmePath = compConfig.info.ReadmePath;
        }

        public void SetInstanceObj(ScriptableObject instanceObj)
        {
            this.instanceObj = instanceObj;
        }
    }
}