using ISI_Lab.LBS.Plugin.VisualElements.Game;
using System.Collections.Generic;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
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
        /// <param name="layer"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public override GameObject Generate(LBSLayer layer, Generator3D.Settings settings)
        {
            var pivot = new GameObject(layer.ID);
            var observer = pivot.AddComponent<QuestObserver>();

            CloneRefs.Start();
            var quest = layer.GetModule<QuestGraph>().Clone() as QuestGraph;
            CloneRefs.End();

            var triggers = new List<QuestStep>();

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

            // replace with your own function to incorporate it into your game
            CreateUIDocument(pivot.transform);
            
            return pivot;
        }

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