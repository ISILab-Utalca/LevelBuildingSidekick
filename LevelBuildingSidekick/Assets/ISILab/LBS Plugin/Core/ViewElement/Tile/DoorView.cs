using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DoorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<DoorView, VisualElement.UxmlTraits> { }

    public VisualElement left;
    public VisualElement right;
    public VisualElement top;
    public VisualElement bottom;

    public DoorView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Door");
        visualTree.CloneTree(this);

        left = this.Q<VisualElement>(name: "Left");
        right = this.Q<VisualElement>(name: "Right");
        top = this.Q<VisualElement>(name: "Top");
        bottom = this.Q<VisualElement>(name: "Botton");

    }
    /// <summary>
    /// Terminar Nicolas
    /// </summary>
    /// <param name="one"></param>
    public DoorView(Vector2Int from, Vector2Int to, Color color)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Door");
        visualTree.CloneTree(this);

        left = this.Q<VisualElement>(name: "DoorLeft");
        right = this.Q<VisualElement>(name: "DoorRight");
        top = this.Q<VisualElement>(name: "DoorTop");
        bottom = this.Q<VisualElement>(name: "DoorBottom");

        HideAllDoors();
        ShowDir(to-from);
        SetDoorColor(color);
    }

    public void ShowDir(Vector2Int dir)
    {
        var angle = Vector2.Angle(Vector2.right, dir) % 360;

        if(angle < 45 || angle > 315)
        {
            right.visible = true;
        }
        else if( angle > 45 && angle <= 135)
        {
            top.visible = true;
        }
        else if (angle > 135 && angle <= 225)
        {
            left.visible = true;
        }
        else if (angle > 225 && angle <= 315)
        {
            bottom.visible = true;
        }
    }

    public void SetDoorColor(Color color)
    {
        left.style.backgroundColor = color;
        right.style.backgroundColor = color;
        top.style.backgroundColor = color;
        bottom.style.backgroundColor = color;
    }

    public void HideAllDoors()
    {
        left.visible = false;
        right.visible = false;
        top.visible = false;
        bottom.visible = false;
    }

}
