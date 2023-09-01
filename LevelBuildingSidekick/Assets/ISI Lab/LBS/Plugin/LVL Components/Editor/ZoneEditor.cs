using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.InputField;

[LBSCustomEditor("Zone", typeof(Zone))]
public class ZoneEditor : LBSCustomEditor
{
    private Zone target;

    public override void SetInfo(object target)
    {
        this.target = target as Zone;
    }

    protected override VisualElement CreateVisualElement()
    {
        var content = new VisualElement();
        
        var nameFiled = new TextField();
        nameFiled.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            target.ID = evt.newValue;
        }); 
        
        var colorField = new ColorField();
        colorField.RegisterCallback<ChangeEvent<Color>>(evt => 
        {
            target.Color = evt.newValue;
        });

        return content;
    }


}
