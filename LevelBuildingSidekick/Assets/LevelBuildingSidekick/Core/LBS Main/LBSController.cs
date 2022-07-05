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
                    _Instance = new LBSController(Resources.Load("LBSData") as LBSData);
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

        List<Step> steps;
        public Step currentStep;

        private GenericWindow _LevelWindow;
        public GenericWindow LevelWindow
        {
            get
            {
                if(_LevelWindow == null)
                {
                    _LevelWindow = GenericWindow.CreateInstance<GenericWindow>();
                    _LevelWindow.titleContent = new GUIContent("Level Window");
                    _LevelWindow.draw = Instance.currentStep.View.Draw2D;
                }
                return _LevelWindow;
            }
        }

        public GenericWindow _InspectorWindow;
        public GenericWindow InspectorWindow
        {
            get
            {
                if (_InspectorWindow == null)
                {
                    _InspectorWindow = EditorWindow.CreateInstance<GenericWindow>();
                    _InspectorWindow.titleContent = new GUIContent("Inspector Window");
                    _InspectorWindow.draw = Instance.currentStep.LevelRepresentation.View.DrawEditor;
                }
                return _InspectorWindow;
            }
        }

        public LevelData CurrentLevel
        {
            get
            {
                return (Data as LBSData).levelData;
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

        public LBSController(Data data) : base(data)
        {
            //Debug.Log((data as LBSData).stepsData[0]);
            //View = new LBSView(this);

            //LoadData();
            currentStep = steps[0];
        }

        [MenuItem("Level Building Sidekick/Open New")]
        public static void ShowWindow()
        {
            //Debug.Log(Instance.currentStep);
            Instance.currentStep.View.Display2DWindow();
        }

        [MenuItem("Level Building Sidekick/Show Inspector")]
        public static void ShowInspector()
        {
            Instance.currentStep.View.DisplayInspectorWindow();
        }

        public override void LoadData()
        {
            
            LBSData data = Data as LBSData;
            if(data == null)
            { 
                Debug.LogError("Incorrect Data Type: " + data.GetType());
                return;
            }


            steps = new List<Step>();
            //Debug.Log(data.stepsData.Count);
            foreach (Data d in data.stepsData)
            {
                var step = Activator.CreateInstance(d.ControllerType, new object[] { d });
                if(step is Step)
                {
                    //(step as Step).Data = d;
                    steps.Add(step as Step);
                    //Debug.Log(steps[^1]);
                    //steps[^1].Data = d;
                    continue;
                }
                Debug.LogError("Type: " + d.ControllerType + " is not an inheritance of Step class");
            }

            //Debug.Log(steps.Count);
            if (steps.Count > 0)
            {
                currentStep = steps[0];
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

            return window;
        }
    }
}


