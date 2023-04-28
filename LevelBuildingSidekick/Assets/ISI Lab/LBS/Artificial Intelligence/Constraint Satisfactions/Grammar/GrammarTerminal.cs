using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GrammarTerminal : GrammarElement
{
    [SerializeField]
    string terminal;
    public string Text { get { return terminal; } set { terminal = value; } }


    public GrammarTerminal(string text)
    {
        ID = text;
        terminal = text;
    }

    public override string GetText()
    {
        return terminal;
    }

    public override List<GrammarElement> GetTerminals()
    {
        return new List<GrammarElement>() {this};
    }

    public override List<string> GetExpansionsText()
    {
        return new List<string>() { ID };
    }

    public override List<GrammarElement> GetExpansion(int index)
    {
        return new List<GrammarElement>() { this }; 
    }

    public override object Clone()
    {
        return new GrammarTerminal(Text);
    }
}
