using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("Boolean", typeof(LBSBoolCharacteristic))]
public class LBSBoolCharEditor : LBSCustomEditor
{
    public TextField labelField;
    public Toggle toogle;

    public LBSBoolCharacteristic target;

    public LBSBoolCharEditor()
    {
        labelField = new TextField();
        this.Add(labelField);
        labelField.RegisterCallback<BlurEvent>(e => {
            target.Label = labelField.value;
        });

        toogle = new Toggle("Value:");
        this.Add(toogle);
        toogle.RegisterCallback<ChangeEvent<bool>>(e =>
        {
            target.Value = e.newValue;
        });
    }

    public override void SetInfo(object obj)
    {
        this.target = obj as LBSBoolCharacteristic;

        labelField.value = this.target?.Label;
        toogle.value = this.target.Value;
    }

    protected override VisualElement CreateVisualElement()
    {
        throw new System.NotImplementedException();
    }
}
