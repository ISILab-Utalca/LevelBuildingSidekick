using LBS.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CustomVisualElement(typeof(AssistantHillClimbing))]
public class SchemaHCVE : VisualElement
{
    AssistantHillClimbing agent;
    VisualElement log = new VisualElement();

    static double Nlog = 0;
    static double NNlog = 0;
    static double Elog = 0;
    static double generationsRun = 0;
    static double totalTime = 0;

    public SchemaHCVE(LBSAssistantAI agent)
    {
        this.agent = agent as AssistantHillClimbing;
        if (this.agent == null)
            return;

        var label = new Label(agent.GetType().Name);


        var button = new Button(this.agent.Execute);
        button.text = "Run";

        this.Add(label);
        this.Add(button);
        
    }
}
