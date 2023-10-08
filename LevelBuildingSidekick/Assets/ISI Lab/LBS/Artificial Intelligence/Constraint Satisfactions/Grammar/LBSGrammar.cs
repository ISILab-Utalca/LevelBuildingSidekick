using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ISILab/LBS/LBSGrammar")]
[System.Serializable]
public class LBSGrammar : ScriptableObject
{
    [SerializeField]
    GrammarTree grammarTree;

    [SerializeField]
    private List<ActionTargetDepiction> actions;

    public int ActionCount => actions.Count;

    public List<ActionTargetDepiction> Actions => new List<ActionTargetDepiction>(actions);

    public GrammarTree GrammarTree
    {
        get => grammarTree;
        set
        {
            grammarTree = value;
            UpdateActions();
        }
    }

    private void UpdateActions()
    {
        actions.Clear();
        foreach(var terminal in grammarTree.Terminals)
        {
            actions.Add(new ActionTargetDepiction(terminal, new List<string>()));
        }
    }

    public ActionTargetDepiction GetAction(int index)
    {
        return actions[index];
    }


}

