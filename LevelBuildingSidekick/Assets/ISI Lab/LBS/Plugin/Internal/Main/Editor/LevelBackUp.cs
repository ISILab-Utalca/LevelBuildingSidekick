using LBS.Settings;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ISILab.LBS
{
    public class BackUp : ScriptableObject
    {
        public LoadedLevel level;
    }

    public static class LBS
    {
        public static LoadedLevel loadedLevel;
    }

    [System.Serializable]
    public class LoadedLevel : ScriptableObject
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

        public static LoadedLevel CreateInstance(LBSLevelData data, string fullName)
        {
            var level = ScriptableObject.CreateInstance<LoadedLevel>();
            level.data = data;
            level.fullName = fullName;
            return level;
        }
    }
}
