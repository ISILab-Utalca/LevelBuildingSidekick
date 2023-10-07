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

public class LBSLocalAssistants : LBSInspector
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<LBSLocalAssistants, VisualElement.UxmlTraits> { }
    #endregion

    private Color color => LBSSettings.Instance.view.assitantsColor;

    private VisualElement content;
    private VisualElement noContentPanel;
    private VisualElement contentAssist;

    private List<LBSCustomEditor> editores = new List<LBSCustomEditor>();

    private LBSLayer target;

    private ToolKit toolkit => ToolKit.Instance;

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
            return;
        }

        noContentPanel.SetDisplay(false);

        foreach (var assist in target.Assitants)
        {
            var type = assist.GetType();
            var ves = Utility.Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                .Where(t => t.Item2.Any(v => v.type == type));

            if (ves.Count() == 0)
            {
                Debug.LogWarning("[ISI Lab] No class marked as LBSCustomEditor found for type: " + type);
                continue;
            }

            var ovg = ves.First().Item1;
            var ve = Activator.CreateInstance(ovg, new object[] { assist });
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

            var content = new BehaviourContent(ve as LBSCustomEditor, assist.Name, assist.Icon, color);
            contentAssist.Add(content);
        }
    }

    public override void Repaint()
    {
        foreach (var ve in editores)
        {
            ve?.Repaint();
        }
    }

    public override void Init( MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {
        throw new NotImplementedException();
    }

    public override void OnLayerChange(LBSLayer layer)
    {
        SetInfo(layer);
        ToolKit.Instance.SetActive(0);
    }
}
