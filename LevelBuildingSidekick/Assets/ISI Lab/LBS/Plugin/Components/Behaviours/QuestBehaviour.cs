using System;
using ISILab.AI.Grammar;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components;
using Newtonsoft.Json;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

namespace ISILab.LBS.Behaviours
{
    [RequieredModule(typeof(QuestGraph))]
    public class QuestBehaviour : LBSBehaviour
    {
        [JsonIgnore]
        public GrammarTerminal ToSet { get; set; }

        public QuestGraph Graph => OwnerLayer.GetModule<QuestGraph>();
        
        public QuestBehaviour(VectorImage icon, string name, Color colorTint) : base(icon, name, colorTint)
        {
        }

        public override object[] RetrieveNewTiles()
        {
            QuestNode[] qn = Graph.RetrieveNewNodes();
            QuestEdge[] qe = Graph.RetrieveNewEdges();
            
            object[] o = new object[qn.Length + qe.Length];
            o.SetValue(qn, 0);
            o.SetValue(qe, qn.Length);
            
            return o;
        }

        public override object[] RetrieveExpiredTiles()
        {
            QuestNode[] qn = Graph.RetrieveExpiredNodes();
            QuestEdge[] qe = Graph.RetrieveExpiredEdges();
            
            object[] o = new object[qn.Length + qe.Length];
            for (int i = 0; i < qn.Length; i++)
            {
                o[i] =  qn[i];
            }
            for (int i = qn.Length; i < qn.Length + qe.Length; i++)
            {
                o[i] = qe[i - qn.Length]; // subtracting length of nodes, because counter adds edges
            }   
            return o;
        }

        public override void OnGUI()
        {

        }
        
        public override object Clone()
        {
            return new QuestBehaviour(this.Icon, this.Name, this.ColorTint);
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