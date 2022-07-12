using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick
{
    //[CreateAssetMenu(menuName = "LevelBuildingSidekick/Level Element")]
    [System.Serializable]
    public class LevelElementData : Data
    {
        public override Type ControllerType => typeof(LevelElementController);
        List<string> tags;
       
    }
}

