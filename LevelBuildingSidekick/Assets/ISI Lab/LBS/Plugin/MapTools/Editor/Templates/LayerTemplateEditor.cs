using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ISILab.Extensions;
using ISILab.LBS.AI.Assistants;
using ISILab.LBS.Generators;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Assistants;
using ISILab.LBS.Editor;
using ISILab.Macros;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace ISILab.LBS.Template.Editor
{
    [LBSCustomEditor("Layer template", typeof(LayerTemplate))]
    [CustomEditor(typeof(LayerTemplate))]
    public class LayerTemplateEditor : UnityEditor.Editor
    {
        private int behaviourIndex = 0;
        private List<Type> behaviourOptions;

        private int assitantIndex = 0;
        private List<Type> assistantOptions;

        private int ruleIndex = 0;
        private List<Type> ruleOptions;

        VectorImage behaviourIcon;
        VectorImage assistantIcon;
        
        const string defaultBehaviorIcon = "e17eb0e02534666439fca8ea30b4d4e4";
        const string defaultAssistantIcon = "ad8feef201665454ca79e31b7d798ac3";
        
        void OnEnable()
        {
            behaviourOptions = typeof(LBSBehaviour).GetDerivedTypes().ToList();
            assistantOptions = typeof(LBSAssistant).GetDerivedTypes().ToList();
            ruleOptions = typeof(LBSGeneratorRule).GetDerivedTypes().ToList();
        }

        public override void OnInspectorGUI()
        {
            behaviourIcon = LBSAssetMacro.LoadAssetByGuid<VectorImage>(defaultBehaviorIcon); 
            assistantIcon = LBSAssetMacro.LoadAssetByGuid<VectorImage>(defaultAssistantIcon); 
            
            base.OnInspectorGUI();

            var template = (LayerTemplate)target;

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            behaviourIndex = EditorGUILayout.Popup("Type:", behaviourIndex, behaviourOptions.Select(e => e.Name).ToArray());
            var selected = behaviourOptions[behaviourIndex];
            if (GUILayout.Button("Add behaviour"))
            {
                var bh = Activator.CreateInstance(selected, null, "Default Name", Color.clear);
                template.layer.AddBehaviour(bh as LBSBehaviour);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            assitantIndex = EditorGUILayout.Popup("Type:", assitantIndex, assistantOptions.Select(e => e.Name).ToArray());
            var selected2 = assistantOptions[assitantIndex];
            if (GUILayout.Button("Add Assistent"))
            {
                var ass = Activator.CreateInstance(selected2);
                template.layer.AddAssistant(ass as LBSAssistant);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            ruleIndex = EditorGUILayout.Popup("Type:", ruleIndex, ruleOptions.Select(e => e.Name).ToArray());
            var selected3 = ruleOptions[ruleIndex];
            if (GUILayout.Button("Add Generator"))
            {
                var rule = Activator.CreateInstance(selected3);
                template.layer.AddGeneratorRule(rule as LBSGeneratorRule);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(20);

            if (GUILayout.Button("Set as Interior"))
            {
                InteriorConstruct(template);
            }

            if (GUILayout.Button("Set as Exterior"))
            {
                ExteriorConstruct(template);
            }

            if (GUILayout.Button("Set as Population"))
            {
                PopulationConstruct(template);
            }
            
            if (GUILayout.Button("Set as Quest"))
            {
                QuestConstruct(template);
            }

            if (GUILayout.Button("Apply Changes"))
            {
                ApplyChanges();
            }
            
            if (GUILayout.Button("Set as Testing"))
            {
                //TODO: implement this!
                //TestingConstruct(template);
            }
        }

        private void ApplyChanges()
        {
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();

        }
        
        /// <summary>
        /// This function adjust the icons, layout and labels of the system for Contructi�n in interior
        /// also call the manipulators to make functional buttons in the layout
        /// </summary>
        /// <param name="template"></param>
        private void InteriorConstruct(LayerTemplate template)
        {
            template.Clear();

            // Basic data layer
            var layer = template.layer;
            layer.ID = "Interior";
            layer.Name = "Layer Interior";
            layer.iconPath = "Assets/ISI Lab/Commons/Assets2D/Resources/Icons/Vectorial/Icon=InteriorLayerIcon.png";
            template.layer = layer;

            // Behavior
            var bh = new SchemaBehaviour(behaviourIcon, "Schema behaviour", Settings.LBSSettings.Instance.view.behavioursColor);
            layer.AddBehaviour(bh);

            // Assistants
            var ass = new HillClimbingAssistant(assistantIcon, "HillClimbing", Settings.LBSSettings.Instance.view.assistantColor);
            layer.AddAssistant(ass);

            // Generators
            layer.AddGeneratorRule(new SchemaRuleGenerator());
            layer.AddGeneratorRule(new SchemaRuleGeneratorExterior());

            // Seting generator
            layer.Settings = new Generator3D.Settings()
            {
                scale = new Vector2Int(2, 2),
                resize = new Vector2(0, 0),
                position = new Vector3(0, 0, 0),
                name = "Interior",
            };

            // Save scriptableObject
            Debug.Log("Set Interior Default");
            ApplyChanges();
        }

        /// <summary>
        /// This function adjust the icons, layout and labels of the system for Contructi�n in exterior
        /// also call the manipulators to make functional buttons in the layout
        /// </summary>
        /// <param name="template"></param>
        private void ExteriorConstruct(LayerTemplate template)
        {
            template.Clear();

            // Basic data layer
            var layer = template.layer;
            layer.ID = "Exterior";
            layer.Name = "Layer Exterior";
            layer.iconPath = "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/pine-tree.png";
            template.layer = layer;
            
            // Behaviours
            var bh = new ExteriorBehaviour(behaviourIcon, "Exterior behaviour", Settings.LBSSettings.Instance.view.behavioursColor);
            bh.OnAttachLayer(layer);
            layer.AddBehaviour(bh);

            // Assistant
            var ass = new AssistantWFC(assistantIcon, "Assistant WFC", Settings.LBSSettings.Instance.view.assistantColor);
            ass.OnAttachLayer(layer);
            layer.AddAssistant(ass);

            // Generators
            layer.AddGeneratorRule(new ExteriorRuleGenerator());

            // Settings generator
            layer.Settings = new Generator3D.Settings()
            {
                scale = new Vector2Int(2, 2),
                resize = new Vector2(0, 0),
                position = new Vector3(0, 0, 0),
                name = "Exterior",
            };

            // Save scriptableObject
            Debug.Log("Set Exterior Default");
            ApplyChanges();
        }

        /// <summary>
        /// This function adjust the icons, layout and labels of the Population system
        /// also call the manipulators to make functional buttons in the layout
        /// </summary>
        /// <param name="template"></param>
        private void PopulationConstruct(LayerTemplate template)
        {
            template.Clear();

            // Basic data layer
            var layer = template.layer;
            layer.ID = "Population";
            layer.Name = "Layer Population";
            layer.iconPath = "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/ghost.png";
            template.layer = layer;

            layer.Settings = new Generator3D.Settings()
            {
                scale = new Vector2Int(2, 2),
                resize = new Vector2(0, 0),
                position = new Vector3(0, 0, 0),
                name = "Population",
            };
            
            // Behaviours
            layer.AddBehaviour(new PopulationBehaviour(behaviourIcon, "Population Behavior", Settings.LBSSettings.Instance.view.behavioursColor));

            // Assistants
            var ass = new AssistantMapElite(assistantIcon, "Map Elite - Genetic Algorithm", Settings.LBSSettings.Instance.view.assistantColor);
            ass.OnAttachLayer(layer);
            layer.AddAssistant(ass);

            // Rules
            layer.AddGeneratorRule(new PopulationRuleGenerator());

            Debug.Log("Set Population Default");
            ApplyChanges();
        }

        
        /// <summary>
        /// This function adjust the icons, layout and labels of the Quest system
        /// also call the manipulators to make functional buttons in the layout
        /// </summary>
        /// <param name="template"></param>
        private void QuestConstruct(LayerTemplate template)
        {
            template.Clear();

            // Basic data layer
            var layer = template.layer;
            layer.ID = "Quest";
            layer.Name = "Layer Quest";
            layer.iconPath = "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/Stamp_Icon.png";
            template.layer = layer;

            layer.Settings = new Generator3D.Settings()
            {
                scale = new Vector2Int(2, 2),
                resize = new Vector2(0, 0),
                position = new Vector3(0, 0, 0),
                name = "Quest",
            };
            
            // Behaviours TODO add history behavior
            var bh = new QuestBehaviour(behaviourIcon, "Quest Behavior", Settings.LBSSettings.Instance.view.behavioursColor);
            bh.OnAttachLayer(layer);
            layer.AddBehaviour(bh);
            
            // Assistants
   
            var ass = new GrammarAssistant(assistantIcon, "Grammar Assistant", Settings.LBSSettings.Instance.view.assistantColor);
            ass.OnAttachLayer(layer);
            layer.AddAssistant(ass);
            
            // Rules
            layer.AddGeneratorRule(new QuestRuleGenerator());

            Debug.Log("Set Quest Default");
            ApplyChanges();
        }
        /// <summary>
        /// This function adjust the icons, layout and labels of the PathOS Testing system.
        /// Also calls the manipulators to make functional buttons in the layout (TO BE IMPLEMENTED).
        /// </summary>
        /// <param name="template"></param>
        //private void TestingConstruct(LayerTemplate template)
        //{
        //    template.Clear();
        //
        //    // Basic data layer
        //    var layer = template.layer;
        //    layer.ID = "Testing";
        //    layer.Name = "Layer Testing";
        //    layer.iconPath = "Assets/ISI Lab/LBS/GABO/Resources/Icons/TinyIconPathOSModule16x16.png";
        //    template.layer = layer;
        //
        //    // Generator
        //    layer.Settings = new Generator3D.Settings()
        //    {
        //        scale = new Vector2Int(2, 2),
        //        resize = new Vector2(0, 0),
        //        position = new Vector3(0, 0, 0),
        //        name = "Testing",
        //    };
        //
        //    // Behaviours
        //    var Icon = Resources.Load<Texture2D>("Icons/Select");
        //    layer.AddBehaviour(new PathOSBehaviour(Icon, "PathOS Behaviour"));
        //
        //    // Rules
        //    layer.AddGeneratorRule(new PathOSRuleGenerator());
        //
        //    Debug.Log("Set Testing Default");
        //    EditorUtility.SetDirty(target);
        //    AssetDatabase.SaveAssets();
        //}
    }
}