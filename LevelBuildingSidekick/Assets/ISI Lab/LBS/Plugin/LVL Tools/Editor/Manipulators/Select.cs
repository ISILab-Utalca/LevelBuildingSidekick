using LBS;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Select : LBSManipulator
{
    private LBSLayer layer;
    private LBSLocalCurrent current;

    public Select()
    {
        // Unset feedback
        feedback = null;

        current = LBSInspectorPanel.Instance.current; // (?) parche ??
    }

    public override void Init(LBSLayer layer, object provider)
    {
        // Set layer reference
        this.layer = layer;

        // Set provider reference
        current = provider as LBSLocalCurrent;

    }

    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int position, MouseMoveEvent e)
    {
    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        current = LBSInspectorPanel.Instance.current; // (?) parche ??

        // Get fixed position
        var pos = layer.ToFixedPosition(position);

        // Get selectable elements
        var selected = new List<object>();
        foreach (var module in layer.Modules)
        {
            if(module is ISelectable)
            {
                selected.AddRange((module as ISelectable).GetSelected(pos));
            }
        }

        current.SetSelectedVE(selected);
    }
}
