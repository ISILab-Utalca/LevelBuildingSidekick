using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LevelBuildingSidekick
{
    //[CreateAssetMenu(menuName = "LevelBuildingSidekick/PhysicalSpaceData")]
    [System.Serializable]
    public class PSEditorData : Data
    {
        public string WindowName;
        public override Type ControllerType => typeof(PSEditorController);

        public LevelRepresentationData levelData = new OfficePlan.OfficePlanData();

        public PSEditorData()
        {
            levelData =  new OfficePlan.OfficePlanData();
        }

    }
}

