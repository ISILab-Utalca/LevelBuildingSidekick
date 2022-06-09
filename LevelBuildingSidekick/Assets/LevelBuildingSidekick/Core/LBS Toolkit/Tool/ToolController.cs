using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using LevelBuildingSidekick;

public abstract class ToolController : Controller
{
    protected ToolkitController Toolkit { get; set; }
    public bool IsActive { get; set; }
    protected ToolController(Data data, ToolkitController toolkit) : base(data)
    {
        Toolkit = toolkit;
    }

    public virtual void Switch()
    {
        IsActive = !IsActive;
    }

    public abstract void Action(LevelRepresentationController level);

    public override void Update()
    {
    }


}
