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
    public class BackUp : ScriptableObject
    {
        public LoadedLevel level;
    }

    public static class LBS
    {
        public static LoadedLevel loadedLevel;
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
