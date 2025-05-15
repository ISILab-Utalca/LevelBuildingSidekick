using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Assistants;
using ISILab.LBS.Editor;
using ISILab.LBS.Manipulators;
using ISILab.LBS.VisualElements;
using LBS;
using LBS.Bundles;
using LBS.VisualElements;
using System.Linq;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Components;
using ISILab.LBS.Internal;
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
            assistant.Bundle = GetExteriorBehaviour().Bundle;
            CreateVisualElement();
        }

        public override void SetInfo(object target)
        {
            assistant = target as AssistantWFC;
        }

        public void SetTools(ToolKit toolKit)
        {
            toolKit.AddSeparator();

            collapseManipulator = new WaveFunctionCollapseManipulator();
            var t1 = new LBSTool(collapseManipulator);
            t1.OnSelect += LBSInspectorPanel.ActivateAssistantTab;
            t1.Init(assistant.OwnerLayer, assistant);
            toolKit.AddTool(t1);
        }

        protected sealed override VisualElement CreateVisualElement()
        {
            Clear();
  
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("AssistantWFCEditor");
            visualTree.CloneTree(this);

            var bundleField = this.Q<ObjectField>();
            bundleField.objectType = typeof(Bundle);
            bundleField.label = "Exterior Tile Bundle";
            var exterior = GetExteriorBehaviour();
            bundleField.value = exterior.Bundle;
            bundleField.RegisterValueChangedCallback(evt =>
            {
                /*
                 * No longer using assist's own bundle as it
                 * should generate for the layer's bundle
                 */ 
                 //assistant.Bundle = evt.newValue as Bundle; 

                 var bundle = evt.newValue as Bundle;
                
                 // Get current option
                 var connections = bundle.GetChildrenCharacteristics<LBSDirection>();
                 var tags = connections.SelectMany(c => c.Connections).ToList().RemoveDuplicates();
                 var indtifiers = LBSAssetsStorage.Instance.Get<LBSTag>();
                 var idents = tags.Select(s => indtifiers.Find(i => s == i.Label)).ToList().RemoveEmpties();
                
                 if (idents.Any())
                 {
                     exterior.Bundle = bundle; // valid for exterior
                     var owner = exterior.OwnerLayer;
                     owner.OnChangeUpdate(); // updates the assistant and viceversa
                 }
                 else
                 {
                     bundleField.value = exterior.Bundle; // set default or current if new option not valid
                 }
                 
                 assistant.Bundle = exterior.Bundle;
                 ToolKit.Instance.SetActive("Wave Function Collapse");
                 MarkDirtyRepaint();
                
            });
            
            exterior.OwnerLayer.OnChange += () =>
            {
                bundleField.SetValueWithoutNotify(exterior.Bundle);
                assistant.Bundle = exterior.Bundle;
            };

            assistant.Bundle = exterior.Bundle;
            
            return this;
        }

        private ExteriorBehaviour GetExteriorBehaviour()
        {
            ExteriorBehaviour exterior = assistant.OwnerLayer.Behaviours
                .Find(b => b is ExteriorBehaviour) as ExteriorBehaviour;
            
            return exterior;
        }
    }
}