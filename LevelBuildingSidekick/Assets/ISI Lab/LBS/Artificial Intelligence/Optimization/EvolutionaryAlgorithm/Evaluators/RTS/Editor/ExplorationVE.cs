using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("Exploration", typeof(Exploration))]
public class ExplorationVE : LBSCustomEditor
{
    DynamicFoldout colliderCharacteristic;

    public ExplorationVE(object target) : base(target)
    {
        Add(CreateVisualElement());
        SetInfo(target);
    }

    public override void SetInfo(object target)
    {
        this.target = target;
        var eval = target as Exploration;
        if (eval.colliderCharacteristic != null)
        {
            colliderCharacteristic.SetInfo(eval.colliderCharacteristic);
        }
    }

    protected override VisualElement CreateVisualElement()
    {
        var ve = new VisualElement();
        var eval = target as Exploration;

        colliderCharacteristic = new DynamicFoldout(typeof(LBSCharacteristic));
        colliderCharacteristic.Label = "Collider Characteristic";

        if (eval != null && eval.colliderCharacteristic != null)
        {
            colliderCharacteristic.Data = eval.colliderCharacteristic;
        }

        colliderCharacteristic.OnChoiceSelection += () => { eval.colliderCharacteristic = colliderCharacteristic.Data as LBSCharacteristic; };

        ve.Add(colliderCharacteristic);
        return ve;
    }
}
