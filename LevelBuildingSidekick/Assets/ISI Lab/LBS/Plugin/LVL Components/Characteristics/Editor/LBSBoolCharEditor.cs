using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("Boolean", typeof(LBSBoolCharacteristic))]
public class LBSBoolCharEditor : LBSCustomEditor
{
    public TextField labelField;
    public Toggle toogle;

    public LBSBoolCharEditor(object target) : base(target)
    {
        Add(CreateVisualElement());
        SetInfo(target);
    }

    public override void SetInfo(object obj)
    {
        this.target = obj;
        var target = obj as LBSBoolCharacteristic;

        labelField.value = target?.Label;
        toogle.value = target.Value;
    }

    protected override VisualElement CreateVisualElement()
    {
        var ve = new VisualElement();
        var target = this.target as LBSBoolCharacteristic;

        labelField = new TextField();
        ve.Add(labelField);
        labelField.RegisterCallback<BlurEvent>(e => {
            target.Label = labelField.value;
        });

        toogle = new Toggle("Value:");
        ve.Add(toogle);
        toogle.RegisterCallback<ChangeEvent<bool>>(e =>
        {
            target.Value = e.newValue;
        });

        return ve;
    }
}
