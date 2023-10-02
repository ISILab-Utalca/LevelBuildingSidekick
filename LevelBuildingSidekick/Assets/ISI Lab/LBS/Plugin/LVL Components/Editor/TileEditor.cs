using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;
using LBS.Components.TileMap;

[LBSCustomEditor("Tile", typeof(LBSTile))]
public class TileEditor : LBSCustomEditor
{
    private Vector2Field field;

    public TileEditor()
    {
        CreateVisualElement();
    }

    public override void SetInfo(object target)
    {
        this.target = target;
        var t = target as LBSTile;

        field.value = t.Position;
    }

    protected override VisualElement CreateVisualElement()
    {
        field = new Vector2Field("Position");
        field.SetEnabled(false);
        this.Add(field);
        this.style.flexGrow = 1;

        return this;
    }
}
