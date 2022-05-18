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

        public LBSController(Data data) : base(data)
        {
            View = new LBSView(this);

            //LoadData();
        }

        [MenuItem("Level Building Sidekick/Open New")]
        public static void ShowWindow()
        {
            //Debug.Log("I: " + Instance + " - Step: " + Instance.currentStep);
            Instance.currentStep.View.Display();
            //view = EditorWindow.GetWindow<LBSWindow>("Level Building Sidekick");
            //view.Show();
            /*
            if(currentStep != null)
            {
                view.titleContent.text = currentStep.Name;
            }*/
        }

        public override void LoadData()
        {
            /*LBSData data = ScriptableObject.CreateInstance<LBSData>();
            data.name = "LBSData";
            //Parse data from Data to LBSData
            if (Data == null)
            {
                try
                {
                    Data = Resources.Load("LBSData") as LBSData;
                }
                catch
                {
                    Debug.LogError("No Data Found");
                }
            }*/
            LBSData data;
            try
            {
                data = Data as LBSData;
            }
            catch
            {
                Debug.LogError("Incorrect Data Type");
                return;
            }


            steps = new List<Step>();
            foreach (Data d in data.StepsData)
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


