using LevelBuildingSidekick;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class LBSRepesentationData : Data
{
    public override Type ControllerType => throw new NotImplementedException();

    public abstract void Print();
}
