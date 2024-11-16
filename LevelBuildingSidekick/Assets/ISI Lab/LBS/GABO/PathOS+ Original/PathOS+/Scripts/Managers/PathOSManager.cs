﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using PathOS;

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
            if(null == levelEntities[i].objectRef
                || !levelEntities[i].objectRef.activeInHierarchy)
            {
                if(!warned)
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
        if(!UnityEditor.EditorApplication.isPlaying)
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
            
            if(curMouseover != null)
            {          
                Matrix4x4 oldGizmos = Gizmos.matrix;
                Color oldGizmosColor = Gizmos.color;

                List<Matrix4x4> mouseoverMeshTransforms = new List<Matrix4x4>();
                List<Mesh> mouseoverMeshes = new List<Mesh>();

                MeshFilter[] filters = curMouseover.GetComponentsInChildren<MeshFilter>();
                SkinnedMeshRenderer[] renderers = curMouseover.GetComponentsInChildren<SkinnedMeshRenderer>();

                foreach(MeshFilter filter in filters)
                {
                    mouseoverMeshes.Add(filter.sharedMesh);
                    mouseoverMeshTransforms.Add(filter.transform.localToWorldMatrix);
                }

                foreach(SkinnedMeshRenderer renderer in renderers)
                {
                    mouseoverMeshes.Add(renderer.sharedMesh);
                    mouseoverMeshTransforms.Add(renderer.transform.localToWorldMatrix);
                }

                for(int i = 0; i < mouseoverMeshes.Count; ++i)
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

        for(int i = 0; i < heuristicWeights.Count; ++i)
        {
            for(int j = 0; j < heuristicWeights[i].weights.Count; ++j)
            {
                weights.Add((heuristicWeights[i].heuristic, 
                    heuristicWeights[i].weights[j].entype),
                    heuristicWeights[i].weights[j].weight);
            }
        }

        heuristicWeights.Clear();
        
        foreach(Heuristic heuristic in System.Enum.GetValues(typeof(Heuristic)))
        {
            heuristicWeights.Add(new HeuristicWeightSet(heuristic));
            HeuristicWeightSet newWeights = heuristicWeights[heuristicWeights.Count - 1];

            foreach(EntityType entype in System.Enum.GetValues(typeof(EntityType)))
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
            if(filename != "")
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

        while((line = sr.ReadLine()) != null)
        {
            string[] lineContents = line.Split(sep, System.StringSplitOptions.RemoveEmptyEntries);

            if (lineContents.Length < 1)
                continue;

            Heuristic heuristic;

            if(System.Enum.TryParse<Heuristic>(lineContents[0], out heuristic))
            {
                for(int i = 1; i < lineContents.Length; ++i)
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

        for(int i = 0; i < heuristicWeights.Count; ++i)
        {
            for(int j = 0; j < heuristicWeights[i].weights.Count; ++j)
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

        for(int i = 0; i < heuristicWeights.Count; ++i)
        {
            sw.Write(heuristicWeights[i].heuristic.ToString() + ",");
            
            for(int j = 0; j < heuristicWeights[i].weights.Count; ++j)
            {
                sw.Write(heuristicWeights[i].weights[j].weight + ",");
            }

            sw.Write("\n");
        }

        sw.Close();
    }
}
