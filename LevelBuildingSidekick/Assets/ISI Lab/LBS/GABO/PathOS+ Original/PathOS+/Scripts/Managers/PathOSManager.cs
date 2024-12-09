using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using PathOS;
using Unity.AI.Navigation;
using System.Linq;
using ISILab.LBS.Components;
using UnityEngine.AI;
using UnityEngine.Rendering.VirtualTexturing;

/*
PathOSManager.cs 
PathOSManager (c) Nine Penguins (Samantha Stahlke) 2018 Atiya Nova 2021
*/

//Simple class for defining entities in the level.
public class PathOSManager : NPSingleton<PathOSManager>
{
    public const string simulationEndedEditorPrefsID = "PathOSSimulationEndFlag";
    public bool limitSimulationTime = false;

    [Tooltip("Maximum runtime in seconds (uses unscaled time)")]
    public float maxSimulationTime = 180.0f;

    public bool endOnCompletionGoal = true;
    public bool simulationEnded { get; set; }

    public bool showLevelMarkup = true;
    public List<GameObject> ignoredEntities = new List<GameObject>();
    public List<LevelEntity> levelEntities = new List<LevelEntity>();
    public List<HeuristicWeightSet> heuristicWeights = new List<HeuristicWeightSet>();

    public GameObject curMouseover { get; set; }

    public Dictionary<EntityType, string> entityGizmoLookup = new Dictionary<EntityType, string>
    {
        {EntityType.ET_NONE, "entity_null" },
        {EntityType.ET_GOAL_OPTIONAL, "goal_optional" },
        {EntityType.ET_GOAL_MANDATORY, "goal_mandatory" },
        {EntityType.ET_GOAL_COMPLETION, "goal_completion" },
        {EntityType.ET_RESOURCE_ACHIEVEMENT, "resource_achievement" },
        {EntityType.ET_RESOURCE_PRESERVATION_LOW, "resource_preservation_low" },
        {EntityType.ET_RESOURCE_PRESERVATION_MED, "resource_preservation_med" },
        {EntityType.ET_RESOURCE_PRESERVATION_HIGH, "resource_preservation_high" },
        {EntityType.ET_HAZARD_ENEMY_LOW, "hazard_enemy_low" },
        {EntityType.ET_HAZARD_ENEMY_MED, "hazard_enemy_medium" },
        {EntityType.ET_HAZARD_ENEMY_HIGH, "hazard_enemy_high" },
        {EntityType.ET_HAZARD_ENEMY_BOSS, "hazard_enemy_boss" },
        {EntityType.ET_HAZARD_ENVIRONMENT, "hazard_environment" },
        {EntityType.ET_POI, "poi_environment" },
        {EntityType.ET_POI_NPC, "poi_npc" }
    };


    public Dictionary<EntityType, string> entityLabelLookup = new Dictionary<EntityType, string>
    {
        {EntityType.ET_NONE, "Undefined Type (unaffected by agent motives)" },
        {EntityType.ET_GOAL_OPTIONAL, "Optional Goal" },
        {EntityType.ET_GOAL_MANDATORY, "Mandatory Goal" },
        {EntityType.ET_GOAL_COMPLETION, "Final Goal" },
        {EntityType.ET_RESOURCE_ACHIEVEMENT, "Collectible" },
        {EntityType.ET_RESOURCE_PRESERVATION_LOW, "Self-Preservation Item (e.g., low health)" },
        {EntityType.ET_RESOURCE_PRESERVATION_MED, "Self-Preservation Item (e.g., medium health)" },
        {EntityType.ET_RESOURCE_PRESERVATION_HIGH, "Self-Preservation Item (e.g., high health)" },
        {EntityType.ET_HAZARD_ENEMY_LOW, "Enemy Hazard Low" },
        {EntityType.ET_HAZARD_ENEMY_MED, "Enemy Hazard Medium" },
        {EntityType.ET_HAZARD_ENEMY_HIGH, "Enemy Hazard High" },
        {EntityType.ET_HAZARD_ENEMY_BOSS, "Enemy Hazard Boss" },
        {EntityType.ET_HAZARD_ENVIRONMENT, "Environmental Hazard" },
        {EntityType.ET_POI, "Point-of-Interest (e.g., setpiece)" },
        {EntityType.ET_POI_NPC, "NPC (non-hostile)" }
    };

    private float simulationTimer = 0.0f;

    private List<PathOSAgent> agents = new List<PathOSAgent>();

    //Combat/health variables
    public bool endSimulationOnDeath = false;

    private void Awake()
    {
        bool warned = false;

        for (int i = levelEntities.Count - 1; i >= 0; --i)
        {
            if (null == levelEntities[i].objectRef
                || !levelEntities[i].objectRef.activeInHierarchy)
            {
                if (!warned)
                {
                    NPDebug.LogWarning("One or more PathOS entities in the " +
                        "scene is null or inactive in the scene. It will be " +
                        "ignored during the simulation.");

                    warned = true;
                }

                levelEntities.RemoveAt(i);
                continue;
            }

            //Set up entity (grab renderers, calculate squared visitation radius).
            levelEntities[i].Init();
        }

        simulationEnded = false;
        ResizeWeightMatrix();

#if UNITY_EDITOR
        EditorPrefs.SetBool(simulationEndedEditorPrefsID, false);
#endif
    }

    private void Start()
    {
        agents.AddRange(FindObjectsOfType<PathOSAgent>());
    }

    private void Update()
    {
        simulationTimer += Time.unscaledDeltaTime;

#if UNITY_EDITOR

        bool stopSimulation = (limitSimulationTime && simulationTimer > maxSimulationTime) || (endSimulationOnDeath && AreAllAgentsDead());

        if (!stopSimulation)
        {
            stopSimulation = true;

            for (int i = 0; i < agents.Count; ++i)
            {
                if (!agents[i].completed)
                {
                    stopSimulation = false;
                    break;
                }
            }
        }

        if (stopSimulation)
        {
            EditorPrefs.SetBool(simulationEndedEditorPrefsID, true);

            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }
#endif
    }

    private void OnDrawGizmosSelected()
    {

#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            if (showLevelMarkup)
            {
                foreach (LevelEntity entity in levelEntities)
                {
                    if (entity.objectRef != null)
                        Gizmos.DrawIcon(entity.objectRef.transform.position,
                            entityGizmoLookup[entity.entityType] + ".png");
                }
            }

            if (curMouseover != null)
            {
                Matrix4x4 oldGizmos = Gizmos.matrix;
                Color oldGizmosColor = Gizmos.color;

                List<Matrix4x4> mouseoverMeshTransforms = new List<Matrix4x4>();
                List<Mesh> mouseoverMeshes = new List<Mesh>();

                MeshFilter[] filters = curMouseover.GetComponentsInChildren<MeshFilter>();
                SkinnedMeshRenderer[] renderers = curMouseover.GetComponentsInChildren<SkinnedMeshRenderer>();

                foreach (MeshFilter filter in filters)
                {
                    mouseoverMeshes.Add(filter.sharedMesh);
                    mouseoverMeshTransforms.Add(filter.transform.localToWorldMatrix);
                }

                foreach (SkinnedMeshRenderer renderer in renderers)
                {
                    mouseoverMeshes.Add(renderer.sharedMesh);
                    mouseoverMeshTransforms.Add(renderer.transform.localToWorldMatrix);
                }

                for (int i = 0; i < mouseoverMeshes.Count; ++i)
                {
                    Gizmos.matrix = mouseoverMeshTransforms[i];
                    Gizmos.DrawWireMesh(mouseoverMeshes[i]);
                }

                curMouseover = null;
                Gizmos.matrix = oldGizmos;
                Gizmos.color = oldGizmosColor;
            }
        }
#endif
    }

    /// <summary>
    /// GABO: Allows addition of level entities from outside.
    /// </summary>
    /// <param name="currGameObject"> Object to be tagged. </param>
    /// <param name="entityType"> Type of PathOS entity. </param>
    public LevelEntity AddLevelEntity(GameObject currGameObject, EntityType entityType)
    {
        LevelEntity newEntity = new(currGameObject, entityType);
        levelEntities.Add(newEntity);
        return newEntity;
    }

    // GABO
    // GABO TODO: Make it usable with MULTIPLE agents! (currently it can't due to navmesh changes affecting all agents)
    /// <summary>
    /// Toggle dynamic obstacle connections of this entity (if any) for the given agent.
    /// Includes both marked up entities and Testing Layer walls.
    /// If Testing Layer walls were added or removed, re-bakes NavMesh.
    /// </summary>
    /// <param name="agent"> Agent which is being affected by the toggling. </param>
    /// <param name="entity"> Entity which dynamic obstacles we're toggling.</param>
    public void ToggleDynamicObstacles(PathOSAgent agent, LevelEntity entity)
    {
        // Abort if no dynamic obstacles connected
        if (entity.dynamicObstacles.Count == 0) { return; }

        bool IsWallsAddedOrRemoved = false; // Modify NavMesh only if walls were added or removed
        foreach (EntityObstaclePair connectedObject in entity.dynamicObstacles)
        {
            // CLOSE: Make object visible
            if (connectedObject.connectionType == PathOSObstacleConnections.Category.CLOSE)
            {
                // Walls
                if (connectedObject.entityObjectRef.name == "WallPrefab")
                {
                    agent.eyes.RemoveInvisibleWall(connectedObject.entityObjectRef);
                    IsWallsAddedOrRemoved = true;
                }
                // Entities
                else
                {
                    agent.eyes.RemoveInvisibleEntity(GetEntity(connectedObject.entityObjectRef));
                }
            }
            // OPEN: Make object invisible
            else if (connectedObject.connectionType == PathOSObstacleConnections.Category.OPEN)
            {
                // Walls
                if (connectedObject.entityObjectRef.name == "WallPrefab")
                {
                    agent.eyes.AddInvisibleWall(connectedObject.entityObjectRef);
                    IsWallsAddedOrRemoved = true;
                }
                // Entities
                else
                {
                    agent.eyes.AddInvisibleEntity(GetEntity(connectedObject.entityObjectRef));
                }
            }
            else
            {
                Debug.LogError("EntityObstaclePair object has no connectionType set!");
            }
        }

        // Re-Bake NavMesh (if walls were added or removed)
        if (IsWallsAddedOrRemoved)
        {
            GenerateNavMeshFromLBSModules(agent);
        }
    }

    // GABO: Regenerate NavMesh considering Exterior, Interior and Testing Layer objects.
    // Testing Layer "Walls" instances considered depend on given agent.
    // GABO TODO: ARREGLAR PARA FUNCIONAR CON MULTIPLES AGENTES
    public void GenerateNavMeshFromLBSModules(PathOSAgent affectedAgent)
    {
        // Variables
        bool newAgentIdFound = false;
        int currentSmallestUnassignedIdIndex = -1;
        // Re-bake NavMesh (based on PathOSRuleGenerator.GenerateNavMesh(), but using existing NavMeshSurface object)
        GameObject surfaceGameObject = new GameObject("TemporaryNavMeshSurface");
        NavMeshSurface surface = surfaceGameObject.AddComponent<NavMeshSurface>();

        // Assign new agent type to affectedAgent in order to separate its NavMesh from other agents (if it hasn't been done yet).
        // *** Due to Unity's limitations to create and modify agent types at runtime, 8 "Temporary Agent <X>" types
        // were created in the editor to cover for the maximum of 8 simultaneous agents used in PathOS batching system.
        // In the meantime, limit use to 8 agents at the same time, or add more agent types manually.
        PathOSAgent[] activeAgents = FindObjectsOfType<PathOSAgent>(false);
        // 8-maximum check
        if (activeAgents.Length > 8) { Debug.LogWarning("There shouldn't be more than 8 concurrent agents!"); return; }
        // "Already-changed-agent-type" check //GABO TODO: Buscar una manera menos propensa a error de deteccion
        else if (affectedAgent.GetComponent<NavMeshAgent>().agentTypeID != 0)
        {
            surface.agentTypeID = affectedAgent.GetComponent<NavMeshAgent>().agentTypeID;
        }
        else
        {
            currentSmallestUnassignedIdIndex = 1;
            while (true)
            {
                bool isAgentTypeTaken = false;
                foreach (PathOSAgent agent in activeAgents)
                {
                    if (agent.GetComponent<NavMeshAgent>().agentTypeID == NavMesh.GetSettingsByIndex(currentSmallestUnassignedIdIndex).agentTypeID)
                    {
                        isAgentTypeTaken = true;
                        break;
                    }
                }
                if (isAgentTypeTaken)
                {
                    if (currentSmallestUnassignedIdIndex == 8)
                    {
                        Debug.LogError("There shouldn't be more than 8 extra agent types needed! Check code.");
                        return;
                    }
                    currentSmallestUnassignedIdIndex++;
                }
                else
                {
                    newAgentIdFound = true;
                    break;
                }
            }
        }

        // Surface parameters
        surface.defaultArea = NavMesh.GetAreaFromName("Walkable");
        surface.layerMask = Physics.AllLayers;
        surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        surface.collectObjects = CollectObjects.Children;
        surface.overrideVoxelSize = false;
        surface.overrideTileSize = false;

        // Interior Layers: GameObjects
        List<GameObject> interiorLayerGameObjects = GameObject.FindObjectsOfType<GameObject>().Where(
            obj => obj.transform.childCount == 2 &&
            obj.transform?.GetChild(0).name == "Schema" &&
            obj.transform?.GetChild(1).name == "Schema outside").ToList();

        // Exterior Layers: GameObjects
        List<GameObject> exteriorLayerGameObjects = GameObject.FindObjectsOfType<GameObject>().Where(
            obj => obj.transform.childCount == 1 &&
            obj.transform?.GetChild(0).name == "Exterior").ToList();
        // Testing Layers: Active walls (depend on given agent)
        List<GameObject> testingLayerWalls = GameObject.FindObjectsOfType<GameObject>().Where(
            obj => //obj.name == "WallPrefab" &&
            obj.transform.parent?.name == "Walls" &&
            obj.transform.parent.parent?.name == "PathOS+" &&
            !affectedAgent.eyes.invisibleWalls.Contains(obj)).ToList();

        // If none found, sends warning.
        if (interiorLayerGameObjects.Count == 0 && exteriorLayerGameObjects.Count == 0 && testingLayerWalls.Count == 0)
        {
            Debug.LogWarning("No standard instances of Interior/Exterior layers, or Testing Layer walls, found. " +
                "NavMesh won't be re-baked! " +
                "Avoid changing first-order children name structure " +
                "(i.e. \"Schema\" (0) and \"Schema Outside\" (1) names for an Interior layer).");
            return;
        }

        // Arrays to save old parent transforms for later reinsertion
        GameObject[] interiorOldParents = new GameObject[interiorLayerGameObjects.Count];
        GameObject[] exteriorOldParents = new GameObject[exteriorLayerGameObjects.Count];
        GameObject[] wallsOldParents = new GameObject[testingLayerWalls.Count];

        // Surface object becomes temporal parent
        GameObject tempParent = surface.gameObject;

        // Interior Layers: New temporary parent
        for (int i = 0; i < interiorLayerGameObjects.Count; i++)
        {
            interiorOldParents[i] = interiorLayerGameObjects[i].transform.parent?.gameObject;
            // Add interior objects to temporary parent
            interiorLayerGameObjects[i].transform.parent = tempParent.transform;
        }
        // Exterior Layers: New temporary parent
        for (int i = 0; i < exteriorLayerGameObjects.Count; i++)
        {
            exteriorOldParents[i] = exteriorLayerGameObjects[i].transform.parent?.gameObject;
            // Add exterior objects to temporary parent
            exteriorLayerGameObjects[i].transform.parent = tempParent.transform;
        }
        // Testing Layer walls: New temporary parent
        for (int i = 0; i < testingLayerWalls.Count; i++)
        {
            wallsOldParents[i] = testingLayerWalls[i].transform.parent?.gameObject;
            // Add walls to temporary parent
            testingLayerWalls[i].transform.parent = tempParent.transform;
        }

        // Generate new NavMesh
        // ***Seems to need to go like this to prevent "Not close to NavMesh" warnings" when assigning new ids
        if (newAgentIdFound)
        {
            surface.agentTypeID = NavMesh.GetSettingsByIndex(currentSmallestUnassignedIdIndex).agentTypeID;
            surface.BuildNavMesh();
            affectedAgent.GetComponent<NavMeshAgent>().agentTypeID = NavMesh.GetSettingsByIndex(currentSmallestUnassignedIdIndex).agentTypeID;
        }
        else
        {
            surface.BuildNavMesh();
        }

        // Interior Layers: Reassign original parent
        for (int i = 0; i < interiorLayerGameObjects.Count; i++)
        {
            interiorLayerGameObjects[i].transform.parent = interiorOldParents[i]?.transform;
        }
        // Exterior Layers: Reassign original parent
        for (int i = 0; i < exteriorLayerGameObjects.Count; i++)
        {
            exteriorLayerGameObjects[i].transform.parent = exteriorOldParents[i]?.transform;
        }
        // Testing Layer walls: Reassign original parent
        for (int i = 0; i < testingLayerWalls.Count; i++)
        {
            testingLayerWalls[i].transform.parent = wallsOldParents[i]?.transform;
        }
    }

    private bool AreAllAgentsDead()
    {
        for (int i = 0; i < agents.Count; ++i)
        {
            if (!agents[i].IsDead())
            {
                return false;
            }
        }

        return true;
    }

    public void ClearEntities()
    {
        levelEntities.Clear();
    }

    public void ResizeWeightMatrix()
    {
        //Save existing values and rebuild the weight matrix.
        Dictionary<(Heuristic, EntityType), float> weights =
            new Dictionary<(Heuristic, EntityType), float>();

        for (int i = 0; i < heuristicWeights.Count; ++i)
        {
            for (int j = 0; j < heuristicWeights[i].weights.Count; ++j)
            {
                weights.Add((heuristicWeights[i].heuristic,
                    heuristicWeights[i].weights[j].entype),
                    heuristicWeights[i].weights[j].weight);
            }
        }

        heuristicWeights.Clear();

        foreach (Heuristic heuristic in System.Enum.GetValues(typeof(Heuristic)))
        {
            heuristicWeights.Add(new HeuristicWeightSet(heuristic));
            HeuristicWeightSet newWeights = heuristicWeights[heuristicWeights.Count - 1];

            foreach (EntityType entype in System.Enum.GetValues(typeof(EntityType)))
            {
                float weight = 0.0f;

                if (weights.ContainsKey((heuristic, entype)))
                    weight = weights[(heuristic, entype)];

                newWeights.weights.Add(new EntityWeight(entype, weight));
            }
        }
    }

    public bool ImportWeights(string filename)
    {
        if (!File.Exists(filename) || filename.Substring(filename.Length - 3) != "csv")
        {
            //Only show an error if some filename was selected.
            //(Don't throw an error if the user cancels out of the dialog.)
            if (filename != "")
                NPDebug.LogError("Could not load heuristic weights! " +
                    "PathOS heuristic weights can only be imported from a " +
                    "valid local .csv file.");

            return false;
        }

        Dictionary<(Heuristic, EntityType), float> weights =
            new Dictionary<(Heuristic, EntityType), float>();

        List<EntityType> entypes = new List<EntityType>();

        StreamReader sr = new StreamReader(filename);
        char[] sep = { ',' };

        string line = sr.ReadLine();

        string[] headerTypes = line.Split(sep, System.StringSplitOptions.RemoveEmptyEntries);

        while ((line = sr.ReadLine()) != null)
        {
            string[] lineContents = line.Split(sep, System.StringSplitOptions.RemoveEmptyEntries);

            if (lineContents.Length < 1)
                continue;

            Heuristic heuristic;

            if (System.Enum.TryParse<Heuristic>(lineContents[0], out heuristic))
            {
                for (int i = 1; i < lineContents.Length; ++i)
                {
                    if (i - 1 > headerTypes.Length)
                        break;

                    EntityType entype;

                    if (System.Enum.TryParse<EntityType>(headerTypes[i - 1], out entype))
                        weights[(heuristic, entype)] = float.Parse(lineContents[i]);
                }
            }
        }

        sr.Close();

        for (int i = 0; i < heuristicWeights.Count; ++i)
        {
            for (int j = 0; j < heuristicWeights[i].weights.Count; ++j)
            {
                (Heuristic, EntityType) key = (heuristicWeights[i].heuristic,
                    heuristicWeights[i].weights[j].entype);

                if (weights.ContainsKey(key))
                    heuristicWeights[i].weights[j].weight = weights[key];
            }
        }

        return true;
    }

    public void ExportWeights(string filename)
    {
        if (filename == "")
            return;

        ResizeWeightMatrix();

        StreamWriter sw = new StreamWriter(filename);

        sw.Write(",");

        foreach (EntityType entype in System.Enum.GetValues(typeof(EntityType)))
        {
            sw.Write(entype.ToString() + ",");
        }

        sw.Write("\n");

        for (int i = 0; i < heuristicWeights.Count; ++i)
        {
            sw.Write(heuristicWeights[i].heuristic.ToString() + ",");

            for (int j = 0; j < heuristicWeights[i].weights.Count; ++j)
            {
                sw.Write(heuristicWeights[i].weights[j].weight + ",");
            }

            sw.Write("\n");
        }

        sw.Close();
    }

    // GABO: Obtains LevelEntity from a given GameObject
    public LevelEntity GetEntity(GameObject obj)
    {
        return levelEntities.Find(ent => ent.objectRef == obj);
    }
}
