using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        if (target.Behaviours.Count <= 0)
        {
            noContentPanel.SetDisplay(true);
            return;
        }

        foreach (var b in target.Behaviours)
        {
            var type = b.GetType();
            var ves = Utility.Reflection.GetClassesWith<LBSCustomEditorAttribute>().Where(t => t.Item2.Any(v => v.type == type));
            if (ves.Count() == 0)
            {
                Debug.LogWarning("[ISI Lab] No class marked as LBSCustomEditor found for type: " + type);
                continue;
            }

            var ve = Activator.CreateInstance(ves.First().Item1, new object[] { b });
            if (!(ve is VisualElement))
            {
                Debug.LogWarning("[ISI Lab] " + ve.GetType() + " is not a VisualElement ");
                continue;
            }

            contentContainer.Add(ve as VisualElement);
        }
        /*
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
        }*/
    }

    public override void Init(List<IManipulatorLBS> lBSManipulators, MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {

    }

    public override void OnLayerChange(LBSLayer layer)
    {
        SetInfo(layer);
    }
}
