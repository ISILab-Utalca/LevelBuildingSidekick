using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace LevelBuildingSidekick
{
    //[CreateAssetMenu(menuName = "LevelBuildingSidekick/PhysicalSpaceData")]
    [System.Serializable]
    public class PSEditorData : Data
    {
        [JsonIgnore] public string WindowName; // puede que esto ne sea necesario (?)
        public override Type ControllerType => typeof(PSEditorController);

        public LevelRepresentationData levelData = new OfficePlan.OfficePlanData();

        public PSEditorData()
        {
            levelData =  new OfficePlan.OfficePlanData();
        }

    }
}

