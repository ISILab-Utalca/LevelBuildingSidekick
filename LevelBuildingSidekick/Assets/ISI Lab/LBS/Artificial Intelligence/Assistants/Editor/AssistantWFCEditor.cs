using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Assistants;
using ISILab.LBS.Editor;
using ISILab.LBS.Manipulators;
using ISILab.LBS.VisualElements;
using LBS;
using LBS.Bundles;
using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.AI.Assistants.Editor
{
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
            assistant = target as AssistantWFC;
        }

        public void SetTools(ToolKit toolKit)
        {
            Texture2D icon;

            toolKit.AddSeparator(10);

            // Wave function collapse
            icon = Resources.Load<Texture2D>("Icons/Assistans/Assistans_WaveFunctionCollapse");
            this.collapseManipulator = new WaveFunctionCollapseManipulator();
            var t1 = new LBSTool(icon, "Wave function collapse", collapseManipulator);
            t1.OnSelect += () => LBSInspectorPanel.ShowInspector(LBSInspectorPanel.AssistantsTab);
            t1.Init(assistant.Owner, assistant);
            toolKit.AddTool(t1);
        }

        protected override VisualElement CreateVisualElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("AssistantWFCEditor");
            visualTree.CloneTree(this);

            var field = this.Q<ObjectField>();
            field.value = assistant.Bundle;
            field.RegisterValueChangedCallback(evt =>
            {
                assistant.Bundle = evt.newValue as Bundle;
                ToolKit.Instance.SetActive("Wave function collapse");
            });

            return this;
        }
    }
}