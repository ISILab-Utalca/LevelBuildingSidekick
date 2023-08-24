using LBS.AI;
using LBS.Assisstants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CustomVisualElement(typeof(HillClimbingAssistant))]
public class SchemaHCVE : VisualElement
{
    HillClimbingAssistant agent;

    public SchemaHCVE(LBSAssistant agent)
    {
        this.agent = agent as HillClimbingAssistant;
        if (this.agent == null)
            return;

        var label = new Label(agent.GetType().Name);


        var button = new Button(this.agent.Execute);
        button.text = "Run";

        this.Add(label);
        this.Add(button);
        
    }
}
