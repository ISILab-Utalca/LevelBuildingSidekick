using LBS.Settings;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LBS
{
    [CreateAssetMenu(menuName = "ISILab/LBS plugin/Back Up", fileName ="LBSBackUp.asset")]
    public class LevelBackUp : ScriptableObject
    {
        public static LevelBackUp instance;

        public LoadedLevel level; // current

        public static LevelBackUp Instance() // Singleton
        {
            var settings = LBSSettings.Instance;
            var path = settings.paths.backUpPath;

            // si la instancia ya esta registrada la retorna
            if (instance != null)
            {
                return instance;
            }

            // si la instancia no esta registrada la busca y la retorna
            instance = Utility.DirectoryTools.GetScriptablesByType<LevelBackUp>()[0];
            if (instance != null) { return instance; }

            // List<LevelBackUp> lbus = Resources.FindObjectsOfTypeAll<LevelBackUp>().Select(lbu => lbu as LevelBackUp).ToList();
            // if (lbus.Count > 0)
            // {
            //     instance = lbus[0];
            //     return instance;
            //}

            // si no encuentra la instancia, la crea y la retorna
            var backUp = ScriptableObject.CreateInstance<LevelBackUp>();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            AssetDatabase.CreateAsset(backUp, path);
            AssetDatabase.SaveAssets();

            instance = backUp;
            return instance;

        }

        private void OnDisable()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        private void OnDestroy()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }

    [System.Serializable]
    public class LoadedLevel
    {
        [JsonRequired]
        public string fullName = "";
        [JsonRequired]
        public LBSLevelData data;

        public FileInfo FileInfo
        {
            get
            {
                try
                {
                    var fileInfo = new FileInfo(fullName);
                    return fileInfo;
                }
                catch
                {
                    return null;
                }
            }
        }

        public LoadedLevel(LBSLevelData data, string fullName)
        {
            this.fullName = fullName;
            this.data = data;
        }
    }

}
