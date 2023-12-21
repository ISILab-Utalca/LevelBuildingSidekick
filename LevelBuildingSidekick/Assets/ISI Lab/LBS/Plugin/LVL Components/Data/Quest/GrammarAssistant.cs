using LBS.Assisstants;
using LBS.Components;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequieredModule(typeof(LBSQuestGraph))]
public class GrammarAssistant : LBSAssistant
{
    [JsonIgnore]
    public LBSQuestGraph Quest => Owner.GetModule<LBSQuestGraph>();

    public GrammarAssistant(Texture2D icon, string name) : base(icon, name)
    {
    }

    public void CheckGrammar()
    {
        var grammar = Quest.Grammar;

        var root = Quest.Root;


    }

    public void CheckMap()
    {

    }

    public override void Execute()
    {
        throw new NotImplementedException();
    }

    public override object Clone()
    {
        throw new NotImplementedException();
    }

    public void CheckNode()
    {

    }
}
