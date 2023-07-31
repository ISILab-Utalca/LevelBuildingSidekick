using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSLocalBehaviours : LBSInspector 
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<LBSLocalBehaviours, VisualElement.UxmlTraits> { }
    #endregion

    private VisualElement content;
    private VisualElement noContentPanel;
    private VisualElement contentBehaviour;

    private LBSLayer target;

    public LBSLocalBehaviours()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSLocalBehaviours");
        visualTree.CloneTree(this);

        content = this.Q<VisualElement>("Content");
        this.noContentPanel = this.Q<VisualElement>("NoContentPanel");
        this.contentBehaviour = this.Q<VisualElement>("ContentBehaviour");
    }

    public void SetInfo(LBSLayer target)
    {
        contentBehaviour.Clear();

        this.target = target;

        if (target.Assitants.Count <= 0)
        {
            noContentPanel.SetDisplay(true);
        }

        foreach (var behaviour in target.Behaviours)
        {
            var so = Utility.Reflection.MakeGenericScriptable(behaviour);
            var editor = Editor.CreateEditor(so);

            var cont = new VisualElement();
            cont.style.backgroundColor = new Color(0, 0, 0, 0.1f);
            cont.SetBorder(new Color(0, 0, 0, 0.6f), 1);
            cont.SetBorderRadius(3);
            cont.SetPaddings(4);

            var inspector = new IMGUIContainer(() =>
            {
                editor.OnInspectorGUI();
            });

            cont.Add(inspector);
            contentBehaviour.Add(cont);
        }
    }

    public override void Init(List<IManipulatorLBS> lBSManipulators, MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {

    }

    public override void OnLayerChange(LBSLayer layer)
    {
        SetInfo(layer);
    }
}
