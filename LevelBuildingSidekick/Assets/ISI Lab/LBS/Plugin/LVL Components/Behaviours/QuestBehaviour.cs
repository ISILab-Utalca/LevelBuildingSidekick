using LBS.Behaviours;
using LBS.Components;
using LBS.Components.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequieredModule(typeof(LBSGraph), typeof (LBSGrammarGraph))]
public class QuestBehaviour : LBSBehaviour
{
    #region CONSTRUCTORS
    public QuestBehaviour(Texture2D icon, string name) : base(icon, name) { }

    public void AddNode(LBSNode n, QuestStep a)
    {
        throw new NotImplementedException();
    }
    #endregion

    public override object Clone()
    {
        return new QuestBehaviour(this.Icon, this.Name);
    }

    public override void OnAttachLayer(LBSLayer layer)
    {

    }

    public override void OnDetachLayer(LBSLayer layer)
    {

    }
}
