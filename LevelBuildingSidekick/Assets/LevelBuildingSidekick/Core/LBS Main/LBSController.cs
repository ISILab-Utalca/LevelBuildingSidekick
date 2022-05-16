using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace LevelBuildingSidekick
{
    public class LBSController : Controller
    {
        public static LBSController Instance {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new LBSController();
                }
                return _Instance;
            }
        }
        private static LBSController _Instance;

        List<Step> steps;
        public Step currentStep;

        public LBSController()
        {
            View = new LBSView(this);
            steps = new List<Step>();

            LoadData();
        }

        [MenuItem("Level Building Sidekick/Open New")]
        public static void ShowWindow()
        {
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
            LBSData data = new LBSData();
            //Parse data from Data to LBSData
            if(Data == null)
            {
                try
                {
                    Data = Resources.Load("New LBS Data") as LBSData;
                }
                catch
                {
                    Debug.LogError("No Data Found");
                }
            }

            try
            {
                data = Data as LBSData;
            }
            catch
            {
                Debug.LogError("Incorrect Data Type");
                return;
            }

            foreach (Data d in data.StepsData)
            {
                var step = Activator.CreateInstance(d.ControllerType);
                if(step is Step)
                {
                    (step as Step).Data = d;
                    Debug.Log(d.GetType());
                    steps.Add(step as Step);
                    continue;
                }
                Debug.LogError("Type: " + d.ControllerType + " is not an inheritance of Step class");
            }

            Debug.Log(steps.Count);
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


