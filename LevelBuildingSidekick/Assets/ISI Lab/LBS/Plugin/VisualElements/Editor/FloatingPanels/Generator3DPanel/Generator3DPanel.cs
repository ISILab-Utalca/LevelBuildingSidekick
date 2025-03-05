using ISILab.LBS.Editor.Windows;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Components;

using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using UnityEditor;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Generators;

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
            this.generator = new Generator3D();

            generator.settings = settings;
            this.settings = layer.Settings;


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
        }
        
        private void GenerateCurrentLayer()
        {
            GenerateSingleLayer();
            OnFinishGenerate();
        }
        
        public void GenerateSingleLayer()
        {
            string ifReplace = "";
            
            if (this.layer == null)
            {
                LBSMainWindow.MessageNotify("There is no reference for any layer to generate.", LogType.Error, 2);
                return;
            }

            if (replacePrev.value)
            {
                GameObject[] allObjects = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                List<GameObject> prevs = new List<GameObject>();

                foreach (var generic in allObjects)
                {
                    if (generic.name == layer.Name)  prevs.Add(generic);
                }
                foreach (var prev in prevs)
                {
                    ifReplace = "Previous layer replaced.";
                    GameObject.DestroyImmediate(prev);
                }
            }

            if (generator == null)
            {
                generator = dropDownField.GetChoiceInstance() as Generator3D;
            }

            var settings = new Generator3D.Settings
            {
                name = layer.Name,
                //name = nameField.value != "" ? name = nameField.value : layer.Name,
                position = positionField.value,
                resize = resizeField.value,
                scale = scaleField.value,
                useBundleSize = !ignoreBundleTileSize.value,
                reflectionProbe = reflection.value,
                lightVolume = buildLightProbes.value
            };

            var obj = generator.Generate(this.layer, this.layer.GeneratorRules, settings);
            
            // If it created a usable LBS game object 
            if (obj == null || obj.gameObject == null || obj.GetComponentsInChildren<Transform>().Length == 0)
            {
                LBSMainWindow.MessageNotify("Layer " + layer.Name + " could not be created.", LogType.Error, 3);
                return;
            }

            
            Undo.RegisterCreatedObjectUndo(obj, "Create my GameObject");
            
            LBSMainWindow.MessageNotify("Layer " + obj.gameObject.name + " created. " + ifReplace, LogType.Log, 3);
            EditorWindow.FocusWindowIfItsOpen<SceneView>();
            
            if (bakeLights.value)
            {
                StaticObjs(obj);
                BakeReflections();
            }

            if (buildLightProbes.value)
            {
                LightProbeCubeGenerator[] allLightProbes = 
                    UnityEngine.Object.FindObjectsByType<LightProbeCubeGenerator>(FindObjectsSortMode.None);
                foreach (var lpcg in allLightProbes) lpcg.Execute();
            }

        }

        private void BakeReflections()
        {
            ReflectionProbe[] probes = UnityEngine.Object.FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None);
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