using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("ExhaustiveMoveGene", typeof(ExhaustiveMoveGene))]
public class ExhaustiveMoveGeneVE : LBSCustomEditor
{
    IntegerField range;

    public ExhaustiveMoveGeneVE(object target) : base(target)
    {
        CreateVisualElement();
        SetInfo(target);
    }

    public override void SetInfo(object target)
    {
        this.target = target;
        var mut = target as ExhaustiveMoveGene;

        range.value = mut.Range;
        range.RegisterValueChangedCallback(e => 
        {
            if (e.newValue < 1)
                range.SetValueWithoutNotify(1);
            mut.Range = e.newValue; 
        });
    }

    protected override VisualElement CreateVisualElement()
    {
        range = new IntegerField();
        range.label = "Movement Range";
        this.Add(range);

        return this;
    }
}
