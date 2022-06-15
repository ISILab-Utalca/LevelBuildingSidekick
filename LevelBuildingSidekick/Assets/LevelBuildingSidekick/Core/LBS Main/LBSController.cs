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
                if (_Instance == null)
                {
                    _Instance = new LBSController(Resources.Load("LBSData") as LBSData);
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

        public InspectorWindow _InspectorWindow;
        public InspectorWindow InspectorWindow
        {
            get
            {
                if (_InspectorWindow == null)
                {
                    _InspectorWindow = InspectorWindow.CreateInstance<InspectorWindow>();
                    _InspectorWindow.titleContent = new GUIContent("Inspector Window");
                    _InspectorWindow.controller = Instance.currentStep.LevelRepresentation;
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

        public LBSController(Data data) : base(data)
        {
            //View = new LBSView(this);

            //LoadData();
        }

        [MenuItem("Level Building Sidekick/Open New")]
        public static void ShowWindow()
        {
            Instance.LevelWindow.Show();
        }

        [MenuItem("Level Building Sidekick/Show Inspector")]
        public static void ShowInspector()
        {
            //Debug.Log(Instance.InspectorWindow);
            Instance.InspectorWindow.Show();
        }

        public override void LoadData()
        {

            LBSData data;
            try
            {
                data = Instance.Data as LBSData;
            }
            catch
            {
                //Debug.LogError("Incorrect Data Type");
                return;
            }


            steps = new List<Step>();
            foreach (Data d in data.stepsData)
            {
                var step = Activator.CreateInstance(d.ControllerType, new object[] { d });
                if(step is Step)
                {
                    //(step as Step).Data = d;
                    steps.Add(step as Step);
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
    }
}


