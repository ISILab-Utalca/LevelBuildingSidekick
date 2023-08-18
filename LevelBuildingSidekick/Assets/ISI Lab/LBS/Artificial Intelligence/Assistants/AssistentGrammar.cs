using LBS.Assisstants;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class AssistentGrammar : LBSAssistantAI
{
    public override object Clone()
    {
        return new AssistentGrammar();
    }

    public override void Execute()
    {
        Debug.Log("No implementado Grammar 'Execute()'");
    }

    public override void OnAdd(LBSLayer layer)
    {
        Debug.Log("No implementado Grammar 'Init(layer)'");
    }
}