using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick
{
    //[CreateAssetMenu(menuName = "LevelBuildingSidekick/GlobalData")]
    [System.Serializable]
    public class LBSData : Data
    {
        public LevelData levelData;
        [NonReorderable]
        public List<Data> stepsData = new List<Data>();
        public Dictionary<string, LevelRepresentationData> levelRepresentations = new Dictionary<string, LevelRepresentationData>();
        public override Type ControllerType => typeof(LBSController);
    }
}

