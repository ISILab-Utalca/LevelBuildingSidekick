using ISILab.LBS.Assistants;
using ISILab.LBS.Internal;
using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Template;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
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

            Add(label);
            Add(button);

            agent.OnTermination += () =>
            {
                LBSInspectorPanel.Instance.SetTarget(agent.OwnerLayer);
               // LBSInspectorPanel.Instance.InitTabs(new List<LayerTemplate>());
                Debug.Log("Weird Init called!!!");
            };
        }
    }
}