using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;

public abstract class ToolController : Controller
{
    ToolkitController Toolkit { get; set; }
    protected ToolController(Data data) : base(data)
    {
    }

    public abstract void PrepareAction(LevelRepresentationController level); 

    public abstract void Action(LevelRepresentationController level);
}
