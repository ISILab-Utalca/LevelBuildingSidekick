using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelBuildingSidekick
{
    public class LBSController : Controller
    {
        public static LBSController Instance{
            get
            {
                if(_Instance == null)
                {
                    _Instance = new LBSController();
                }
                return _Instance;
            }
        }
        private static LBSController _Instance;

        static List<Step> steps;
        static Step currentStep;

        public LBSController()
        {
            View = new LBSView();
        }

        [MenuItem("Level Building Sidekick/Open New")]
        public static void ShowWindow()
        {
            Instance.View.Display();
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
            LBSData data = Data as LBSData;
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
        }
    }
}


