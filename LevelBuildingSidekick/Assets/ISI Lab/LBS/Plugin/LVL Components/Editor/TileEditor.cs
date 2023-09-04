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

    private Vector2Field field;

    public TileEditor()
    {
        CreateVisualElement();
    }

    public override void SetInfo(object target)
    {
        this.target = target as LBSTile;

        field.value = this.target.Position;

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
