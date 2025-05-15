using ISILab.LBS.Modules;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemoveQuestConnection : LBSManipulator
    {
        QuestGraph quest;
        protected override string IconGuid { get => "b534f3f3d94bf1349babd81aa035d583"; }
        
        public RemoveQuestConnection() : base()
        {
            name = "Remove Quest Connection";
            description = "Click a connection line between nodes to remove it.";
        }
        
        public override void Init(LBSLayer layer, object provider)
        {
            base.Init(layer, provider);
            
            quest = layer.GetModule<QuestGraph>();
        }
        
        protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
        {
            quest.RemoveEdge(endPosition, 50);
            OnManipulationEnd.Invoke();
        }
    }
}