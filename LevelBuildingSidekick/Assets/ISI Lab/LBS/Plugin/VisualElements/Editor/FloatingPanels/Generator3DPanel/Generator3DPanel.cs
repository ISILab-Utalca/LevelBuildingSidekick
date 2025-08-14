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
using ISILab.LBS.Settings;

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

  
        public Generator3D GeneratorSettings
        {
            get => LBSSettings.Instance.generator;
            set => LBSSettings.Instance.generator = value;
        }

        #endregion

        #region CONSTRUCTORS
        public Generator3DPanel()
        {
            GeneratorSettings = LBSSettings.Instance.generator;
            Debug.Log(GeneratorSettings.settings);
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

            _replacePrev = this.Q<Toggle>("ToggleReplace");

            if (GeneratorSettings.settings != null)
            {
                
                _replacePrev.value = GeneratorSettings.settings.replacePrevious;
            }
            else
            {   //Set generation setting by default
                _replacePrev.value = true;
                GeneratorSettings = LBSSettings.Instance.generator;
                GeneratorSettings.settings.replacePrevious = true;
            }
            _replacePrev.RegisterValueChangedCallback<bool>(evt => { GeneratorSettings.settings.replacePrevious = evt.newValue; });

            _ignoreBundleTileSize = this.Q<Toggle>(name: "ToggleTileSize");  
            _reflection = this.Q<Toggle>(name: "ToggleReflection");
            
            _generateCurrLayer = this.Q<Button>(name: "ButtonGenCurrentLayer");
            _generateCurrLayer.clicked += OnExecute;
            _generateCurrLayer.clicked += GenerateCurrentLayer;
            
            _generateAllLayers = this.Q<Button>(name: "ButtonGenAllLayers");
            _generateAllLayers.clicked += OnExecute;
            _generateAllLayers.clicked += GenerateAllLayers;

            GeneratorSettings ??= new Generator3D();
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
                //TODO: WHY IS THIS RECOGNIZING THE LAYER ON THE INITIATOR?????????????
                LBSMainWindow.MessageNotify("[ISI Lab] Warning: You don't have any layer selected.", LogType.Warning, 2);
                return;
            }

            _layer = layer;
            GeneratorSettings ??= LBSSettings.Instance.generator;
            _settings = GeneratorSettings.settings;

            if (GeneratorSettings == null) return;
            _dropDownField.Value = GeneratorSettings.GetType().Name;
            _scaleField.value = _settings.scale;
            _positionField.value = _settings.position;
            _nameField.value = layer.Name;
            _resizeField.value = _settings.resize;
        }

        private void GenerateAllLayers()
        {
            LBSMainWindow mw = EditorWindow.GetWindow<LBSMainWindow>();
            if(mw == null) return;
            //var layers = mw.GetLayers();
            var layers = new List<LBSLayer>(mw.GetLayers());
            layers.Sort((l1, l2) =>
            {
                const string testing = "Testing";
                bool l1IsTesting = l1.ID.Equals(testing),
                    l2IsTesting = l2.ID.Equals(testing);

                if (l1IsTesting && !l2IsTesting) return 1;
                if(!l1IsTesting && l2IsTesting) return -1;
                return 0;
            });
            //string log = "";
            //foreach (var l in layers)
            //    log += l.ID + "\n";
            //Debug.Log(log);
            
            foreach (LBSLayer layer in layers)
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
            var valid = GenerateSingleLayer();
            if(valid) OnFinishGenerate();
        }

        private bool GenerateSingleLayer()
        {
            string ifReplace = "";
            
            if (_layer == null)
            {
                LBSMainWindow.MessageNotify("There is no reference for any layer to generate.", LogType.Error, 2);
                return false;
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

            GeneratorSettings ??= _dropDownField.GetChoiceInstance() as Generator3D;

            if (GeneratorSettings != null) _settings = GeneratorSettings.settings;
            // {
            //     name = _layer.Name,
            //     position = _positionField.value,
            //     resize = _resizeField.value,
            //     scale = _scaleField.value,
            //     useBundleSize = !_ignoreBundleTileSize.value,
            //     reflectionProbe = _reflection.value,
            //     lightVolume = _buildLightProbes.value
            // };

            if (GeneratorSettings == null) return false;

            var questGen = GeneratorSettings.GetRule<QuestRuleGenerator>(_layer.GeneratorRules);
            if (questGen is not null) questGen.OnLayerRequired += GenerateLayerByName;
            
            // Generation Call
            var generated = GeneratorSettings.Generate(_layer, _layer.GeneratorRules, _settings);
            
            // If it created a usable LBS game object 
            if (generated.Item1 == null || generated.Item1.gameObject == null ||
                !generated.Item1.GetComponentsInChildren<Transform>().Any() || generated.Item2.Any())
            {
                var errormessage = "Layer " + _layer.Name + " could not be created correctly.";
                LBSMainWindow.MessageNotify(errormessage, LogType.Error);
                Debug.LogError(errormessage);
                
                foreach (var message in generated.Item2)
                {
                    LBSMainWindow.MessageNotify("   " + message, LogType.Error, 6);
                }

                if (generated.Item1 is not null) Object.DestroyImmediate(generated.Item1.gameObject);
                return false;
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

            return true;

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