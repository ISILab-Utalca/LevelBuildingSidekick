using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Editor.Windows;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class ConnectQuestNodes : LBSManipulator
    {
        QuestGraph quest;

        public QuestNode first;
        protected override string IconGuid { get => "ec280cec81783e94cb5df0b0b40dec7e"; }
        
        public ConnectQuestNodes() : base()
        {
            feedback = new ConnectedLine();
            name = "Connect Quest Node";
            description = "Click on a starting node, then release on the follow up node.";
        }



        public override void Init(LBSLayer layer, object provider)
        {
            base.Init(layer, provider);
            
            quest = layer.GetModule<QuestGraph>();
        }

        protected override void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e)
        {
            first = quest.GetQuestNode(startPosition);
        }

        protected override void OnMouseUp(VisualElement paramTarget, Vector2Int endPosition, MouseUpEvent e)
        {
            var second = quest.GetQuestNode(endPosition);
            var result = quest.AddEdge(first, second);
            LBSMainWindow.MessageNotify(result.Item1, result.Item2, 4);
           
            OnManipulationEnd.Invoke();
        }

    }
}