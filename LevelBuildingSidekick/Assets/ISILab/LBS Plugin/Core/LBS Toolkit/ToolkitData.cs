using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS;
using System;
using Newtonsoft.Json;

[System.Serializable]
//[CreateAssetMenu(menuName = "LevelBuildingSidekick/Tools/Toolkit")]
public class ToolkitData : Data
{
    public List<ToolData> tools = new List<ToolData>();

    [JsonIgnore]
    public override Type ControllerType => typeof(ToolkitController);

    public ToolkitData() { }
    public ToolkitData(List<ToolData> _tools) 
    {
        tools = _tools;
    }
}
