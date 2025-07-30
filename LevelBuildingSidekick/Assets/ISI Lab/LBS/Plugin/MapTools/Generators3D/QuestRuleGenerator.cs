using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Assistants;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using ISILab.Macros;
using LBS.Bundles;
using LBS.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ISILab.LBS.Generators
{

    public class QuestRuleGenerator : LBSGeneratorRule
    {
        private const float frameDelay = 5f;
        private float _currentFrameDelay = frameDelay;
        private const float ProbeRadius = 10f;
       // private static readonly Collider[] OverlapResults = new Collider[32];
        
        private Action<string> _onLayerRequired;
        public event Action<string> OnLayerRequired
        {
            add => _onLayerRequired = value;
            remove => _onLayerRequired -= value;
        }

        
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
            bool allValid = assistant.ValidateQuestGraph();
             if (!allValid)
             {
                 return Tuple.Create<GameObject, string>(null, "At least one quest node is not grammatically valid. Fix or remove");
             }
          
            
            GenerateTriggers(settings, quest, observer, pivot);


            observer.Init(quest);

            /* For LBS User:
             * ----------------------------------------------------------------
             * Replace with your own function to incorporate the created quests
             * into your game. Check the "QuestVisualTree" class as an example.
             * ----------------------------------------------------------------
             */
            CreateUIDocument(pivot.transform, observer.gameObject);
            
            return Tuple.Create<GameObject, string>(pivot, null);
        }

        private void GenerateTriggers(Generator3D.Settings settings, QuestGraph quest, QuestObserver observer, GameObject pivot)
        {
            foreach (var node in quest.QuestNodes)
            {
                // Find if it has a reference to another layer
                GenerateRequiredLayers(node);
            }
            
            // Delay execution so the engine enables the colliders
            EditorApplication.update += DelayGeneration;
            return;

            void DelayGeneration()
            {
                if (_currentFrameDelay-- > 0) return;
                _currentFrameDelay = frameDelay;
                EditorApplication.update -= DelayGeneration;
                GenerateTriggersPerNode(settings, quest, observer, pivot);
            }
        }

        private static void GenerateTriggersPerNode(Generator3D.Settings settings, QuestGraph quest, QuestObserver observer,
            GameObject pivot)
        {
            foreach (var node in quest.QuestNodes)
            {
                Type triggerType = QuestTagRegistry.GetTriggerTypeForTag(node.QuestAction);

                if (triggerType == null)
                {
                    Debug.LogError($"No trigger type found for tag '{node.QuestAction}' in QuestTagRegistry");
                    continue;
                }
                
                // Create GameObject for the trigger
                var go = new GameObject(node.ID)
                {
                    transform = { parent = observer.transform }
                };

                // Add the trigger component dynamically
                var trigger = (QuestTrigger)go.AddComponent(triggerType);
                
                // Set up visual size
                var size = node.NodeData.Area;
                trigger.SetSize(new Vector3(
                    size.x * settings.scale.x,
                    size.height * settings.scale.y,
                    size.y * settings.scale.y));

                // Position the trigger in the world
                var x = node.NodeData.Area.x * settings.scale.x;
                var z = node.NodeData.Area.y * settings.scale.y;
                var y = pivot.transform.position.y;

                var questPos = new Vector3(x, y, z);
                var basePos = settings.position;
                var delta = new Vector3(settings.scale.x, 0, settings.scale.y) / 2f;

                go.transform.position = basePos + questPos - delta;

                // Set shared data
                trigger.SetData(node);
                
                // Find and assign population objects for specific node types
                FindPopulationObjects(trigger, settings, node, basePos, y, delta);

                if(node.NodeData.IsValid())
                {
                    trigger.SetDataNode(node.NodeData);
                }
                else
                {
                    Debug.LogError($"Node Data '{node.ID}' doesn't have a valid data");
                    Object.DestroyImmediate(pivot);
                    return;
                }
                
                go.SetActive(false);
            }
        }

        private void GenerateRequiredLayers(QuestNode node)
        {
            List<string> referencedLayers = node.NodeData.ReferencedLayerNames();
            if (referencedLayers is null || !referencedLayers.Any()) return;
            
            // Find all GameObjects in the scene
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            List<GameObject> matchingObjects = allObjects.Where(gameObject => referencedLayers.Contains(gameObject.name)).ToList();
                    
            foreach (GameObject gameObject in matchingObjects)
            {
                referencedLayers.Remove(gameObject.name);
            }

            // the list keeps the non existing objects
            foreach (var pendingLayerID in referencedLayers.Distinct())
            {
                if (EditorUtility.DisplayDialog(
                        "Missing Layer Dependency",
                        $"The layer \"{pendingLayerID}\" does not exist and its data is being used by " +
                        $"the quest layer.\nWould you like to generate it now?",
                        "Yes (Generate Layer)",
                        "No (Set values manually in scene)"
                    ))
                {
                    _onLayerRequired?.Invoke(pendingLayerID);
                }
              
            }

        }


        /// <summary>
        /// Tries to find objects in the scene, assuming they were generated previously
        /// </summary>
        /// <param name="trigger">Trigger type to be instance into the Quest Observer GameObject</param>
        /// <param name="settings">Settings to get the positions the population objects should have on the scene</param>
        /// <param name="node">Node to recognize failure to find object</param>
        /// <param name="basePos">the base position corresponds to the grid location</param>
        /// <param name="y">Pivot.y</param>
        /// <param name="delta">Rescale from graph size </param>
        private static void FindPopulationObjects(QuestTrigger trigger, Generator3D.Settings settings, QuestNode node, Vector3 basePos, float y, Vector3 delta)
        {
            switch (node.NodeData)
            {
                case DataTake dataTake when trigger is QuestTriggerTake takeTrigger:
                    if (dataTake.bundleToTake.Valid())
                    {
                        AssignObjectByBundleGraph(
                            node,
                            dataTake.bundleToTake,
                            settings,
                            basePos,
                            y,
                            delta,
                            foundObject => takeTrigger.objectToTake = foundObject
                        );
                    }
                    break;
                
                case DataStealth dataStealth when trigger is QuestTriggerStealth stealthTrigger:
                    var scenePosition = 
                        GetScenePosition(new Rect(dataStealth.objective.x,dataStealth.objective.y,1,1), 
                            settings, basePos, y, delta);  
                    stealthTrigger.objectivePosition = scenePosition;
                    break;

                case DataRead dataRead when trigger is QuestTriggerRead readTrigger:
                    if (dataRead.bundleToRead.Valid())
                    {
                        AssignObjectByBundleGraph(
                            node,
                            dataRead.bundleToRead,
                            settings,
                            basePos,
                            y,
                            delta,
                            foundObject => readTrigger.objectToRead = foundObject
                        );
                    }
                    break;

                case DataGive dataGive when trigger is QuestTriggerGive giveTrigger:
                    if (dataGive.bundleGiveTo.Valid())
                    {
                        AssignObjectByBundleGraph(
                            node,
                            dataGive.bundleGiveTo,
                            settings,
                            basePos,
                            y,
                            delta,
                            foundObject => giveTrigger.objectToGiveTo = foundObject
                        );
                    }
                    break;

                case DataReport dataReport when trigger is QuestTriggerReport reportTrigger:
                    if (dataReport.bundleReportTo.Valid())
                    {
                        AssignObjectByBundleGraph(
                            node,
                            dataReport.bundleReportTo,
                            settings,
                            basePos,
                            y,
                            delta,
                            foundObject => reportTrigger.objectToReport = foundObject
                        );
                    }
                    break;

                case DataSpy dataSpy when trigger is QuestTriggerSpy spyTrigger:
                    if (dataSpy.bundleToSpy.Valid())
                    {
                        AssignObjectByBundleGraph(
                            node,
                            dataSpy.bundleToSpy,
                            settings,
                            basePos,
                            y,
                            delta,
                            foundObject => spyTrigger.objectToSpy = foundObject
                        );
                    }
                    break;

                case DataListen dataListen when trigger is QuestTriggerListen listenTrigger:
                    if (dataListen.bundleListenTo.Valid())
                    {
                        AssignObjectByBundleGraph(
                            node,
                            dataListen.bundleListenTo,
                            settings,
                            basePos,
                            y,
                            delta,
                            foundObject => listenTrigger.objectToListen = foundObject
                        );
                    }
                    break;

                case DataKill dataKill when trigger is QuestTriggerKill killTrigger:
                    if (dataKill.bundlesToKill != null && dataKill.bundlesToKill.Any(bg => bg.Valid()))
                    {
                        killTrigger.objectsToKill = new List<GameObject>();
                        foreach (var bundleGraph in dataKill.bundlesToKill.Where(bg => bg.Valid()))
                        {
                            AssignObjectByBundleGraph(
                                node,
                                bundleGraph,
                                settings,
                                basePos,
                                y,
                                delta,
                                foundObject => killTrigger.objectsToKill.Add(foundObject)
                            );
                        }
                    }
                    break;
                
            }
        }

      private static void AssignObjectByBundleGraph(
            QuestNode node,
            BundleGraph bundleGraph,
            Generator3D.Settings settings,
            Vector3 basePos,
            float y,
            Vector3 delta,
            Action<GameObject> assignAction)
      {
            // Calculate the world position of the BundleGraph's position
            var scenePosition = GetScenePosition(bundleGraph.Area, settings, basePos, y, delta);

            // Find objects at the position with LBSGenerated component using physics query
            var colliders = Physics.OverlapSphere(scenePosition, ProbeRadius);
            if (colliders == null || colliders.Length == 0)
            {
                Debug.LogWarning($"OverlapSphere collider empty, no objects found.");
                return;
            }

            for (int i = 0; i < colliders.Length; i++)
            {
                var collider = colliders[i];
                if (collider == null) continue;

                var lbsGenerated = collider.GetComponent<LBSGenerated>();
                if (lbsGenerated == null || lbsGenerated.BundleRef == null) continue;
                if (lbsGenerated.LayerName != bundleGraph.GetLayerName()) continue; 

                Bundle bundleRef = LBSAssetMacro.LoadAssetByGuid<Bundle>(bundleGraph.GetGuid());
                if (lbsGenerated.BundleRef != bundleRef) continue;

                assignAction?.Invoke(collider.gameObject);
                return;
            }

            Debug.LogWarning($"No object with LBSGenerated component and matching BundleRef Guid '{bundleGraph.GetGuid()}' found at position {scenePosition} for node {node.ID}");
        }

        private static Vector3 GetScenePosition(Rect graphArea, Generator3D.Settings settings, Vector3 basePos, float y,
            Vector3 delta)
        {
            var bundlePosX = graphArea.x * settings.scale.x;
            var bundlePosZ = graphArea.y * settings.scale.y;
            var scenePosition = basePos + new Vector3(bundlePosX, y, bundlePosZ) - delta;
            return scenePosition;
        }


        /// <summary>
        /// Creates the ui document class (that's displayed during game mode) and
        /// adds it into the layer generated game object
        /// </summary>
        /// <param name="pivotTransform"> transform to assign the UI as child</param>
        private void CreateUIDocument(Transform pivotTransform, GameObject observerGameObject)
        {
            GameObject uiGameObject = new GameObject("UIDocument");
            UIDocument uiDocument = uiGameObject.AddComponent<UIDocument>();
           
            if (!uiGameObject) return;
            var questVisualTree = uiGameObject.AddComponent<QuestVisualTree>();
            var uiAsset = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestVisualTree");
            var panelSettings = LBSAssetMacro.LoadAssetByGuid<PanelSettings>("da6adae693698d3409943a20661e2031");

            if (!uiAsset || !panelSettings) return;

            questVisualTree.Observer = observerGameObject;
            uiDocument.visualTreeAsset = uiAsset;
            uiDocument.panelSettings = panelSettings;
            uiGameObject.transform.SetParent(pivotTransform);
        }
    }
}