using LBS.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CustomVisualElement(typeof(SchemaHCAgent))]
public class SchemaHCVE : VisualElement
{
    SchemaHCAgent agent;
    VisualElement log = new VisualElement();

    static double Nlog = 0;
    static double NNlog = 0;
    static double Elog = 0;
    static double generationsRun = 0;
    static double totalTime = 0;

    public SchemaHCVE(LBSAIAgent agent)
    {
        this.agent = agent as SchemaHCAgent;
        if (this.agent == null)
            return;

        var label = new Label(agent.GetType().Name);


        var button = new Button(this.agent.RunExperiment);
        button.text = "Run Experiment";

        this.Add(label);
        this.Add(button);
        
    }
}
