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
            var captureWeightsButton = this.Q<Button>("CopyWeights");
            captureWeightsButton.clicked += CaptureWeights;

            //Save weights in a preset button
            var saveWeightsButton = this.Q<Button>("SaveWeights");
            saveWeightsButton.clicked += SaveWeights;
            presetName = this.Q<TextField>("PresetName");
            presetsFolder = this.Q<TextField>("PresetsPath");
            presetsFolder.focusable = false; // This field needs to be reworked. Meanwhile it'll remain disabled.

            // Load weights from a preset
            var loadWeightsButton = this.Q<Button>("LoadWeights");
            loadWeightsButton.clicked += LoadWeights;
            currentPreset = this.Q<ObjectField>("CurrentPreset");
            currentPreset.value = null;

            // Safe Generation Mode
            var safeModeCheckbox = this.Q<Toggle>("SafeMode");
            safeModeCheckbox.RegisterValueChangedCallback(evt => 
            { 
                assistant.SafeMode = safeModeCheckbox.value;
                if (!safeModeCheckbox.value)
                    LBSMainWindow.MessageNotify("Safe generation disabled. Some tiles may not be generated. Ensure you have enough variety of bundles for exteriors.", LogType.Warning, 7);
            });
            safeModeCheckbox.SetValueWithoutNotify(assistant.SafeMode);

            presetsList = this.Q<ListView>("PresetsList");
            UpdatePresetsList();
            
            return this;
        }

        private void CaptureWeights()
        {
            if(assistant.CaptureWeights(out string errMsg))
                LBSMainWindow.MessageNotify("Current map weights captured.");
            else LBSMainWindow.MessageNotify(errMsg, LogType.Warning);
        }

        private void SaveWeights()
        {
            bool saved = assistant.SaveWeights(presetName.value, presetsFolder.value, out string endName, out WFCPreset newPreset, out string errMsg);
            if(saved)
            {
                UpdatePresetsList();
                currentPreset.value = newPreset;
                presetsList.SetSelection(presetsList.itemsSource.IndexOf(newPreset));
                LBSMainWindow.MessageNotify($"Weights saved as preset: {endName}.");
            }
            else if(!string.IsNullOrEmpty(errMsg))
                LBSMainWindow.MessageNotify(errMsg, LogType.Warning);
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
            else LBSMainWindow.MessageNotify("Failed to load: you must select a non-null preset from the list or create a new one.", LogType.Warning, 5);
        }

        private void UpdatePresetsList()
        {
            //Debug.Log("Update Presets List");
            var WFCPresets = AssetDatabase.FindAssets("", new[] { presetsFolder.value });
            var a = WFCPresets.Select(guid => AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid)));
            var b = a.Where(asset => asset != null && asset is WFCPreset)
                           .ToList();
            
            presetsList.itemsSource = b;
            presetsList.bindItem = (element, i) =>
            {
                var obj = element.Q<ObjectField>("Element");

                var asset = presetsList.itemsSource[i];
                obj.value = asset as WFCPreset;
            };
            presetsList.Rebuild();
        }

        public override void OnFocus()
        {
            base.OnFocus();
            UpdatePresetsList();
            
        }

        private ExteriorBehaviour GetExteriorBehaviour()
        {
            ExteriorBehaviour exterior = assistant.OwnerLayer.Behaviours
                .Find(b => b is ExteriorBehaviour) as ExteriorBehaviour;
            
            return exterior;
        }
    }
}