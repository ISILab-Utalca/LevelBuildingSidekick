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
        public string ActionToSet { get; set; }

        public QuestGraph Graph => OwnerLayer.GetModule<QuestGraph>();
        
        public QuestBehaviour(VectorImage icon, string name, Color colorTint) : base(icon, name, colorTint)
        {
        }
        

        public override void OnGUI()
        {

        }
        
        public override object Clone()
        {
            return new QuestBehaviour(Icon, Name, ColorTint);
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