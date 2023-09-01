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
    private LBSTile target;

    public override void SetInfo(object target)
    {
        this.target = target as LBSTile;
    }

    protected override VisualElement CreateVisualElement()
    {
        var field = new Vector2Field("Position");
        field.SetEnabled(false);
        field.value = target.Position;

        this.Add(field);

        return this;
    }
}
