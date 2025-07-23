using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Hayson.EditorKit
{
    [Serializable]
    public class ComponentConfig
    {
        public Type type;
        [field: SerializeField] public ComponentInfo info { get; set; }
    }

    [Serializable]
    public class InstanceData
    {
        [field: SerializeField] public string typeFullName { get; private set; }
        [field: SerializeField] public string name { get; private set; }
        [field: SerializeField] public string description { get; private set; }
        [field: SerializeField] public string author { get; private set; }
        [field: SerializeField] public string version { get; private set; }
        [field: SerializeField] public string readmePath { get; private set; }
        [field: SerializeField] public ComponentBase instanceTarget { get; private set; }

        Type _type;

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

        public void SetInstanceTarget(ComponentBase instanceTarget)
        {
            this.instanceTarget = instanceTarget;
        }
    }

    [Serializable]
    class ComponentManager
    {
        IReadOnlyList<ComponentConfig> compConfigs;
        List<InstanceData> comps = new();

        public IReadOnlyList<InstanceData> Comps => comps;
        public IReadOnlyList<ComponentConfig> CompsConfigs => compConfigs;

        public void Setup(IReadOnlyList<ComponentConfig> compConfigs)
        {
            this.compConfigs = compConfigs;
            RestoreStashedComps();
        }

        public void PinComp(ComponentConfig compConfig)
        {
            var instanceData = new InstanceData();
            instanceData.SetCompInfo(compConfig);
            comps.Add(instanceData);
        }

        public void PinComp(InstanceData data)
        {
            comps.Add(data);
        }

        public void UnpinComp(InstanceData compData)
        {
            comps.Remove(compData);
        }

        void RestoreStashedComps()
        {
            var stashedComps = ComponentReadWriter.LoadComps();
            if (stashedComps.Count == 0)
            {
                comps.Clear();
                return;
            }

            for (int i = comps.Count - 1; i >= 0; i--)
            {
                var comp = comps[i];
                var idx = stashedComps.IndexOf(comp.typeFullName);
                if (idx != -1)
                {
                    stashedComps.RemoveAt(idx);
                }
            }
            foreach (var el in stashedComps)
            {
                var config = compConfigs.FirstOrDefault(item => item.type.FullName == el);
                if (config == null)
                {
                    Debug.LogError($"Failed to find type for {el}");
                    continue;
                }

                var instance = InstanceComp(config.type);
                var instanceData = new InstanceData();
                instanceData.SetCompInfo(config);
                instanceData.SetInstanceTarget(instance);
                comps.Add(instanceData);

                // instance.OnEnable();
            }
        }

        public static ComponentBase InstanceComp(Type type)
        {
            if (type.IsSubclassOf(typeof(ComponentBase)))
            {
                var instance = ScriptableObject.CreateInstance(type);
                instance.hideFlags = HideFlags.HideAndDontSave;
                return instance as ComponentBase;
            }
            else
            {
                Debug.LogError($"Failed to create instance of {type.FullName}");
                return null;
            }
        }
    }
}