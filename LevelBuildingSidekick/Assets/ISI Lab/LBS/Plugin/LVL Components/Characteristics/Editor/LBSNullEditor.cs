using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSNullEditor : LBSCustomEditor
{
    public LBSNullEditor()
    {
    }

    public override void SetInfo(object obj)
    {
        var target = obj as LBSCharacteristic;

        var label = new Label("la caracteristica "+target+" no se puede visualizar.");
        label.style.flexWrap = Wrap.Wrap;
        label.style.whiteSpace = WhiteSpace.Normal;
        this.Add(label);
    }

    protected override VisualElement CreateVisualElement()
    {
        throw new System.NotImplementedException();
    }
}
