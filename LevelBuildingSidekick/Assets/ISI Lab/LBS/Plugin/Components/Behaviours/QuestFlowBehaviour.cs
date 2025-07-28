using ISILab.AI.Grammar;
using ISILab.LBS.Modules;
using LBS.Components;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.Behaviours
{
    [RequieredModule(typeof(QuestGraph))]
    public class QuestFlowBehaviour : LBSBehaviour
    {
        public QuestGraph Graph => OwnerLayer.GetModule<QuestGraph>();
        
        public QuestFlowBehaviour(VectorImage icon, string name, Color colorTint) : base(icon, name, colorTint)
        {
        }

        public override void OnGUI()
        {

        }
        
        public override object Clone()
        {
            return new QuestFlowBehaviour(this.Icon, this.Name, this.ColorTint);
        }

        public override void OnAttachLayer(LBSLayer layer)
        {
            OwnerLayer = layer;
        }

        public override void OnDetachLayer(LBSLayer layer)
        {
        }
    }
}