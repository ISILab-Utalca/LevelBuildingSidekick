using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

//[LBSCustomEditor("ExhaustiveAddGene", typeof(ExhaustiveAddGene))]
public class ExhaustiveAddGeneVE : LBSCustomEditor
{
    public ExhaustiveAddGeneVE(object target) : base(target)
    {
        Add(CreateVisualElement());
        SetInfo(target);
    }

    public override void SetInfo(object target)
    {
        this.target = target;
        var mut = target as ExhaustiveAddGene;
    }

    protected override VisualElement CreateVisualElement()
    {
        this.Add(new Label("Exhaustive Add Gene"));
        return this;
    }
}
