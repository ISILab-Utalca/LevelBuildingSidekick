using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using ISILab.LBS.Characteristics;

[LBSCustomEditorAttribute("Weigths", typeof(LBSDirection))]
public class LBSDirectionEditor : LBSCustomEditor
{
    public LBSDirectionEditor()
    {

    }

    public LBSDirectionEditor(object target) : base(target)
    {
        SetInfo(target);
    }

    public override void SetInfo(object obj)
    {
        var target = obj as LBSDirection;

        if (target == null)
            return;
    }

    protected override VisualElement CreateVisualElement()
    {
        throw new System.NotImplementedException();
    }
}