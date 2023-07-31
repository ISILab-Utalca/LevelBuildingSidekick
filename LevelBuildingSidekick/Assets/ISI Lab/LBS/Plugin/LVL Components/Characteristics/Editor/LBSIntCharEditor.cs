using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("Integer", typeof(LBSIntCharacteristic))]
public class LBSIntCharEditor : LBSCustomEditor
{
    public TextField labelField;
    public IntegerField input;

    public LBSIntCharacteristic target;

    public LBSIntCharEditor()
    {
        labelField = new TextField();
        this.Add(labelField);
        labelField.RegisterCallback<BlurEvent>(e => {
            target.Label = labelField.value;
        });

        input = new IntegerField("Value:");
        this.Add(input);
        input.RegisterCallback<ChangeEvent<int>>(e =>
        {
            target.Value = e.newValue;
        });
    }

    public override void SetInfo(object obj)
    {
        this.target = obj as LBSIntCharacteristic;

        labelField.value = this.target?.Label;
        input.value = this.target.Value;
    }

    protected override VisualElement CreateVisualElement()
    {
        throw new System.NotImplementedException();
    }
}
