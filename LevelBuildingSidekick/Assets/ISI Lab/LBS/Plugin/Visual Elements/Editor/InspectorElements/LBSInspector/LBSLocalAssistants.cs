using LBS.Components;
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

    private Color colorAS = new Color(135f / 255f, 215f / 255f, 246f / 255f);

    private VisualElement content;
    private VisualElement noContentPanel;
    private VisualElement contentAssist;

    private LBSLayer target;

    private ToolKit toolkit;

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
            var type = assist.GetType();
            var ves = Utility.Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                .Where(t => t.Item2.Any(v => v.type == type));

            if (ves.Count() == 0)
            {
                Debug.LogWarning("[ISI Lab] No class marked as LBSCustomEditor found for type: " + type);
                continue;
            }

            var ve = Activator.CreateInstance(ves.First().Item1, new object[] { assist });
            if (!(ve is VisualElement))
            {
                Debug.LogWarning("[ISI Lab] " + ve.GetType() + " is not a VisualElement ");
                continue;
            }

            if (ve is IToolProvider)
            {
                ((IToolProvider)ve).SetTools(toolkit);
            }

            toolkit.ChangeActive(0);

            var content = new BehaviourContent(ve as LBSCustomEditor, assist.name, assist.icon, colorAS);
            contentContainer.Add(content);

            /*
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
            */
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
