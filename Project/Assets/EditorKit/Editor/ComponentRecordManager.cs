using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit
{
    public class ComponentReadWriter : ScriptableObject
    {
        const string FileName = "PinnedComps.txt";

        static string GetUserSettingsDirectory()
        {
            string projectRoot = Directory.GetParent(Application.dataPath).FullName;
            return Path.Combine(projectRoot, Config.UserSettingsDirectory, Config.EditorKitDirectory);
        }

        static string GetFilePath()
        {
            return Path.Combine(GetUserSettingsDirectory(), FileName);
        }

        public static void StashComps(IEnumerable<Type> typesToSave)
        {
            if (typesToSave == null)
            {
                return;
            }

            IEnumerable<string> typeFullNames = typesToSave.Select(t => t.FullName);

            try
            {
                string directory = GetUserSettingsDirectory();

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string filePath = GetFilePath();

                File.WriteAllLines(filePath, typeFullNames);
            }
            catch (Exception ex)
            {
                Debug.LogError($"儲存類型時發生錯誤: {ex.Message}");
            }
        }

        public static List<string> LoadComps()
        {
            string filePath = GetFilePath();

            if (!File.Exists(filePath))
            {
                return new List<string>();
            }

            try
            {
                string[] lines = File.ReadAllLines(filePath);
                List<string> loadedNames = lines.Where(line => !string.IsNullOrEmpty(line)).ToList();
                return loadedNames;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
    }
}