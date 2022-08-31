using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class LBSBaseView : GraphView
{
    protected LBSViewController controller;

    public void ClearView()
    {
        DeleteElements(graphElements);
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        controller?.contextActions.ForEach(a => evt.menu.AppendAction(a.name, (dma) => a.action?.Invoke(dma, this, evt)));
    }

    public abstract void Populate<T>(T value) where T : LBSRepesentationData;
}