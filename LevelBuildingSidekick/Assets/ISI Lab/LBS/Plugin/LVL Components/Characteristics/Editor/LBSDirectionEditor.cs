using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public abstract class LBSCustomEditor : VisualElement
{
    public abstract void SetInfo(object target);
}

[LBSCustomEditorAttribute(typeof(LBSDirection))]
public class LBSDirectionEditor : LBSCustomEditor
{
    public LBSDirectionEditor()
    {

    }

    public override void SetInfo(object obj)
    {
        var target = obj as LBSDirection;

        if (target == null)
            return;

        var assetsW = target.Weights;

        for (int i = 0; i < assetsW.Count; i++)
        {
            var current = assetsW[i];

            var foldout = new Foldout();
            foldout.text = assetsW[i].target.name;
            this.Add(foldout);

            var slider = new Slider();
            slider.RegisterCallback<ChangeEvent<float>>((v) =>
            {
                current.weigth = v.newValue;
            });
            foldout.Add(slider);
        }

    }
}
