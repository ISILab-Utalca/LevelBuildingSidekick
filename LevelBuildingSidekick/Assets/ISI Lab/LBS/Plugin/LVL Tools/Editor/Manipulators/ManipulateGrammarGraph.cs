using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ManipulateGrammarGraph : LBSManipulator
{
    public GrammarElement actionToSet;
    protected LBSGrammarGraph module;

    public override void Init(ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        this.module = layer.GetModule<LBSGrammarGraph>();
        MainView = view;
    }
}
