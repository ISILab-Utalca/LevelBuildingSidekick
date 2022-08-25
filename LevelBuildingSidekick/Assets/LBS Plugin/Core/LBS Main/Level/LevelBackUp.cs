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
        private static LevelBackUp instance;
        public LevelData level; // current

        public static LevelBackUp Instance() // Singleton
        {
            // si la instancia ya esta registrada la retorna
            if (instance != null)
                return instance;

            // si la instancia no esta registrada la busca y la retorna
            List<LevelBackUp> lbus = Utility.DirectoryTools.GetScriptablesByType<LevelBackUp>();
            if (lbus.Count > 0)
            {
                instance = lbus[0];
                return instance;
            }

            // si no encuentra la instancia, la crea y la retorna
            var backUp = ScriptableObject.CreateInstance<LevelBackUp>();
            if (!Directory.Exists(defaultPath))
                Directory.CreateDirectory(defaultPath);

            AssetDatabase.CreateAsset(backUp, defaultPath + defaultName);
            AssetDatabase.SaveAssets();
            instance = backUp;
            return instance;
        }
    }

}
