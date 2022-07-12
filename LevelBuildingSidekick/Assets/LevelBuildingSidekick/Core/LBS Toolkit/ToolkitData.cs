using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;

[System.Serializable]
//[CreateAssetMenu(menuName = "LevelBuildingSidekick/Tools/Toolkit")]
public class ToolkitData : Data
{
    public List<ToolData> tools = new List<ToolData>();
    public override Type ControllerType => typeof(ToolkitController);

    public ToolkitData() { }
    public ToolkitData(List<ToolData> _tools) 
    {
        tools = _tools;
    }
}
