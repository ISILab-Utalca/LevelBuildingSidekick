using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;

namespace LevelBuildingSidekick
{
    public class LBSController
    {
        private static LevelBackUp backUp;

        public static LevelData CurrentLevel
        {
            get
            {
                LoadbakcUp();
                return backUp.level;
            }
            set
            {
                LoadbakcUp();
                backUp.level = value;
            }
        }

        [MenuItem("Level Building Sidekick/Open New")]
        public static void ShowWindow()
        {
            var window = LBSView.GetWindow<LBSView>("Level Building Sidekick");
        }

        private static void LoadbakcUp()
        {
            if (backUp == null)
            {
                backUp = Resources.Load("LBSBackUp") as LevelBackUp;
                if (backUp == null)
                {
                    backUp = ScriptableObject.CreateInstance<LevelBackUp>();
                    if(!Directory.Exists("Assets/LevelBuildingSidekick/Core/LBS Main/Level/Resources"))
                    {
                        Directory.CreateDirectory("Assets/LevelBuildingSidekick/Core/LBS Main/Level/Resources");
                    }
                    AssetDatabase.CreateAsset(backUp, "Assets/LevelBuildingSidekick/Core/LBS Main/Level/Resources/LBSBackUp.asset");
                    AssetDatabase.SaveAssets();
                }
            }
        }

        public static LevelData CreateLevel(string levelName, Vector2Int size)
        {
            LevelData data = new LevelData();
            data.levelName = levelName;
            data.size = size;
            return data;
        }
    }
}


