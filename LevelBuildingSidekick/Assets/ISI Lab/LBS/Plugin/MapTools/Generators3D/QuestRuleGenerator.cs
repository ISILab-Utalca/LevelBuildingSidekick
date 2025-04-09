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
        public QuestRuleGenerator()
        { }

        public override List<Message> CheckViability(LBSLayer layer)
        {
            throw new System.NotImplementedException();
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

            var triggers = new List<QuestStep>();

            var assistant = layer.GetAssistant<GrammarAssistant>();
            /*foreach (var edge in quest.QuestEdges)
            {
                assistant?.ValidateEdgeGrammarOLD(edge);
            }
            bool allValid = quest.QuestNodes.All(q => q.GrammarCheck);
         
            assistant?.ValidateEdgeGrammar(quest.QuestEdges.First());
            bool allValid = assistant!.fastValidGrammar(quest.QuestNodes);
            if (!allValid)
            {
                return Tuple.Create<GameObject, string>(null, "At least one quest node is not grammatically valid. Fix or remove");
            }
               */
            foreach (var node in quest.QuestNodes)
            {
                var go = new GameObject(node.ID);

                go.transform.position = node.Target.Rect.position;
                go.transform.parent = observer.transform;

                var trigger = go.AddComponent<QuestTrigger>();
                trigger.Init(new Vector3(node.Target.Rect.width, 1, node.Target.Rect.height));

                go.SetActive(false);

                triggers.Add(new QuestStep(node, trigger));
            }

            observer.Init(quest, triggers);

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
            uiGameObject.AddComponent<QuestVisualTree>();
            var uiAsset = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestVisualTree");
            var panelSettings = LBSAssetMacro.LoadAssetByGuid<PanelSettings>("da6adae693698d3409943a20661e2031");
            
            if (uiAsset == null || panelSettings == null) return; 
            
            uiDocument.visualTreeAsset = uiAsset;
            uiDocument.panelSettings = panelSettings;
            uiGameObject.transform.SetParent(pivotTransform);
        }
    }
}