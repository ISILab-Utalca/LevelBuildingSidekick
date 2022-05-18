using LevelBuildingSidekick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CreateNodeView : ToolView
{
    public UnityEvent OnButtonClick = new UnityEvent();

    public CreateNodeView(Controller controller) : base(controller)
    {

    }

    public override void Display()
    {
        Draw();
    }

    public override void Draw()
    {
        var data = Controller.Data as CreateNodeData;
        var controller = Controller as CreateNodeController;
        if(GUILayout.Button(data.label))
        {
            OnButtonClick?.Invoke();
        }
    }
}
