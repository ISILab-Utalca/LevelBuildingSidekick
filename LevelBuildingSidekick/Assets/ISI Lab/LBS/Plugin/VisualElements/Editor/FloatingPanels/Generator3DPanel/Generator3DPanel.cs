using ISILab.LBS.Editor.Windows;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using UnityEditor;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Generators;
using LBS.Bundles;
using Object = UnityEngine.Object;

namespace ISILab.LBS.VisualElements.Editor
{
    public class Generator3DPanel : VisualElement
    {
        #region UXMLFACTORY
        [UxmlElementAttribute]
        public new class UxmlFactory { }
        #endregion

        #region VIEW ELEMENTS
        private ClassDropDown dropDownField;
        
        private Vector3Field positionField;
        private Vector2Field scaleField;
        private Vector2Field resizeField;
        
        private TextField nameField;
        
        private Button generateCurrLayer;
        private Button generateAllLayers;
        
        private Toggle genCeilling;
        private Toggle buildLightProbes;
        private Toggle bakeLights;
        private Toggle autoGen;
        private Toggle replacePrev;
        private Toggle ignoreBundleTileSize;
        private Toggle reflection;
        #endregion

        #region FIELDS
        private Generator3D generator;
        private Generator3D.Settings settings;
        private LBSLayer layer;
        #endregion

        #region EVENTS
        public Action OnExecute;
        #endregion

        #region PROPERTIES
        public Generator3D Generator
        {
            get => generator;
            set => generator = value;
        }
        #endregion

        #region CONSTRUCTORS
        public Generator3DPanel()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("Generator3DPanel");
            visualTree.CloneTree(this);

            // room's name
            nameField = this.Q<TextField>(name: "ObjName");
            
            // missing in design prototype
            resizeField = this.Q<Vector2Field>(name: "Resize");
            resizeField.value = Vector2.one;
             
            positionField = this.Q<Vector3Field>(name: "Position");
               
            scaleField = this.Q<Vector2Field>(name: "TileSize");
            scaleField.value = new Vector2(2, 2);
            
            genCeilling = this.Q<Toggle>(name: "ToggleGenCeilling");
            buildLightProbes = this.Q<Toggle>(name: "ToggleLightProbes");
            bakeLights = this.Q<Toggle>(name: "ToggleBakeLights");
            
            // dropDown's visibilty hidden in UMXL
            dropDownField = this.Q<ClassDropDown>(name: "Generator");
            dropDownField.label = "Gennerator";
            dropDownField.Type = typeof(Generator3D);
            
            autoGen = this.Q<Toggle>(name: "ToggleAutoGen");
            replacePrev = this.Q<Toggle>(name: "ToggleReplace");
            ignoreBundleTileSize = this.Q<Toggle>(name: "ToggleTileSize");  
            reflection = this.Q<Toggle>(name: "ToggleReflection");
            
            generateCurrLayer = this.Q<Button>(name: "ButtonGenCurrentLayer");
            generateCurrLayer.clicked += OnExecute;
            generateCurrLayer.clicked += GenerateCurrentLayer;
            
            generateAllLayers = this.Q<Button>(name: "ButtonGenAllLayers");
            generateAllLayers.clicked += OnExecute;
            generateAllLayers.clicked += GenerateAllLayers;
            
            if(generator == null) generator = new Generator3D();
        }
        #endregion

        public void Init(LBSLayer layer)
        {
            if (layer == null)
            {
                LBSMainWindow.MessageNotify("[ISI Lab] Warning: You don't have any layer selected.", LogType.Warning, 2);
                return;
            }

            this.layer = layer;
            generator = new Generator3D();

            generator.settings = settings;
            settings = layer.Settings;


            if (generator != null)
            {
                dropDownField.Value = generator.GetType().Name;
                scaleField.value = settings.scale;
                positionField.value = settings.position;
                nameField.value = layer.Name;
                resizeField.value = settings.resize;
            }
        }

        public void GenerateAllLayers()
        {
            LBSMainWindow mw = EditorWindow.GetWindow<LBSMainWindow>();
            if(mw == null) return;
            var layers = mw.GetLayers();
            
            foreach (var _layer in layers)
            {
                this.layer = _layer;
                GenerateSingleLayer();
            }
            OnFinishGenerate();
        }

        private void OnFinishGenerate()
        {
            if (bakeLights.value)
            {
                // Retrieve the LightingSettings asset by its path
                string BakePath = AssetDatabase.GUIDToAssetPath("e64852b0a0c259543bc34a95930684dd");
                LightingSettings lightingSettings = AssetDatabase.LoadAssetAtPath<LightingSettings>(BakePath);
                if (lightingSettings)
                {
                    Lightmapping.lightingSettings = lightingSettings;
                    Lightmapping.Bake();
                }
                else
                {
                    LBSMainWindow.MessageNotify("Missing lightning settings BakeSetting", LogType.Error, 4);
                }

            }
            
            EditorWindow.FocusWindowIfItsOpen<SceneView>();
        }
        
        private void GenerateCurrentLayer()
        {
            GenerateSingleLayer();
            OnFinishGenerate();
        }

        private void GenerateSingleLayer()
        {
            string ifReplace = "";
            
            if (layer == null)
            {
                LBSMainWindow.MessageNotify("There is no reference for any layer to generate.", LogType.Error, 2);
                return;
            }

            if (replacePrev.value)
            {
                GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                List<GameObject> previousObjects = new List<GameObject>();

                foreach (var generic in allObjects)
                {
                    if (generic.name == layer.Name)  previousObjects.Add(generic);
                }
                foreach (var prev in previousObjects)
                {
                    if(prev == null) continue;
                    ifReplace = "Previous layer replaced.";
                    Object.DestroyImmediate(prev);
                }
            }

            generator ??= dropDownField.GetChoiceInstance() as Generator3D;

            settings = new Generator3D.Settings
            {
                name = layer.Name,
                position = positionField.value,
                resize = resizeField.value,
                scale = scaleField.value,
                useBundleSize = !ignoreBundleTileSize.value,
                reflectionProbe = reflection.value,
                lightVolume = buildLightProbes.value
            };

            if (generator == null) return;
            
            // Generation Call
            var generated = generator.Generate(layer, layer.GeneratorRules, settings);
            
            // If it created a usable LBS game object 
            if (generated.Item1 == null || generated.Item1.gameObject == null ||
                !generated.Item1.GetComponentsInChildren<Transform>().Any() || generated.Item2.Any())
            {
                LBSMainWindow.MessageNotify("Layer " + layer.Name + " could not be created.", LogType.Error);
                foreach (var message in generated.Item2)
                {
                    LBSMainWindow.MessageNotify("   " + message, LogType.Error, 6);
                }
                return;
            }
            
            Undo.RegisterCreatedObjectUndo(generated.Item1, "Create my GameObject");
            
            LBSMainWindow.MessageNotify("Layer " + generated.Item1.gameObject.name + " created. " + ifReplace);
            if (bakeLights.value)
            {
                StaticObjs(generated.Item1);
                BakeReflections();
            }

            if (buildLightProbes.value)
            {
                LightProbeCubeGenerator[] allLightProbes = 
                    Object.FindObjectsByType<LightProbeCubeGenerator>(FindObjectsSortMode.None);
                foreach (var lpcg in allLightProbes) lpcg.Execute();
            }

        }

        private void BakeReflections()
        {
            ReflectionProbe[] probes = Object.FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None);
            foreach (var probe in probes)
            {
                probe.RenderProbe();
            }
        }

        private void StaticObjs(GameObject obj)
        {
            var lbsGen = obj.GetComponent<LBSGenerated>();
            if (lbsGen != null)
            {
                if (lbsGen.BundleRef.PopulationType == Bundle.PopulationTypeE.Character) return;
                if (lbsGen.HasLBSTag("NoBake")) return;
            }
            
            obj.isStatic = true; 
            foreach (Transform child in obj.transform)
            {
                StaticObjs(child.gameObject);
            }
        }
    }
}