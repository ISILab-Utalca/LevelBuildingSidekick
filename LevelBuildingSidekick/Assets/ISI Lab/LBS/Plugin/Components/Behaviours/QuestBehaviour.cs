using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using ISILab.AI.Grammar;
using ISILab.LBS.Modules;
using LBS.Components;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

namespace ISILab.LBS.Behaviours
{
    [RequieredModule(typeof(QuestGraph))]
    public class QuestBehaviour : LBSBehaviour
    {
        [JsonIgnore]
        public GrammarTerminal ToSet { get; set; }

        public QuestGraph Graph => Owner.GetModule<QuestGraph>();


        public QuestBehaviour(VectorImage icon, string name, Color colorTint) : base(icon, name, colorTint)
        {
        }

        public override void OnGUI()
        {

        }
        
        public override object Clone()
        {
            return new QuestBehaviour(this.Icon, this.Name, this.ColorTint);
            //throw new System.NotImplementedException();
        }

        public override void OnAttachLayer(LBSLayer layer)
        {
            Owner = layer;
        }

        public override void OnDetachLayer(LBSLayer layer)
        {
        }
    }
}