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
        [SerializeField]
        public List<Data> StepsData;

        public override Type ControllerType => typeof(LBSController);
    }
}

