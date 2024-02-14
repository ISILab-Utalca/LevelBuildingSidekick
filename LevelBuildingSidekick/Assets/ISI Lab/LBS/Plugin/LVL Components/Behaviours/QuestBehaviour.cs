using LBS.Behaviours;
using LBS.Components;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

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
        throw new System.NotImplementedException();
    }

    public override void OnAttachLayer(LBSLayer layer)
    {
        Owner = layer;
        //throw new System.NotImplementedException();
    }

    public override void OnDetachLayer(LBSLayer layer)
    {
        //throw new System.NotImplementedException();
    }
}
