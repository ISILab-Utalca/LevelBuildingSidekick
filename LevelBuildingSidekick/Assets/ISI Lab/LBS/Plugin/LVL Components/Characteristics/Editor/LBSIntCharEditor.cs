using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("Integer", typeof(LBSIntCharacteristic))]
public class LBSIntCharEditor : LBSCustomEditor
{
    public TextField labelField;
    public IntegerField input;


    public LBSIntCharEditor(object target) : base(target)
    {
        Add(CreateVisualElement());
        SetInfo(target);
    }
    public LBSIntCharEditor()
    {
        Add(CreateVisualElement());
    }

    public override void SetInfo(object obj)
    {
        this.target = obj;
        var target = obj as LBSIntCharacteristic;

        labelField.value = target?.Label;
        input.value = target.Value;
    }

    protected override VisualElement CreateVisualElement()
    {
        var cr = target as LBSIntCharacteristic;

        var ve = new VisualElement();
        labelField = new TextField();
        ve.Add(labelField);
        labelField.RegisterCallback<BlurEvent>(e => {
            cr.Label = labelField.value;
        });

        input = new IntegerField("Value:");
        ve.Add(input);
        input.RegisterCallback<ChangeEvent<int>>(e =>
        {
            cr.Value = e.newValue;
        });
        return ve;
    }
}
