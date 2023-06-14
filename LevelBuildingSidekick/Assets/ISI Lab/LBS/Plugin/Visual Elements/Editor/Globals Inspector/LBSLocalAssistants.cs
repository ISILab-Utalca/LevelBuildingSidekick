using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSLocalAssistants : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<LBSLocalAssistants, VisualElement.UxmlTraits> { }
    #endregion

    private VisualElement content;
    private VisualElement noContentPanel;
    private VisualElement contentAssist;

    private LBSLayer target;

    public LBSLocalAssistants()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSLocalAssistants");
        visualTree.CloneTree(this);

        this.content = this.Q<VisualElement>("Content");
        this.noContentPanel = this.Q<VisualElement>("NoContentPanel");
        this.contentAssist = this.Q<VisualElement>("ContentAssist");
    }

    public void SetInfo(LBSLayer target)
    {
        this.target = target;

        if(target.Assitants.Count <= 0)
        {
            noContentPanel.SetDisplay(true);
        }

        foreach (var assist in target.Assitants)
        {
            var so = Utility.Reflection.MakeGenericScriptable(assist);
            var inspector = Editor.CreateEditor(so).CreateInspectorGUI();
            contentAssist.Add(inspector);
        }
    }
}
