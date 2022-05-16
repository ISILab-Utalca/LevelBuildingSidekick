using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LevelBuildingSidekick
{
    [CreateAssetMenu(menuName = "LevelBuildingSidekick/PhysicalSpaceData")]
    public class PSEditorData : Data
    {
        public string WindowName;
        public override Type ControllerType => typeof(PSEditorController);

    }
}

