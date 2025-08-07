using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit
{
    [InitializeOnLoad]
    sealed internal class ComponentRegistry
    {
        class MsgTemplate
        {
            public const string CantFindInfoProperty
            = "EditorKit component registration failed on <color=#1ed760>{0}</color>: The Info property is not correctly implemented.\n<color=#ffc107>You can see the example in the <color=#1ed760>{1}</color>.</color>";
        }

        readonly static List<ComponentConfig> infos = new();

        public static IReadOnlyList<ComponentConfig> Infos => infos;

        static ComponentRegistry()
        {
            ScanForComponents();
        }

        static void ScanForComponents()
        {
            infos.Clear();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (IsSystemAssembly(assembly))
                {
                    continue;
                }

                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (
                            typeof(IComponent).IsAssignableFrom(type) &&
                            !type.IsInterface &&
                            !type.IsAbstract &&
                            !type.IsGenericTypeDefinition)
                        {
                            if (typeof(ScriptableObject).IsAssignableFrom(type) is false)
                            {
                                Debug.LogError($"EditorKit component registration failed on {type.FullName}\nComponent must inherit from ScriptableObject.");
                                continue;
                            }

                            PropertyInfo infoProperty = type.GetProperty("Info", BindingFlags.Public | BindingFlags.Static);
                            if (infoProperty != null && infoProperty.PropertyType == typeof(ComponentInfo))
                            {
                                ComponentInfo componentInfo = infoProperty.GetValue(null) as ComponentInfo;
                                if (componentInfo != null)
                                {
                                    infos.Add(new(type, componentInfo));
                                }
                            }
                            else Debug.LogError(string.Format(MsgTemplate.CantFindInfoProperty, type.FullName, typeof(IComponent).FullName));
                        }
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Debug.LogWarning($"ComponentRegistry: Failed to load types from assembly '{assembly.FullName}'. Error: {ex.Message}");
                    foreach (Exception loaderEx in ex.LoaderExceptions)
                    {
                        Debug.LogWarning($"Loader exception: {loaderEx.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"ComponentRegistry: Failed to scan assembly '{assembly.FullName}'. Error: {ex.Message}");
                }
            }

            // Debug.Log($"ComponentRegistry: Scan completed. Found {infos.Count} components implementing IComponent.");
        }

        static bool IsSystemAssembly(Assembly assembly)
        {
            return assembly.FullName.StartsWith("Unity") ||
                   assembly.FullName.StartsWith("System") ||
                   assembly.FullName.StartsWith("mscorlib") ||
                   assembly.FullName.StartsWith("Mono") ||
                   assembly.FullName.Contains("Newtonsoft.Json");
        }
    }
}