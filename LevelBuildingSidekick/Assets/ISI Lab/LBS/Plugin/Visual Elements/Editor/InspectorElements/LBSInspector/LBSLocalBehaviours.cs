using LBS.Behaviours;
using LBS.Components;
using LBS.VisualElements;
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

    private Color colorBH = new Color(135f / 255f, 215f / 255f, 246f / 255f);

    private VisualElement content;
    private VisualElement noContentPanel;
    private VisualElement contentBehaviour;

    private LBSLayer target;

    private ToolKit toolkit;

    public LBSLocalBehaviours()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSLocalBehaviours");
        visualTree.CloneTree(this);

        content = this.Q<VisualElement>("Content");
        this.noContentPanel = this.Q<VisualElement>("NoContentPanel");
        this.contentBehaviour = this.Q<VisualElement>("ContentBehaviour");

        toolkit = ToolKit.Instance;
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

        foreach (var behaviour in target.Behaviours)
        {
            var type = behaviour.GetType();
            var ves = Utility.Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                .Where(t => t.Item2.Any(v => v.type == type)).ToList();

            if (ves.Count() == 0)
            {
                Debug.LogWarning("[ISI Lab] No class marked as LBSCustomEditor found for type: " + type);
                continue;
            }

            var ovg = ves.First().Item1;
            var ve = Activator.CreateInstance(ovg, new object[] { behaviour });
            if (!(ve is VisualElement))
            {
                Debug.LogWarning("[ISI Lab] " + ve.GetType() + " is not a VisualElement ");
                continue;
            }

            if(ve is IToolProvider)
            {
                ((IToolProvider)ve).SetTools(toolkit);
            }

            toolkit.ChangeActive(0);

            var content = new BehaviourContent(ve as LBSCustomEditor, behaviour.Name, behaviour.Icon, colorBH);
            contentContainer.Add(content);

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
