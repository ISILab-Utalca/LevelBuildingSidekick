using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace LevelBuildingSidekick
{

    public class LBSController : Controller
    {
        public static LBSController Instance {
            get
            {
                //Debug.Log("Here");
                if (_Instance == null)
                {
                    //Debug.Log("It's Null");
                    _Instance = new LBSController( new LBSData() );
                    //Debug.Log(_Instance + " - " + _Instance.currentStep);
                }
                else
                {
                    //Debug.Log(_Instance);
                }
                return _Instance;
            }
        }
        private static LBSController _Instance;

        public LevelRepresentationController CurrentStep;

        private LevelController _CurrentLevel;
        public LevelController CurrentLevel
        {
            get
            {
                return _CurrentLevel;
            }
            set
            {
                _CurrentLevel = value;
                (Data as LBSData).levelData = value.Data as LevelData;
                //Debug.Log(_CurrentLevel.Steps.Count);
                if(_CurrentLevel.Representations.Count > 0)
                    CurrentStep = _CurrentLevel.Representations[0];
            }
        }

        private Dictionary<string, GenericWindow> _Windows;
        public Dictionary<string, GenericWindow> Windows
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

        public Dictionary<string, LevelRepresentationData> LevelRepresentations
        {
            get
            {
                if((Data as LBSData).levelRepresentations == null)
                {
                    //SHOULD LOAD DATA
                    (Data as LBSData).levelRepresentations = new Dictionary<string, LevelRepresentationData>();
                }
                return (Data as LBSData).levelRepresentations;
            }
        }

        public LBSController(Data data) : base(data)
        {
            //Debug.Log((data as LBSData).stepsData[0]);
            //View = new LBSView(this);

            //LoadData();
            View = new LBSView(this);
            //currentStep = steps[0];
        }

        [MenuItem("Level Building Sidekick/Open New")]
        public static void ShowWindow()
        {
            //Debug.Log(Instance.View);
            Instance.View.DisplayInspectorWindow();
        }
/*
        [MenuItem("Level Building Sidekick/Show Inspector")]
        public static void ShowInspector()
        {
            Instance.currentStep.View.DisplayInspectorWindow();
        }*/

        public override void LoadData()
        {
            
            LBSData data = Data as LBSData;
            if(data == null)
            { 
                Debug.LogError("Incorrect Data Type: " + data.GetType());
                return;
            }

            if(data.levelData !=  null)
            {
                var level = Activator.CreateInstance(data.levelData.ControllerType, new object[] { data.levelData });
                if (level is LevelController)
                {
                    _CurrentLevel = level as LevelController;
                }
            }
            
        }

        public override void Update()
        {
        }

        public GenericWindow RequestWindow(string title)
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

        public LevelController CreateLevel(string levelName, Vector2Int size)
        {
            LevelData d = new LevelData();
            d.levelName = levelName;
            d.size = size;
            d.representations.Add(new OfficePlan.OfficePlanData () );
            LevelController c = new LevelController(d);
            return c;
        }

        public void SetLevel(LevelData level)
        {
            CurrentLevel = Activator.CreateInstance(typeof(LevelController), new object[] { level}) as LevelController;
        }

    }
}


