using System;
using System.Collections.Generic;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Assistants;
using ISILab.LBS.Modules;
using ISILab.Macros;
using LBS.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Generators
{

    public class QuestRuleGenerator : LBSGeneratorRule
    {
        public override List<Message> CheckViability(LBSLayer layer)
        {
            throw new NotImplementedException();
        }

        public override object Clone()
        {
            return new QuestRuleGenerator();
        }

        /// <summary>
        /// Generates the quest observer (to set up the quest triggers in the scene)
        /// it also generates a UI Document that will display the default display for quest
        /// 
        /// </summary>
        /// <param name="layer"> the quest layer that contains the quest nodes and edges</param>
        /// <param name="settings"> the settings of the generator</param>
        /// <returns></returns>
        public override Tuple<GameObject, string> Generate(LBSLayer layer, Generator3D.Settings settings)
        {
            var pivot = new GameObject(layer.ID);
            var observer = pivot.AddComponent<QuestObserver>();

            CloneRefs.Start();
            var quest = layer.GetModule<QuestGraph>().Clone() as QuestGraph;
            if(quest == null)
            {
                return Tuple.Create<GameObject, string>(null, "No quest graph found. Can't generate");
            }
            CloneRefs.End();
            

            if (!quest.QuestEdges.Any())
            {
                return Tuple.Create<GameObject, string>(null, "The quest graph only has one node, can't generate. Can't generate");
            }
            
            var assistant = layer.GetAssistant<GrammarAssistant>();
            assistant?.ValidateEdgeGrammar(quest.QuestEdges.First());
           // bool allValid = quest.QuestNodes.All(q => q.GrammarCheck);
            bool allValid = true;
            if (!allValid)
            {
                return Tuple.Create<GameObject, string>(null, "At least one quest node is not grammatically valid. Fix or remove");
            }
            /*foreach (var edge in quest.QuestEdges)
            {
                assistant?.ValidateEdgeGrammarOLD(edge);
            }
            bool allValid = quest.QuestNodes.All(q => q.GrammarCheck);
         
           
            bool allValid = assistant!.fastValidGrammar(quest.QuestNodes);
            if (!allValid)
            {
                return Tuple.Create<GameObject, string>(null, "At least one quest node is not grammatically valid. Fix or remove");
            }
               */
            foreach (var node in quest.QuestNodes)
            {
                var go = new GameObject(node.ID)
                {
                    transform =
                    {
                        parent = observer.transform
                    }
                };

                string tag = node.QuestAction.Trim().ToLowerInvariant();

                // Get the proper trigger for the given quest node action
                Type triggerType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => 
                        typeof(QuestTrigger).IsAssignableFrom(t) &&
                        !t.IsAbstract &&
                        t.GetCustomAttributes(typeof(QuestNodeActionTag), false)
                            .Cast<QuestNodeActionTag>()
                            .Any(attr => attr.Tag == tag));

                if (triggerType == null)
                {
                    Debug.LogError($"No QuestTrigger type found for tag '{tag}'");
                    continue;
                }
                
                var trigger = (QuestTrigger)go.AddComponent(triggerType);
                var size = node.NodeData._size;
                trigger.SetSize(new Vector3(
                    size*settings.scale.x, 
                    size*settings.scale.y, 
                    size*settings.scale.y));
                
                trigger.SetData(node); 
                go.SetActive(false);
              
                var x = node.NodeData._position.x * settings.scale.x;
                var z = node.NodeData._position.y * settings.scale.y;
                var y = pivot.transform.position.y; // maybe change this to a line trace
                var questPos= new Vector3(x, y, z);
                
                var basePos = settings.position;
                var delta = new Vector3(settings.scale.x, 0, settings.scale.y) / 2f;
                
                go.transform.position =   basePos + questPos - delta;
                
            }


            observer.Init(quest);

            /* For LBS User:
             * ----------------------------------------------------------------
             * Replace with your own function to incorporate the created quests
             * into your game. Check the "QuestVisualTree" class as an example.
             * ----------------------------------------------------------------
             */
            CreateUIDocument(pivot.transform);
            
            return Tuple.Create<GameObject, string>(pivot, null);
        }

        /// <summary>
        /// Creates the ui document class (that's displayed during game mode) and
        /// adds it into the layer generated game object
        /// </summary>
        /// <param name="pivotTransform"> transform to assign the UI as child</param>
        private void CreateUIDocument(Transform pivotTransform)
        {
            GameObject uiGameObject = new GameObject("UIDocument");
            UIDocument uiDocument = uiGameObject.AddComponent<UIDocument>();
           
            if (!uiGameObject) return;
            uiGameObject.AddComponent<QuestVisualTree>();
            var uiAsset = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestVisualTree");
            var panelSettings = LBSAssetMacro.LoadAssetByGuid<PanelSettings>("da6adae693698d3409943a20661e2031");

            if (!uiAsset || !panelSettings) return;

            uiDocument.visualTreeAsset = uiAsset;
            uiDocument.panelSettings = panelSettings;
            uiGameObject.transform.SetParent(pivotTransform);
        }
    }
}