using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit
{
    [InitializeOnLoad]
    internal class ComponentRegistry
    {
        readonly static List<ComponentConfig> infos = new();

        public static IReadOnlyList<ComponentConfig> Infos => infos;

        static ComponentRegistry()
        {
            ScanForComponents();
        }

        public static void ScanForComponents()
        {
            infos.Clear();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith("Unity") ||
                    assembly.FullName.StartsWith("System") ||
                    assembly.FullName.StartsWith("mscorlib") ||
                    assembly.FullName.StartsWith("Mono") ||
                    assembly.FullName.Contains("Newtonsoft.Json"))
                {
                    continue;
                }

                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.IsSubclassOf(typeof(ComponentBase)) &&
                            !type.IsInterface &&
                            !type.IsAbstract &&
                            !type.IsGenericTypeDefinition)
                        {
                            PropertyInfo infoProperty = type.GetProperty("Info", BindingFlags.Public | BindingFlags.Static);

                            if (infoProperty != null && infoProperty.PropertyType == typeof(ComponentInfo))
                            {
                                ComponentInfo componentInfo = infoProperty.GetValue(null) as ComponentInfo;
                                if (componentInfo != null)
                                {
                                    infos.Add(new()
                                    {
                                        type = type,
                                        info = componentInfo
                                    });
                                }
                            }
                            else
                            {
                                Debug.LogError($"EditorKit component registration failed on {type.FullName}: The Info property is not correctly implemented.");
                            }
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
    }
}