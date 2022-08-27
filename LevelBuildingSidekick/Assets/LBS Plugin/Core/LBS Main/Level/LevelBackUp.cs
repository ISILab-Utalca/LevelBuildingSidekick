using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LevelBuildingSidekick
{
    public class LevelBackUp : ScriptableObject
    {
        private static readonly string defaultPath = "Assets/LevelBuildingSidekick/Core/LBS Main/Level/Resources"; // esto podria ser peligroso (!)
        private static readonly string defaultName = "/LBSBackUp.asset";

        public static LevelBackUp instance;

        public LoadedLevel level; // current

        public static LevelBackUp Instance() // Singleton
        {
            // si la instancia ya esta registrada la retorna
            if (instance != null)
            {
                EditorUtility.SetDirty(instance);
                AssetDatabase.SaveAssets();
                return instance;
            }

            // si la instancia no esta registrada la busca y la retorna
            List<LevelBackUp> lbus = Utility.DirectoryTools.GetScriptablesByType<LevelBackUp>();
            if (lbus.Count > 0)
            {
                instance = lbus[0];
                EditorUtility.SetDirty(instance);
                AssetDatabase.SaveAssets();
                return instance;
            }


            // si no encuentra la instancia, la crea y la retorna
            var backUp = ScriptableObject.CreateInstance<LevelBackUp>();
            if (!Directory.Exists(defaultPath))
                Directory.CreateDirectory(defaultPath);

            EditorUtility.SetDirty(instance);
            AssetDatabase.CreateAsset(backUp, defaultPath + defaultName);
            AssetDatabase.SaveAssets();
            instance = backUp;
            return instance;

        }
    }

    [System.Serializable]
    public class LoadedLevel
    {
        public string fullName = "";
        public LevelData data;

        public FileInfo FileInfo => new FileInfo(fullName);

        public LoadedLevel(LevelData data, string fullName)
        {
            this.fullName = fullName;
            this.data = data;
        }
    }

}
