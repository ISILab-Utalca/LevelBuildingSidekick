using LBS;
using LBS.Bundles;
using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

[LBSCustomEditor("Wave Function Collapse", typeof(AssistantWFC))]
public class AssistantWFCEditor : LBSCustomEditor, IToolProvider
{
    private WaveFunctionCollapseManipulator collapseManipulator;

    private AssistantWFC assistant;

    public AssistantWFCEditor(object target) : base(target)
    {
        assistant = target as AssistantWFC;

        CreateVisualElement();
    }


    public override void SetInfo(object target)
    {
        throw new System.NotImplementedException();
    }

    public void SetTools(ToolKit toolKit)
    {
        Texture2D icon;

        toolKit.AddSeparator(10);

        // Wave function collapse
        icon = Resources.Load<Texture2D>("Icons/Assistans/Assistans_WaveFunctionCollapse");
        this.collapseManipulator = new WaveFunctionCollapseManipulator();
        var t1 = new LBSTool(icon, "Wave function collapse", collapseManipulator);
        t1.OnSelect += () => LBSInspectorPanel.ShowInspector("Local", "Assistants");
        t1.Init(assistant.Owner, assistant);
        toolKit.AddTool(t1);
    }

    protected override VisualElement CreateVisualElement()
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("AssistantWFCEditor");
        visualTree.CloneTree(this);

        var field = this.Q<ObjectField>();
        field.value = assistant.TargetBundle;
        field.RegisterValueChangedCallback(evt =>
        {
            assistant.TargetBundle = evt.newValue as Bundle;
            ToolKit.Instance.SetActive("Wave function collapse");
        });

        return this;
    }
}
