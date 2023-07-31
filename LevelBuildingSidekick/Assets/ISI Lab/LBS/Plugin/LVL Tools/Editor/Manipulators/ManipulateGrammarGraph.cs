using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ManipulateGrammarGraph : LBSManipulator
{
    public GrammarElement actionToSet;
    protected LBSGrammarGraph module;

    public override void Init(MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {
        this.module = layer.GetModule<LBSGrammarGraph>();
        MainView = view;
    }
}
