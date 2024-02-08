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

    private Vector2Int nodeSize = new Vector2Int(3,1);

    [JsonIgnore]
    public GrammarTerminal ToSet { get; set; }

    public QuestGraph Graph => Owner.GetModule<QuestGraph>();

    public Vector2Int NodeSize => nodeSize;

    public QuestBehaviour(Texture2D icon, string name) : base(icon, name)
    {
        nodeSize = new Vector2Int(3,1);
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
