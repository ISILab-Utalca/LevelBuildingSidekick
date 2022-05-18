using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System;

public class ToolkitController : Controller
{
    public List<ToolController> ToolControllers { get; set; }
    public ToolController activeTool;
    public LevelRepresentationController Level { get; set; }

    public ToolkitController(Data data, LevelRepresentationController level) : base(data)
    {
        View = new ToolkitView(this);
        Level = level;
        SetActiveTool(ToolControllers[0]);
    }

    public override void LoadData()
    {
        //Each Tool
        ToolControllers = new List<ToolController>();
        var data = Data as ToolkitData;
        foreach(ToolData t in data.toolDatas)
        {
            var tool = Activator.CreateInstance(t.ControllerType, new object[] { t });
            if (tool is ToolController)
            {
                ToolControllers.Add(tool as ToolController);
            }
        }
    }

    public void SetActiveTool(ToolController tool)
    {
        if(tool == null)
        {
            return;
        }
        activeTool = tool;
        activeTool.PrepareAction(Level);
    }

    public override void Update()
    {
    }
}
