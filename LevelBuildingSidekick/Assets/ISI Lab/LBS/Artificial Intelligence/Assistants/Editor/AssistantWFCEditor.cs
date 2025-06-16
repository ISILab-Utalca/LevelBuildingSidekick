using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Assistants;
using ISILab.LBS.Editor;
using ISILab.LBS.Editor.Windows;
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
using UnityEditor;

namespace ISILab.LBS.AI.Assistants.Editor
{
    [LBSCustomEditor("Wave Function Collapse", typeof(AssistantWFC))]
    public class AssistantWFCEditor : LBSCustomEditor, IToolProvider
    {
        private WaveFunctionCollapseManipulator collapseManipulator;

        private AssistantWFC assistant;

        private TextField presetName;
        private TextField presetsFolder;

        private ObjectField currentPreset;

        private ListView presetsList;

        public AssistantWFCEditor(object target) : base(target)
        {
            assistant = target as AssistantWFC;
            assistant.Bundle = GetExteriorBehaviour().Bundle;
            CreateVisualElement();
            assistant.SafeMode = false;
        }

        public override void SetInfo(object paramTarget)
        {
            assistant = paramTarget as AssistantWFC;
        }

        public void SetTools(ToolKit toolKit)
        {
            collapseManipulator = new WaveFunctionCollapseManipulator();
            var t1 = new LBSTool(collapseManipulator);
            t1.OnSelect += LBSInspectorPanel.ActivateAssistantTab;
            toolKit.ActivateTool(t1,assistant.OwnerLayer, assistant);
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

                System.Action invalidBundleAction = () =>
                {
                    bundleField.value = exterior.Bundle;
                    LBSMainWindow.MessageNotify("Selected bundle was invalid.", LogType.Warning);
                };

                if (bundle)
                {
                    // Get current option
                    var connections = bundle.GetChildrenCharacteristics<LBSDirection>();
                    var tags = connections.SelectMany(c => c.Connections).ToList().RemoveDuplicates();
                    if (tags.Remove("Empty")) tags.Insert(0, "Empty");

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
                        invalidBundleAction(); // set default or current if new option not valid
                    }
                }
                else
                {
                    invalidBundleAction(); // set default or current if new option not valid
                }

                assistant.Bundle = exterior.Bundle;
                ToolKit.Instance.SetActive(typeof(WaveFunctionCollapseManipulator));
                MarkDirtyRepaint();
                
            });
            
            exterior.OwnerLayer.OnChange += () =>
            {
                bundleField.SetValueWithoutNotify(exterior.Bundle);
                assistant.Bundle = exterior.Bundle;
            };

            assistant.Bundle = exterior.Bundle;

            // Copy weights from tilemap button
            var copyWeightsButton = this.Q<Button>("CopyWeights");
            copyWeightsButton.clicked += CopyWeights;

            //Save weights in a preset button
            var saveWeightsButton = this.Q<Button>("SaveWeights");
            saveWeightsButton.clicked += SaveWeights;
            presetName = this.Q<TextField>("PresetName");
            presetsFolder = this.Q<TextField>("PresetsPath");

            // Load weights from a preset
            var loadWeightsButton = this.Q<Button>("LoadWeights");
            loadWeightsButton.clicked += LoadWeights;
            currentPreset = this.Q<ObjectField>("CurrentPreset");

            // Safe Generation Mode
            var safeModeCheckbox = this.Q<Toggle>("SafeMode");
            safeModeCheckbox.RegisterValueChangedCallback(evt => { assistant.SafeMode = safeModeCheckbox.value; });

            presetsList = this.Q<ListView>("PresetsList");
            var WFCPresets = AssetDatabase.FindAssets("", new[] {presetsFolder.value})
                            .Select(guid => AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid)))
                            .Where(asset => asset != null && asset is WFCPreset)
                            .ToList();

            var itemTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
                ("Assets/ISI Lab/LBS/Artificial Intelligence/Assistants/Editor/WFCPresetElement.uxml");

            presetsList.itemsSource = WFCPresets;
            presetsList.bindItem = (element, i) =>
            {
                var obj = element.Q<ObjectField>("Element");

                var asset = WFCPresets[i];
                obj.value = asset;
            };
            presetsList.Rebuild();
            
            return this;
        }

        private void CopyWeights()
        {
            assistant.CopyWeights();
            LBSMainWindow.MessageNotify("Weights copied.");
        }

        private void SaveWeights()
        {
            assistant.SaveWeights(presetName.value, presetsFolder.value, out string endName, out WFCPreset newPreset);
            currentPreset.value = newPreset;
            LBSMainWindow.MessageNotify($"Weights saved to preset: {endName}.");
        }

        private void LoadWeights()
        {
            WFCPreset loaded = presetsList.GetRootElementForIndex(presetsList.selectedIndex)?.Q<ObjectField>("Element")?.value as WFCPreset;//currentPreset.value as WFCPreset;
            if (loaded)
            {
                assistant.LoadWeights(loaded);
                currentPreset.value = loaded;
                LBSMainWindow.MessageNotify($"Weights loaded from preset: {loaded.name}.");
            }
            else LBSMainWindow.MessageNotify($"Failed to load: no preset selected.", LogType.Warning);
        }

        private ExteriorBehaviour GetExteriorBehaviour()
        {
            ExteriorBehaviour exterior = assistant.OwnerLayer.Behaviours
                .Find(b => b is ExteriorBehaviour) as ExteriorBehaviour;
            
            return exterior;
        }
    }
}