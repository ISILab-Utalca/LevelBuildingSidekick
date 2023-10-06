using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

//[LBSCustomEditor("ExhaustiveRemoveGene", typeof(ExhaustiveRemoveGene))]
public class ExhaustiveRemoveGeneVE : LBSCustomEditor
{

    public ExhaustiveRemoveGeneVE(object target) : base(target)
    {
        Add(CreateVisualElement());
        SetInfo(target);
    }

    public override void SetInfo(object target)
    {
        this.target = target;
        var mut = target as ExhaustiveRemoveGene;
    }

    protected override VisualElement CreateVisualElement()
    {
        this.Add(new Label("Exhaustive Remove Gene"));
        return this;
    }

}
