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

        void OnEnable()
        {
            behaviourOptions = typeof(LBSBehaviour).GetDerivedTypes().ToList();
            assistantOptions = typeof(LBSAssistant).GetDerivedTypes().ToList();
            ruleOptions = typeof(LBSGeneratorRule).GetDerivedTypes().ToList();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var template = (LayerTemplate)target;

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            behaviourIndex = EditorGUILayout.Popup("Type:", behaviourIndex, behaviourOptions.Select(e => e.Name).ToArray());
            var selected = behaviourOptions[behaviourIndex];
            if (GUILayout.Button("Add behaviour"))
            {
                var bh = Activator.CreateInstance(selected, null, "Default Name");
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

            // Behaviours
            var bhIcon = Resources.Load<Texture2D>("Icons/Select");
            var bh = new SchemaBehaviour(bhIcon, "Schema behaviour");
            layer.AddBehaviour(bh);

            // Assistants
            var assIcon = Resources.Load<Texture2D>("Icons/Select");
            var ass = new HillClimbingAssistant(assIcon, "HillClimbing");
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
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
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
            var bhIcon = Resources.Load<Texture2D>("Icons/Select"); // TODO: Change icon
            var bh = new ExteriorBehaviour(bhIcon, "Exterior behaviour");
            bh.OnAttachLayer(layer);
            layer.AddBehaviour(bh);

            // Assistant
            var assIcon = Resources.Load<Texture2D>("Icons/Select"); // TODO: Change icon
            var ass = new AssistantWFC(assIcon, "Assistant WFC");
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
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
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

            /*
            layer.ID = "Population";
            layer.Name = "Layer Population";
            layer.iconPath = "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/ghost.png";
            template.layer = layer;
            */
            
            // Behaviours
            var Icon = Resources.Load<Texture2D>("Icons/Select");
            layer.AddBehaviour(new PopulationBehaviour(Icon, "Population Behavior"));

            // Assistants
            var assIcon = Resources.Load<Texture2D>("Icons/Select");
            var ass = new AssistantMapElite(assIcon, "Map Elite - Genetic Algorithm");
            ass.OnAttachLayer(layer);
            //ass.OnAdd(layer);
            layer.AddAssistant(ass);

            // Rules
            layer.AddGeneratorRule(new PopulationRuleGenerator());

            Debug.Log("Set Population Default");
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
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
            
            // Assistants
            var assIcon = Resources.Load<Texture2D>("Icons/Select");
            var ass = new GrammarAssistant(assIcon, "Grammar Assistant");
            ass.OnAttachLayer(layer);
            layer.AddAssistant(ass);
            
            // Behaviours
            var bhIcon = Resources.Load<Texture2D>("Icons/Select");
            var bh = new QuestBehaviour(bhIcon, "Quest Behavior");
            bh.OnAttachLayer(layer);
            layer.AddBehaviour(bh);
            
            // Rules
            layer.AddGeneratorRule(new QuestRuleGenerator());

            Debug.Log("Set Quest Default");
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
    }
}