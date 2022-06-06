using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick
{
    [CreateAssetMenu(menuName = "LevelBuildingSidekick/GlobalData")]
    [System.Serializable]
    public class LBSData : Data
    {
        public LevelData levelData;
        public List<Data> stepsData;

        public override Type ControllerType => typeof(LBSController);
    }
}

