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
        //private LevelController _CurrentLevel;
        public static LevelController CurrentLevel
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

        [MenuItem("Level Building Sidekick/Open New")]
        public static void ShowWindow()
        {
            //Debug.Log(Instance.View);
            var window = LBSView.GetWindow<LBSView>("Level Building Sidekick");
        }

        public static LevelController CreateLevel(string levelName, Vector2Int size)
        {
            LevelData d = new LevelData();
            d.levelName = levelName;
            d.size = size;
            d.representations.Add(new OfficePlan.OfficePlanData () );
            LevelController c = new LevelController(d);
            return c;
        }

        public static void SetLevel(LevelData level)
        {
            CurrentLevel = Activator.CreateInstance(typeof(LevelController), new object[] { level}) as LevelController;
        }

    }
}


