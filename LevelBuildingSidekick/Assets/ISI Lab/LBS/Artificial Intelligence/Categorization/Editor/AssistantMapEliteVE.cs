using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CustomVisualElement(typeof(AssistantMapElite))]
public class AssistantMapEliteVE : VisualElement
{
    MapEliteWindow mapElites;
    AssistantMapElite assitant;

    public AssistantMapEliteVE(LBSAssistantAI agent)
    {
        assitant = agent as AssistantMapElite;
        var button = new Button(OpenAssitant);
        button.text = "Map Elites";
        Add(button);
    }

    public void OpenAssitant()
    {
        var mapElites = MapEliteWindow.GetWindow<MapEliteWindow>();
        mapElites.SetLayer(assitant.Owner);
    }
}
