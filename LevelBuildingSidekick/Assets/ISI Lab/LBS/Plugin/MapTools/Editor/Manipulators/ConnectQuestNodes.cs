using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using LBS.Components;
using ISILab.LBS.Editor.Windows;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class ConnectQuestNodes : LBSManipulator
    {
        private QuestGraph _quest;

        private QuestNode _first;
        protected override string IconGuid => "ec280cec81783e94cb5df0b0b40dec7e";

        public ConnectQuestNodes()
        {
            Feedback = new ConnectedLine();
            Name = "Connect Quest Node";
            Description = "Click on a starting node, then release on the follow up node.";
        }



        public override void Init(LBSLayer layer, object provider = null)
        {
            base.Init(layer, provider);
            
            _quest = layer.GetModule<QuestGraph>();
        }

        protected override void OnMouseDown(VisualElement element, Vector2Int startPosition, MouseDownEvent e)
        {
            _first = _quest.GetQuestNode(startPosition);
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int endPosition, MouseUpEvent e)
        {
            var second = _quest.GetQuestNode(endPosition);
            var result = _quest.AddEdge(_first, second);
            LBSMainWindow.MessageNotify(result.Item1, result.Item2, 4);
           
            OnManipulationEnd.Invoke();
        }

    }
}