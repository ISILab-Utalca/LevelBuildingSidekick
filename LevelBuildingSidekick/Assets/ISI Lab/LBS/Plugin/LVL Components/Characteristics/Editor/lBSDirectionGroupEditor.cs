using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("Connections group", typeof(LBSDirectionedGroup))]
public class LBSDirectionGroupEditor : LBSCustomEditor
{
    public TextField labelField;
    public VisualElement content;

    //ListView weights<>

    public LBSDirectionGroupEditor()
    {

    }

    public LBSDirectionGroupEditor(object target) : base(target)
    {
        Add(CreateVisualElement());
        SetInfo(target);
    }

    public override void SetInfo(object obj)
    {
        this.target = obj;
        var target = obj as LBSDirectionedGroup;

        labelField.value = target.Label;

        content = new VisualElement();
        this.Add(content);
        var weights = target.Weights;
        for (int i = 0; i < weights.Count; i++)
        {
            var current = weights[i];
            var box = new VisualElement();
            content.Add(box);

            box.Add(new Label(current.target.name));

            var slider = new Slider();
            box.Add(slider);
            slider.lowValue = 0;
            slider.highValue = 1;
            slider.value = current.weigth;
            slider.RegisterCallback<ChangeEvent<float>>(e => {
                current.weigth = e.newValue;
            });

        }
    }

    protected override VisualElement CreateVisualElement()
    {
        var target = this.target as LBSDirectionedGroup;

        var ve = new VisualElement();
        labelField = new TextField();
        labelField.RegisterCallback<BlurEvent>(e => {
            target.Label = labelField.value;
        });

        ve.Add(labelField);
        return ve;
    }
}
