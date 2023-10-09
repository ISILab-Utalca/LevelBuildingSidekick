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
    private GrammarElement grammarElement;

    [JsonIgnore]
    public GrammarElement GrammarElement => grammarElement;

    #region CONSTRUCTOR
    public QuestStep()
    {
    }

    public QuestStep(GrammarElement grammarElement)
    {
        this.grammarElement = grammarElement;
    }
    #endregion;

    #region METHODS
    public object Clone()
    {
        return new QuestStep(grammarElement.Clone() as GrammarElement);
    }
    #endregion
}
