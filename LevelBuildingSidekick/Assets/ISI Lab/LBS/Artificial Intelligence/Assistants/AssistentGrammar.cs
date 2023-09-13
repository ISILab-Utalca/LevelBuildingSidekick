using LBS.Assisstants;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class AssistentGrammar : LBSAssistant
{
    public AssistentGrammar(Texture2D icon, string name) : base(icon, name)
    {
    }

    public override object Clone()
    {
        return new AssistentGrammar(this.Icon, this.Name);
    }

    public override void Execute()
    {
        Debug.Log("No implementado Grammar 'Execute()'");
    }
}