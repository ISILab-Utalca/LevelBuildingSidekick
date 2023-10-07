using LBS.Behaviours;
using LBS.Components;
using LBS.Settings;
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

    #region FIELDS
    private LBSLayer target;
    #endregion

    #region VIEW FIELDS
    private VisualElement content;
    private VisualElement noContentPanel;
    private VisualElement contentBehaviour;

    private List<LBSCustomEditor> editores = new List<LBSCustomEditor>();
    #endregion

    #region PROPERTIES
    public Color color;//=> LBSSettings.Instance.view.behavioursColor;
    private ToolKit toolkit => ToolKit.Instance;
    #endregion

    #region CONSTRUCTORS
    public LBSLocalBehaviours()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSLocalBehaviours");
        visualTree.CloneTree(this);

        content = this.Q<VisualElement>("Content");
        this.noContentPanel = this.Q<VisualElement>("NoContentPanel");
        this.contentBehaviour = this.Q<VisualElement>("ContentBehaviour");
    }
    #endregion

    #region METHODS
    public void SetInfo(LBSLayer target)
    {
        contentBehaviour.Clear();

        this.target = target;

        if (target.Behaviours.Count <= 0)
        {
            noContentPanel.SetDisplay(true);
            return;
        }

        noContentPanel.SetDisplay(false);

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

            editores.Add(ve as LBSCustomEditor);


            if (ve is IToolProvider)
            {
                ((IToolProvider)ve).SetTools(toolkit);
            }

            var content = new BehaviourContent(ve as LBSCustomEditor, behaviour.Name, behaviour.Icon, color);
            contentBehaviour.Add(content);

        }
    }

    public override void OnLayerChange(LBSLayer layer)
    {
        SetInfo(layer);
        toolkit.SetActive(0);
    }

    public override void Repaint()
    {
        foreach (var ve in editores)
        {
            ve?.Repaint();
        }
    }

    public override void Init(MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {
        throw new NotImplementedException();
    }
    #endregion
}
