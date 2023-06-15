using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSLocalCurrent : LBSInspector
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<LBSLocalCurrent, VisualElement.UxmlTraits> { }
    #endregion

    public VisualElement currentContent;

    public LBSLocalCurrent()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSLocalCurrent");
        visualTree.CloneTree(this);

        LBSEvents.OnSelectElementInWorkSpace += SetCurrentInfo;

        currentContent = this.Q<VisualElement>("CurrentContent");
    }

    public void SetCurrentInfo(object obj)
    {
        currentContent.Clear();

        var so = (ScriptableObject)obj;

        var editor = Editor.CreateEditor(so);


        var inspector = new IMGUIContainer(() =>
        {
            editor.OnInspectorGUI();
        });

        currentContent.Add(inspector);
    }

    public void SetInfo(LBSLayer target)
    {
        /*
        contentAssist.Clear();

        this.target = target;

        if (target.Assitants.Count <= 0)
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
        */
    }

    public override void Init(List<IManipulatorLBS> lBSManipulators, ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
    }

    public override void OnLayerChange(LBSLayer layer)
    {
        SetInfo(layer);
    }
}
