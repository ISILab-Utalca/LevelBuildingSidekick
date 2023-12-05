using LBS.Components.Graph;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestStep : ICloneable
{
    [SerializeField, SerializeReference, JsonRequired]
    private GrammarTerminal grammarElement;

    [JsonIgnore]
    public GrammarTerminal GrammarElement => grammarElement;


    [SerializeField, SerializeReference, JsonRequired]
    object target;

    public object Target => target;

    #region CONSTRUCTOR
    public QuestStep()
    {
    }

    public QuestStep(GrammarTerminal grammarElement)
    {
        this.grammarElement = grammarElement;
    }
    #endregion;

    #region METHODS
    public object Clone()
    {
        return new QuestStep(grammarElement.Clone() as GrammarTerminal);
    }
    #endregion
}
