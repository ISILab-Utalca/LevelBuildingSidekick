using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("SecurityFairness", typeof(SecurityFairness))]
public class SecurityFairnessVE : LBSCustomEditor
{
    DynamicFoldout colliderCharacteristic;
    DynamicFoldout playerCharacteristic;

    public SecurityFairnessVE(object target) : base(target)
    {
        Add(CreateVisualElement());
        SetInfo(target);
    }

    public override void SetInfo(object target)
    {
        this.target = target;
        var eval = target as SecurityFairness;
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
        var eval = target as SecurityFairness;

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
