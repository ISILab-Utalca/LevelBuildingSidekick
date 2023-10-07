using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("SecurityFairness", typeof(SafetyFairness))]
public class SafetyFairnessVE : LBSCustomEditor
{
    DynamicFoldout colliderCharacteristic;
    DynamicFoldout playerCharacteristic;

    public SafetyFairnessVE(object target) : base(target)
    {
        Add(CreateVisualElement());
        SetInfo(target);
    }

    public override void SetInfo(object target)
    {
        this.target = target;
        var eval = target as SafetyFairness;
        if (eval.colliderCharacteristic != null)
        {
            colliderCharacteristic.SetInfo(eval.colliderCharacteristic);
        }
        if (eval.playerCharacteristic != null)
        {
            playerCharacteristic.SetInfo(eval.playerCharacteristic);
        }
    }

    protected override VisualElement CreateVisualElement()
    {
        var ve = new VisualElement();
        var eval = target as SafetyFairness;

        colliderCharacteristic = new DynamicFoldout(typeof(LBSCharacteristic));
        colliderCharacteristic.Label = "Collider Characteristic";

        if (eval != null && eval.colliderCharacteristic != null)
        {
            colliderCharacteristic.Data = eval.colliderCharacteristic;
        }

        colliderCharacteristic.OnChoiceSelection += () => { eval.colliderCharacteristic = colliderCharacteristic.Data as LBSCharacteristic; };

        playerCharacteristic = new DynamicFoldout(typeof(LBSCharacteristic));
        playerCharacteristic.Label = "Player Characteristic";

        if (eval != null && eval.playerCharacteristic != null)
        {
            playerCharacteristic.Data = eval.playerCharacteristic;
        }

        playerCharacteristic.OnChoiceSelection += () => { eval.playerCharacteristic = playerCharacteristic.Data as LBSCharacteristic; };


        ve.Add(colliderCharacteristic);
        ve.Add(playerCharacteristic);

        return ve;
    }
}
