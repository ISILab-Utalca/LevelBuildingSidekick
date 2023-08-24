using LBS.Settings;
using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor("HillClimbingAssistant", typeof(HillClimbingAssistant))]
public class HillClimbingAssistantEditor : LBSCustomEditor , IToolProvider
{
    private readonly Color AssColor = LBSSettings.Instance.view.assitantsColor;

    private HillClimbingAssistant hillClimbing;

    // Manipulators
    //private SetZoneConnection

    public override void SetInfo(object target)
    {
        throw new System.NotImplementedException();
    }

    public void SetTools(ToolKit group)
    {
        throw new System.NotImplementedException();
    }

    protected override VisualElement CreateVisualElement()
    {
        throw new System.NotImplementedException();
    }
}
