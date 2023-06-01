using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("Connections group", typeof(LBSDirectionedGroup))]
public class lBSDirectionGroupEditor : LBSCustomEditor
{
    public TextField labelField;
    public VisualElement content;

    private LBSDirectionedGroup target;
    public lBSDirectionGroupEditor()
    {

    }

    public override void SetInfo(object obj)
    {
        this.target = obj as LBSDirectionedGroup;

        labelField = new TextField();
        this.Add(labelField);
        labelField.RegisterCallback<BlurEvent>(e => {
            target.Label = labelField.value;
        });

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


}
