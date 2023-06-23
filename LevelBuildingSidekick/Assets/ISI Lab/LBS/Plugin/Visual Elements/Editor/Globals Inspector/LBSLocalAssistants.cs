using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSLocalAssistants : LBSInspector
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
        contentAssist.Clear();

        this.target = target;

        if(target.Assitants.Count <= 0)
        {
            noContentPanel.SetDisplay(true);
        }

        foreach (var assist in target.Assitants)
        {
            var so = Utility.Reflection.MakeGenericScriptable(assist);
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
            contentAssist.Add(cont);
        }
    }

    public override void Init(List<IManipulatorLBS> lBSManipulators, ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {

    }

    public override void OnLayerChange(LBSLayer layer)
    {
        SetInfo(layer);
    }
}
