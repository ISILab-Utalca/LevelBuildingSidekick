using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using LevelBuildingSidekick;

public abstract class ToolView : View
{
    protected ToolView(Controller controller) : base(controller)
    {
    }
    public abstract void DrawInToolkit();

}
