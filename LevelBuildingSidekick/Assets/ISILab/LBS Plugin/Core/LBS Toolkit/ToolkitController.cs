using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS;
using System;

public class ToolkitController : Controller
{
    public List<LBSTool> Tools { get; set; }
    //public LevelRepresentationController Level { get; set; }
    
    public ToolkitController(Data data /*,LevelRepresentationController level*/) : base(data)
    {
        //Level = level;
        View = new ToolkitView(this);
    }

    internal void Switch(LBSTool current)
    {
        Tools.ForEach(t => t.IsActive = false);
        current.IsActive = true;
    }

    public override void LoadData()
    {
        //Each Tool
        Tools = new List<LBSTool>();
        var data = Data as ToolkitData;
        //Debug.Log("D: " + data.tools.Count);
        foreach (ToolData t in data.tools)
        {
            var tool = Activator.CreateInstance(t.ControllerType, new object[] { t , this});
            if (tool is LBSTool)
            {
                Tools.Add(tool as LBSTool);
            }
        }
        //Debug.Log("C: " + Tools.Count);
    }

    public override void Update()
    {
        foreach(LBSTool t in Tools)
        {
            t.InternalUpdate();
            t.Update();
        }
    }
}
