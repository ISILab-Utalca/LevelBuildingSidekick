using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using ISILab.AI.Grammar;
using ISILab.LBS.Modules;
using LBS.Components;
using Newtonsoft.Json;
using UnityEngine;

namespace ISILab.LBS.Behaviours
{
    [RequieredModule(typeof(QuestGraph))]
    public class QuestBehaviour : LBSBehaviour
    {
        [JsonIgnore]
    public GrammarTerminal ToSet { get; set; }

    public QuestGraph Graph => Owner.GetModule<QuestGraph>();


    public QuestBehaviour(Texture2D icon, string name) : base(icon, name)
    {
    }

        public override object Clone()
        {
            return new QuestBehaviour(this.Icon, this.Name);
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