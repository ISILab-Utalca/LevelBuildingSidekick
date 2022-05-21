using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;

public class ToolkitController : Controller
{
    public List<ToolController> ToolControllers { get; set; }
    public LevelRepresentationController Level { get; set; }

    public ToolkitController(Data data, LevelRepresentationController level) : base(data)
    {
        View = new ToolkitView(this);
        Level = level;
    }

    public override void LoadData()
    {
        //Each Tool
        ToolControllers = new List<ToolController>();
        var data = Data as ToolkitData;
        foreach(ToolData t in data.toolDatas)
        {
            var tool = Activator.CreateInstance(t.ControllerType, new object[] { t , this});
            if (tool is ToolController)
            {
                ToolControllers.Add(tool as ToolController);
            }
        }
    }

    public override void Update()
    {
        foreach(ToolController t in ToolControllers)
        {
            t.Update();
        }
    }
}
