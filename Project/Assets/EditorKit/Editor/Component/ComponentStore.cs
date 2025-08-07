using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Hayson.EditorKit
{
    [Serializable]
    class ComponentStore
    {
        [SerializeField] List<ComponentData> comps = new();

        public IReadOnlyList<ComponentData> Comps => comps;

        public IReadOnlyList<ComponentConfig> CompsConfig { get; private set; }

        public void Setup(IReadOnlyList<ComponentConfig> compConfigs)
        {
            CompsConfig = compConfigs;
            RestoreStashedComps();
        }

        public void PinComp(ComponentConfig compConfig)
        {
            var instanceData = new ComponentData();
            instanceData.SetCompInfo(compConfig);
            comps.Add(instanceData);
        }

        public void PinComp(ComponentData data)
        {
            InstallComp(data);
            comps.Add(data);
        }

        public void UnpinComp(ComponentData compData)
        {
            UninstallComp(compData);
            comps.Remove(compData);
        }

        void InstallComp(ComponentData data)
        {

        }

        void UninstallComp(ComponentData data)
        {
            if (data.Component != null)
            {
                data.Component.OnDisable();
            }
            if (data.instanceObj != null)
            {
                UnityEngine.Object.DestroyImmediate(data.instanceObj);
            }
        }

        void RestoreStashedComps()
        {
            var isStorageExist = ComponentRecordStorage.TryLoadComps(out var stashedComps);

            if (isStorageExist is false)
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
                var config = CompsConfig.FirstOrDefault(item => item.type.FullName == el);
                if (config == null)
                {
                    Debug.LogError($"Failed to find type for {el}");
                    continue;
                }

                var instance = ComponentInstanceCreator.Create(config.type);
                var instanceData = new ComponentData();
                instanceData.SetCompInfo(config);
                instanceData.SetInstanceObj(instance);
                comps.Add(instanceData);
            }

            // 如果實體出來的數量和Stash的數量不一致，則回寫最新可用元件的紀錄
            if (comps.Count != stashedComps.Count)
            {
                ComponentRecordStorage.StashComps(comps.Select(el => el.type));
            }
        }
    }
}