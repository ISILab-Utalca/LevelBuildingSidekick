﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using PathOS;
using ISILab.LBS.Components; // GABO: Added to use PathOSObstacleConnections.Category

/*
PathOSAgentEyes.cs 
PathOSAgentEyes (c) Nine Penguins (Samantha Stahlke) 2018-19
*/

[RequireComponent(typeof(PathOSAgent))]
public class PathOSAgentEyes : MonoBehaviour
{
    private PathOSAgent agent;
    private static PathOSManager manager;

    //The agent's "eyes" - i.e., the camera the player would use.
    [PathOSDisplayName("Player Camera")]
    [Tooltip("The camera representing the player's view (agent's \"eyes\")")]
    public Camera cam;

    [Header("Navmesh \"Sight\"")]

    [PathOSDisplayName("Visibility Size Threshold")]
    [Tooltip("How large must an object appear on screen (expressed" +
        "as a percentage of viewport width) before it can be considered" +
        "visible?")]
    public float visSizeThreshold = 0.05f;

    [PathOSDisplayName("Raycast Distance")]
    [Tooltip("How far the agent \"looks\" over the navmesh " +
        "when scanning for obstacles/exploration targets. " +
        "(This value should be positive.)")]

    public float navmeshCastDistance = 50.0f;

    [PathOSDisplayName("Raycast Height")]
    [Tooltip("The y-value at which the agent \"looks\" for " +
        "obstacles/navigation targets.")]

    public float navmeshCastHeight = 5.0f;

    //What can the agent "see" currently?
    public List<PerceivedEntity> visible { get; set; }
    public List<PerceivedEntity> perceptionInfo { get; set; }

    //Timer to handle visual processing checks. Roll for perception.
    private float perceptionTimer = 0.0f;

    //Vertex set for visibility checks on object bounds.
    private Vector3[] boundsCheck;

    //Field of view for immediate-range "explorability" checks.
    private float xFOV;

    // GABO: List of invisible marked up entities (to this agent)
    public List<LevelEntity> invisibleEntities { get; private set; }
    // GABO: List of invisible walls (to this agent)
    public List<GameObject> invisibleWalls { get; private set; }

    void Awake()
    {
        agent = GetComponent<PathOSAgent>();

        visible = new List<PerceivedEntity>();
        perceptionInfo = new List<PerceivedEntity>();

        if (null == manager)
            manager = PathOSManager.instance;

        for (int i = 0; i < manager.levelEntities.Count; ++i)
        {
            LevelEntity entity = manager.levelEntities[i];
            Vector3 entityPos = entity.objectRef.transform.position;

            Vector3 entityVecXZ = entityPos - cam.transform.position;
            entityVecXZ.y = 0.0f;

            perceptionInfo.Add(new PerceivedEntity(entity));
        }

        boundsCheck = new Vector3[8];

        xFOV = Mathf.Rad2Deg * 2.0f * Mathf.Atan(
            cam.aspect * Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f));

        // GABO: Setup empty list of invisible entities and walls
        invisibleEntities = new();
        invisibleWalls = new();
    }

    void Update()
    {
        perceptionTimer += Time.deltaTime;

        //Visual processing update.
        if (perceptionTimer >= PathOS.Constants.Perception.PERCEPTION_COMPUTE_TIME)
        {
            ProcessPerception();
        }
    }

    public float XFOV()
    {
        return xFOV;
    }

    public void ProcessPerception()
    {
        Plane[] frustum = GeometryUtility.CalculateFrustumPlanes(cam);

        visible.Clear();

        Vector3 camForwardXZ = cam.transform.forward;
        camForwardXZ.y = 0.0f;

        for (int i = 0; i < perceptionInfo.Count; ++i)
        {
            PerceivedEntity entity = perceptionInfo[i];

            Vector3 entityPos = entity.entityRef.objectRef.transform.position;

            Vector3 entityVecXZ = entityPos - cam.transform.position;
            entityVecXZ.y = 0.0f;

            bool wasVisible = entity.visible;

            //Visibility check - this can change between checks as the agent
            //moves around.
            entity.entityRef.UpdateBounds();

            entity.visible =
                !invisibleEntities.Contains(entity.entityRef) // GABO: Ignore invisible entities (to be used for OPEN entities)
                && Vector3.Dot(camForwardXZ, entityVecXZ) > 0
                && GeometryUtility.TestPlanesAABB(frustum, entity.entityRef.bounds)
                && entity.entityRef.SizeVisibilityCheck(cam, visSizeThreshold)
                && RaycastVisibilityCheck(entity.entityRef.bounds, entityPos); // GABO: Modified to ignore invisible walls.

            if (wasVisible != entity.visible)
            {
                entity.visibilityTimer = 0.0f;
                entity.impressionMade = false;
            }

            //Keep track of how long the object has been in the current visibility state.
            entity.visibilityTimer = entity.visibilityTimer + perceptionTimer;

            if (entity.visible)
            {
                visible.Add(entity);

                if (entity.visibilityTimer >= PathOS.Constants.Memory.IMPRESSION_TIME_MIN)
                {
                    if (!entity.impressionMade)
                    {
                        entity.impressionMade = true;

                        if (entity.impressionCount < PathOS.Constants.Memory.IMPRESSIONS_MAX)
                            ++entity.impressionCount;
                    }

                    entity.perceivedPos = entityPos;
                    agent.memory.Memorize(entity);

                    //Mandatory/completion/boss goals are committed to LTM automatically.
                    if (entity.entityType == EntityType.ET_GOAL_MANDATORY
                        || entity.entityType == EntityType.ET_GOAL_COMPLETION
                         || entity.entityType == EntityType.ET_HAZARD_ENEMY_BOSS)
                        agent.memory.CommitUnforgettable(entity);

                    agent.memory.TryCommitLTM(entity);
                }
            }
        }

        perceptionTimer = 0.0f;
    }

    //Uses an AABB and given position as nine points for checking
    //visibility via raycast.
    bool RaycastVisibilityCheck(Bounds bounds, Vector3 pos)
    {
        Vector3 ray = cam.transform.position - pos;

        // GABO: We add the invisible walls (and children) to the "Ignore Raycast" layer
        // temporarily and mask the raycasts with it so they're ignored.
        Dictionary<GameObject, int> oldLayers = new();
        foreach (GameObject wall in invisibleWalls)
        {
            // Parent
            oldLayers[wall] = wall.layer;
            wall.layer = LayerMask.NameToLayer("Ignore Raycast");
            // Children
            Transform[] children = wall.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                // Ignore self to avoid bugs
                if (child.transform == wall.transform) { continue; }

                oldLayers[child.gameObject] = child.gameObject.layer;
                child.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            }
        }

        if (!Physics.Raycast(pos, ray.normalized, ray.magnitude, ~LayerMask.GetMask("Ignore Raycast")))
        {
            // GABO: We return invisible walls (and children) to their original layer
            foreach (GameObject wall in invisibleWalls)
            {
                wall.layer = oldLayers[wall];
                Transform[] children = wall.GetComponentsInChildren<Transform>();
                foreach (Transform child in children)
                {
                    // Ignore self to avoid bugs
                    if (child.transform == wall.transform) { continue; }

                    child.gameObject.layer = oldLayers[child.gameObject];
                }
            }
            return true;
        }

        boundsCheck[0].Set(bounds.min.x, bounds.min.y, bounds.min.z);
        boundsCheck[1].Set(bounds.min.x, bounds.min.y, bounds.max.z);
        boundsCheck[2].Set(bounds.min.x, bounds.max.y, bounds.min.z);
        boundsCheck[3].Set(bounds.min.x, bounds.max.y, bounds.max.z);
        boundsCheck[4].Set(bounds.max.x, bounds.min.y, bounds.min.z);
        boundsCheck[5].Set(bounds.max.x, bounds.min.y, bounds.max.z);
        boundsCheck[6].Set(bounds.max.x, bounds.max.y, bounds.min.z);
        boundsCheck[7].Set(bounds.max.x, bounds.max.y, bounds.max.z);

        for (int i = 0; i < boundsCheck.Length; ++i)
        {
            ray = cam.transform.position - boundsCheck[i];

            if (!Physics.Raycast(boundsCheck[i], ray.normalized, ray.magnitude, ~LayerMask.GetMask("Ignore Raycast")))
            {
                // GABO: We return invisible walls (and children) to their original layer
                foreach (GameObject wall in invisibleWalls)
                {
                    wall.layer = oldLayers[wall];
                    Transform[] children = wall.GetComponentsInChildren<Transform>();
                    foreach (Transform child in children)
                    {
                        // Ignore self to avoid bugs
                        if (child.transform == wall.transform) { continue; }

                        child.gameObject.layer = oldLayers[child.gameObject];
                    }
                }
                return true;
            }
        }

        // GABO: We return invisible walls (and children) to their original layer
        foreach (GameObject wall in invisibleWalls)
        {
            wall.layer = oldLayers[wall];
            Transform[] children = wall.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                // Ignore self to avoid bugs
                if (child.transform == wall.transform) { continue; }

                child.gameObject.layer = oldLayers[child.gameObject];
            }
        }

        return false;
    }

    //This should be updated eventually to do a more sophisticated check accounting
    //for *apparent* distance - i.e., by adding a couple of physics raycasts from the 
    //camera.
    public NavMeshHit ExploreVisibilityCheck(Vector3 origin, Vector3 dir)
    {
        NavMeshHit hit = new NavMeshHit();
        bool result = NavMesh.Raycast(origin,
            origin + dir.normalized * navmeshCastDistance + Vector3.up * navmeshCastHeight,
            out hit, NavMesh.AllAreas);


        PathOSNavUtility.NavmeshMemoryMapper.NavmeshMapCode fillCode =
            (result) ?
            PathOSNavUtility.NavmeshMemoryMapper.NavmeshMapCode.NM_OBSTACLE :
            PathOSNavUtility.NavmeshMemoryMapper.NavmeshMapCode.NM_SEEN;

        agent.memory.memoryMap.Fill(hit.position, fillCode);

        PathOSNavUtility.NavmeshMemoryMapper.NavmeshMemoryMapperCastHit memHit =
            new PathOSNavUtility.NavmeshMemoryMapper.NavmeshMemoryMapperCastHit();

        agent.memory.memoryMap.RaycastMemoryMap(origin, dir, hit.distance,
            out memHit, true);

        return hit;
    }

    // GABO: Add to list of invisible objects, so this agent can't see it.
    public void AddInvisibleEntity(LevelEntity newInvisibleEntity)
    {
        invisibleEntities.Add(newInvisibleEntity);
    }
    // GABO: Remove from list of invisible objects, making it visible again.
    public void RemoveInvisibleEntity(LevelEntity invisibleEntityToRemove)
    {
        invisibleEntities.Remove(invisibleEntityToRemove);
    }
    // GABO: Add wall to list of invisible walls, so this agent can see through them.
    public void AddInvisibleWall(GameObject wall)
    {
        invisibleWalls.Add(wall);
    }
    // GABO: Remove wall from list of invisible walls, so they block this agent's view.
    public void RemoveInvisibleWall(GameObject wall)
    {
        invisibleWalls.Remove(wall);
    }
}
