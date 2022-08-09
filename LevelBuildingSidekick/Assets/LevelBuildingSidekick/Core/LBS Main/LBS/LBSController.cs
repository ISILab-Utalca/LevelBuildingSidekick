using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

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
                Debug.Log(backUp.level);
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
                Debug.Log("Loading");
                backUp = Resources.Load("LBSBackUp") as LevelBackUp;
                if (backUp == null)
                {
                    Debug.Log("Created new");
                    backUp = ScriptableObject.CreateInstance<LevelBackUp>();
                    AssetDatabase.CreateAsset(backUp, "Assets/LevelBuildingSidekick/Core/LBS Main/Level/Resources/LBSBackUp.asset");
                    AssetDatabase.SaveAssets();
                }
            }
        }

        private static Dictionary<string, GenericWindow> _Windows;
        public static Dictionary<string, GenericWindow> Windows
        {
            get 
            {
                if(_Windows == null)
                {
                    _Windows = new Dictionary<string, GenericWindow>();
                }
                return _Windows;
            }
        }

        [MenuItem("Level Building Sidekick/Open New")]
        public static void ShowWindow()
        {
            //Debug.Log(Instance.View);
            var window = LBSView.GetWindow<LBSView>("Level Building Sidekick");
        }
/*
        [MenuItem("Level Building Sidekick/Show Inspector")]
        public static void ShowInspector()
        {
            Instance.currentStep.View.DisplayInspectorWindow();
        }*/

        public static GenericWindow RequestWindow(string title)
        {
            GenericWindow window;

            if (!Windows.ContainsKey(title) )
            {
                window = EditorWindow.CreateInstance<GenericWindow>();
                Windows.Add(title, window);
                window.titleContent = new GUIContent(title);
                return window;
            }
            
            window = Windows[title];
            if(window == null)
            {
                window = EditorWindow.CreateInstance<GenericWindow>();
                window.titleContent = new GUIContent(title);
                Windows[title] = window;
            }

            //window.close = () => Utility.JSONDataManager.SaveData(CurrentLevel.Name, CurrentLevel.Data as LevelData);

            return window;
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


