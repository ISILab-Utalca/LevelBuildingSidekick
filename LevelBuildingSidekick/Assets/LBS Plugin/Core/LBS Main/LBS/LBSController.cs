using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using LevelBuildingSidekick.Graph;

namespace LevelBuildingSidekick
{
    public class LBSController
    {
        private static LevelBackUp backUp;

        public static LevelData CurrentLevel
        {
            get
            {
                LoadBackup();
                return backUp.level;
            }
            set
            {
                LoadBackup();
                backUp.level = value;
            }
        }

        [MenuItem("LBS/[Old] Welcome window...",priority = 0)]
        public static void ShowWindow()
        {
            var window = LBSViewOld.GetWindow<LBSViewOld>("Level Building Sidekick");
        }

        private static void LoadBackup()
        {
            if (backUp == null)
            {
                backUp = Resources.Load("LBSBackUp") as LevelBackUp;
                if (backUp == null)
                {
                    backUp = ScriptableObject.CreateInstance<LevelBackUp>();
                    if(!Directory.Exists("Assets/LevelBuildingSidekick/Core/LBS Main/Level/Resources")) // esto podria ser peligroso (!)
                    {
                        Directory.CreateDirectory("Assets/LevelBuildingSidekick/Core/LBS Main/Level/Resources"); // esto podria ser peligroso (!)
                    }
                    AssetDatabase.CreateAsset(backUp, "Assets/LevelBuildingSidekick/Core/LBS Main/Level/Resources/LBSBackUp.asset"); // esto podria ser peligroso (!)
                    AssetDatabase.SaveAssets();
                }
            }
        }

        public static LevelData CreateLevel(string levelName, Vector2Int size)
        {
            LevelData data = new LevelData();
            data.levelName = levelName;
            data.size = size;
            data.representations.Add(new LBSGraphData());
            return data;
        }
    }
}


