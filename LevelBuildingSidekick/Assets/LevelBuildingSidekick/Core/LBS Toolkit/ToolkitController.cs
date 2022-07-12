using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;

public class ToolkitController : Controller
{
    public List<ToolController> Tools { get; set; }
    public LevelRepresentationController Level { get; set; }
    
    public ToolkitController(Data data, LevelRepresentationController level) : base(data)
    {
        Level = level;
        View = new ToolkitView(this);
    }

    public override void LoadData()
    {
        //Each Tool
        Tools = new List<ToolController>();
        var data = Data as ToolkitData;
        //Debug.Log("D: " + data.tools.Count);
        foreach (ToolData t in data.tools)
        {
            var tool = Activator.CreateInstance(t.ControllerType, new object[] { t , this});
            if (tool is ToolController)
            {
                Tools.Add(tool as ToolController);
            }
        }
        //Debug.Log("C: " + Tools.Count);
    }

    public override void Update()
    {
        foreach(ToolController t in Tools)
        {
            t.Update();
        }
    }
}
