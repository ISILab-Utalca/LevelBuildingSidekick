using ISILab.AI.Optimization.Terminations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("GenerationNumberTermination", typeof(GenerationNumberTermination))]
public class GenerationTerminationVE : LBSCustomEditor
{
    IntegerField generations;

    public GenerationTerminationVE(object target) : base(target)
    {
        Add(CreateVisualElement());
        SetInfo(target);
    }

    public override void SetInfo(object target)
    {
        this.target = target;
        var term = target as GenerationNumberTermination;
        generations.value = term.ExpectedGenerationNumber;
    }

    protected override VisualElement CreateVisualElement()
    {
        var term = target as GenerationNumberTermination;
        var ve = new VisualElement();

        generations = new IntegerField();
        generations.label = "Generations Number";

        ve.Add(generations);

        return ve;
    }
}
