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
        private static readonly string defaultPath = "Assets/ISILab OLD/LBS Plugin/Resources/BackUp"; // esto podria ser peligroso (!)
        //private static readonly string defaultName = "/LBSBackUp.asset";

        public static LevelBackUp instance;

        public LoadedLevel level; // current

        public static LevelBackUp Instance() // Singleton
        {
            // si la instancia ya esta registrada la retorna
            if (instance != null)
            {
                return instance;
            }

            // si la instancia no esta registrada la busca y la retorna
            List<LevelBackUp> lbus = Resources.FindObjectsOfTypeAll<LevelBackUp>().Select(lbu => lbu as LevelBackUp).ToList();//Utility.DirectoryTools.GetScriptablesByType<LevelBackUp>();
            if (lbus.Count > 0)
            {
                instance = lbus[0];
                return instance;
            }


            // si no encuentra la instancia, la crea y la retorna
            var backUp = ScriptableObject.CreateInstance<LevelBackUp>();
            if (!Directory.Exists(defaultPath))
                Directory.CreateDirectory(defaultPath);

            //EditorUtility.SetDirty(instance);
            //AssetDatabase.CreateAsset(backUp, defaultPath + defaultName);
            //AssetDatabase.SaveAssets();
            instance = backUp;
            return instance;

        }
    }

    [System.Serializable]
    public class LoadedLevel
    {
        public string fullName = "";
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
