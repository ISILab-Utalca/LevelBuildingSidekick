using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionTargetDepiction
{
    [SerializeField, SerializeReference]
    private GrammarElement grammarElement;
    [SerializeField]
    private List<string> targets = new List<string>();

    public GrammarElement GrammarElement
    {
        get => grammarElement;
        set => grammarElement = value;
    }

    public int TargetCount => targets.Count;

    public ActionTargetDepiction()
    {

    }

    public ActionTargetDepiction(GrammarElement grammarElement, List<string> characterisitcs)
    {
        this.grammarElement = grammarElement;
        this.targets = characterisitcs;
    }

    public void SetTarget(int index, string target)
    {
        targets[index] = target;
    }

    public string GetTarget(int index)
    {
        return targets[index];
    }

    public void AddTarget(string target)
    {
        targets.Add(target);
    }
}