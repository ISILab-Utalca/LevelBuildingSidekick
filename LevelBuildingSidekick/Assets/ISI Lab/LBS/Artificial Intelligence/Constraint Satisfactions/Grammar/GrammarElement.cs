using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class GrammarElement : ICloneable
{

    public GrammarElement(){ }

    [SerializeField]
    string id;
    public string ID 
    {
        get => id;
        set => id = value;
    }
    public abstract string GetText();
    public abstract List<GrammarElement> GetTerminals();

    public abstract List<string> GetExpansionsText();

    public abstract List<GrammarElement> GetExpansion(int index);

    public abstract object Clone();
}
