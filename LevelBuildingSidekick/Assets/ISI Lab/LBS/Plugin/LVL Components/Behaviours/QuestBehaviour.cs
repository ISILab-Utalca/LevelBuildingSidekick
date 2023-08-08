using LBS.Components;
using LBS.Components.Graph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequieredModule(typeof(LBSGraph), typeof(LBSGrammarGraph))]
public class QuestBehaviour : LBSBehaviour
{
    #region CONSTRUCTORS
    public QuestBehaviour(Texture2D icon, string name) : base(icon, name) { }
    #endregion

    public override object Clone()
    {
        return new QuestBehaviour(this.icon, this.name);
    }

    public override void Init(LBSLayer layer)
    {

    }
}
