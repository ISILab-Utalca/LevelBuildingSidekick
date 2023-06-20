using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CustomVisualElement(typeof(AssistantMapElite))]
public class AssistantMapEliteVE : VisualElement
{
    MapEliteWindow mapElites;
    public AssistantMapEliteVE(LBSAssistantAI agent)
    {
        var assitant = agent as AssistantMapElite;
        var mapElites = MapEliteWindow.GetWindow<MapEliteWindow>();
        mapElites.SetLayer(assitant.Owner);
    }
}
