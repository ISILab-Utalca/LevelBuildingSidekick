using ISILab.LBS.Editor.Windows;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Components;

using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ClassDropDown _dropDownField;
        
        private readonly Vector3Field _positionField;
        private readonly Vector2Field _scaleField;
        private readonly Vector2Field _resizeField;
        
        private readonly TextField _nameField;
        
        private readonly Button _generateCurrLayer;
        private readonly Button _generateAllLayers;

        private readonly Toggle _buildLightProbes;
        private readonly Toggle _bakeLights;
        private readonly Toggle _replacePrev;
        private readonly Toggle _ignoreBundleTileSize;
        private readonly Toggle _reflection;
        #endregion

        #region FIELDS
        private Generator3D.Settings _settings;
        private LBSLayer _layer;
        #endregion

        #region EVENTS
        public Action OnExecute;
        #endregion

        #region PROPERTIES

        private Generator3D Generator { get; set; }

        #endregion

        #region CONSTRUCTORS
        public Generator3DPanel()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("Generator3DPanel");
            visualTree.CloneTree(this);

            // room's name
            _nameField = this.Q<TextField>(name: "ObjName");
            
            // missing in design prototype
            _resizeField = this.Q<Vector2Field>(name: "Resize");
            _resizeField.value = Vector2.one;
             
            _positionField = this.Q<Vector3Field>(name: "Position");
               
            _scaleField = this.Q<Vector2Field>(name: "TileSize");
            _scaleField.value = new Vector2(2, 2);
            
            this.Q<Toggle>(name: "ToggleGenCeilling");
            _buildLightProbes = this.Q<Toggle>(name: "ToggleLightProbes");
            _bakeLights = this.Q<Toggle>(name: "ToggleBakeLights");
            
            // dropDown's visibility hidden in UMXL
            _dropDownField = this.Q<ClassDropDown>(name: "Generator");
            _dropDownField.label = "Gennerator";
            _dropDownField.Type = typeof(Generator3D);
            
            this.Q<Toggle>(name: "ToggleAutoGen");
            _replacePrev = this.Q<Toggle>(name: "ToggleReplace");
            _ignoreBundleTileSize = this.Q<Toggle>(name: "ToggleTileSize");  
            _reflection = this.Q<Toggle>(name: "ToggleReflection");
            
            _generateCurrLayer = this.Q<Button>(name: "ButtonGenCurrentLayer");
            _generateCurrLayer.clicked += OnExecute;
            _generateCurrLayer.clicked += GenerateCurrentLayer;
            
            _generateAllLayers = this.Q<Button>(name: "ButtonGenAllLayers");
            _generateAllLayers.clicked += OnExecute;
            _generateAllLayers.clicked += GenerateAllLayers;
            
            Generator ??= new Generator3D();
        }

        public Generator3DPanel(Toggle bakeLights)
        {
            _bakeLights = bakeLights;
        }

        #endregion

        public void Init(LBSLayer layer)
        {
            if (layer == null)
            {
                LBSMainWindow.MessageNotify("[ISI Lab] Warning: You don't have any layer selected.", LogType.Warning, 2);
                return;
            }

            _layer = layer;
            Generator = new Generator3D();

            Generator.settings = _settings;
            _settings = layer.Settings;


            if (Generator != null)
            {
                _dropDownField.Value = Generator.GetType().Name;
                _scaleField.value = _settings.scale;
                _positionField.value = _settings.position;
                _nameField.value = layer.Name;
                _resizeField.value = _settings.resize;
            }
        }

        public void GenerateAllLayers()
        {
            LBSMainWindow mw = EditorWindow.GetWindow<LBSMainWindow>();
            if(mw == null) return;
            var layers = mw.GetLayers();
            
            foreach (var layer in layers)
            {
                _layer = layer;
                GenerateSingleLayer();
            }
            OnFinishGenerate();
        }
        
        
        private void GenerateLayerByName(string layerName)
        {
            LBSMainWindow mw = EditorWindow.GetWindow<LBSMainWindow>();
            if(mw == null) return;
            var layers = mw.GetLayers();

            LBSLayer tempLayer = null;
            if (_layer is not null) tempLayer = _layer;
            
            var foundLayer = layers.Find(l => l.Name == layerName);
            if (foundLayer != null)
            {
                _layer = foundLayer;
                GenerateSingleLayer();
            }

            if (tempLayer is not null) _layer = tempLayer;
            
            OnFinishGenerate();
        }

        private void OnFinishGenerate()
        {
            if (_bakeLights.value)
            {
                // Retrieve the LightingSettings asset by its path
                string bakePath = AssetDatabase.GUIDToAssetPath("e64852b0a0c259543bc34a95930684dd");
                LightingSettings lightingSettings = AssetDatabase.LoadAssetAtPath<LightingSettings>(bakePath);
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
            
            if (_layer == null)
            {
                LBSMainWindow.MessageNotify("There is no reference for any layer to generate.", LogType.Error, 2);
                return;
            }

            if (_replacePrev.value)
            {
                GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                List<GameObject> previousObjects = new List<GameObject>();

                foreach (var generic in allObjects)
                {
                    if (generic.name == _layer.Name)  previousObjects.Add(generic);
                }
                foreach (var prev in previousObjects)
                {
                    if(prev == null) continue;
                    ifReplace = "Previous layer replaced.";
                    Object.DestroyImmediate(prev);
                }
            }

            Generator ??= _dropDownField.GetChoiceInstance() as Generator3D;

            _settings = new Generator3D.Settings
            {
                name = _layer.Name,
                position = _positionField.value,
                resize = _resizeField.value,
                scale = _scaleField.value,
                useBundleSize = !_ignoreBundleTileSize.value,
                reflectionProbe = _reflection.value,
                lightVolume = _buildLightProbes.value
            };

            if (Generator == null) return;

            var questGen = Generator.GetRule<QuestRuleGenerator>(_layer.GeneratorRules);
            if (questGen is not null) questGen.OnLayerRequired += GenerateLayerByName;
            
            // Generation Call
            var generated = Generator.Generate(_layer, _layer.GeneratorRules, _settings);
            
            // If it created a usable LBS game object 
            if (generated.Item1 == null || generated.Item1.gameObject == null ||
                !generated.Item1.GetComponentsInChildren<Transform>().Any() || generated.Item2.Any())
            {
                LBSMainWindow.MessageNotify("Layer " + _layer.Name + " could not be created.", LogType.Error);
                foreach (var message in generated.Item2)
                {
                    LBSMainWindow.MessageNotify("   " + message, LogType.Error, 6);
                }
                return;
            }
            
            Undo.RegisterCreatedObjectUndo(generated.Item1, "Create my GameObject");
            
            LBSMainWindow.MessageNotify("Layer " + generated.Item1.gameObject.name + " created. " + ifReplace);
            if (_bakeLights.value)
            {
                StaticObjs(generated.Item1);
                BakeReflections();
            }

            if (_buildLightProbes.value)
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