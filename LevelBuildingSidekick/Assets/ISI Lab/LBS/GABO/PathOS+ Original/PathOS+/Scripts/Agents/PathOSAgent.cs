﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using PathOS;
using System.Linq;

/*
PathOSAgent.cs 
PathOSAgent (c) Samantha Stahlke and Atiya Nova 2018
*/

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PathOSAgentMemory))]
[RequireComponent(typeof(PathOSAgentEyes))]
public class PathOSAgent : MonoBehaviour
{
    /* OBJECT REFERENCES AND DEBUGGING */
    private NavMeshAgent navAgent;

    //The agent's memory/internal world model.
    public PathOSAgentMemory memory { get; set; }
    //The agent's eyes/perception model.
    public PathOSAgentEyes eyes { get; set; }

    private static PathOSManager manager;
    public static OGLogManager logger { get; set; }

    //Used for testing.
    [Range(1.0f, 8.0f)]
    public float timeScale = 1.0f;
    public bool freezeAgent;
    private bool verboseDebugging = false;

    /* PLAYER CHARACTERISTICS */

    [Range(0.0f, 1.0f)]
    public float experienceScale;

    public List<HeuristicScale> heuristicScales, modifiableHeuristicScales;
    private Dictionary<Heuristic, float> heuristicScaleLookup;
    private Dictionary<(Heuristic, EntityType), float> entityScoringLookup;

    /* NAVIGATION PROPERTIES */

    [Tooltip("How close (in units) does the agent have to get " +
        "to a goal to mark it as visited?")]
    public float visitThreshold = 1.0f;
    public float visitThresholdSqr { get; private set; }

    [Tooltip("How many degrees should separate lines of " +
        "sight checked for \"explorability\" by the agent?")]
    public float exploreDegrees = 5.0f;

    [PathOSDisplayName("Explore Degrees (Back)")]
    [Tooltip("How many degrees should separate paths checked for " +
        "\"explorability\" behind (out of sight of) the agent?")]
    public float invisibleExploreDegrees = 30.0f;

    [Tooltip("How many degrees should the agent sway to either " +
        "side when looking around?")]
    public float lookDegrees = 60.0f;

    [Tooltip("How close do two \"exploration\" goals have to " +
        "be to be considered the same?")]
    public float exploreThreshold = 2.0f;

    [Tooltip("What's the search radius for finding a point on the navmesh when " +
        "setting an exploration target?")]
    public float exploreTargetMargin = 25.0f;

    /* MEMORY STATS */
    //How quickly does the agent forget something in its memory?
    //This is for testing right now, basically just a flat value.
    public float forgetTime { get; set; }
    public int stmSize { get; set; }

    //Timers for handling rerouting and looking around.
    private float routeTimer = 0.0f;
    private float perceptionTimer = 0.0f;
    private float baseLookTime = PathOS.Constants.Behaviour.LOOK_TIME_MAX;
    private float lookTime = PathOS.Constants.Behaviour.LOOK_TIME_MAX;
    private float lookTimer = 0.0f;
    private bool lookingAround = false;

    //Where is the agent targeting?
    private TargetDest currentDest;
    private List<TargetDest> destList = new List<TargetDest>();
    private bool pathResolved = true;

    //Accumulates to prevent rapid changes in goal with no decision made.
    private int changeTargetCount = 0;

    //What is the total positive impact of all unvisited entities?
    //(Used to penalize level completion). 
    private float cumulativeEntityScore = 0.0f;
    private float pastCumulativeEntityScore = 0.0f;
    //Prevent initial selection of final goal as the target (edge case).
    private bool assessedGoalsInit = false;

    //Is the agent "finished" the level?
    public bool completed { get; set; }

    //For backtracking traversal.
    public float hazardPenalty { get; set; }
    private float memPathChance = PathOS.Constants.Behaviour.BASE_MEMORY_NAV_CHANCE;
    private bool onMemPath = false;
    private List<Vector3> memPathWaypoints;
    private Vector3 memWaypoint = Vector3.zero;

    private List<Vector3> unreachableReference;

    //Health variables
    private float health = 100.0f;
    private bool dead = false;
    public TimeRange lowEnemyDamage = new TimeRange(10, 30), medEnemyDamage = new TimeRange(30, 50),
        highEnemyDamage = new TimeRange(50, 70), bossEnemyDamage = new TimeRange(70, 100),
        hazardDamage = new TimeRange(10, 20), lowHealthGain = new TimeRange(10, 30),
        medHealthGain = new TimeRange(30, 60), highHealthGain = new TimeRange(70, 100);
    private int cautionIndex, aggressionIndex, adrenalineIndex;

    private GameObject cameraObject;
    private static bool cameraFollow = false;
    private void Awake()
    {
        eyes = GetComponent<PathOSAgentEyes>();
        memory = GetComponent<PathOSAgentMemory>();

        navAgent = GetComponent<NavMeshAgent>();
        completed = false;

        cameraObject = GameObject.FindWithTag("PathOSCamera");

        currentDest = new TargetDest();
        currentDest.pos = GetPosition();

        memPathWaypoints = new List<Vector3>();
        unreachableReference = new List<Vector3>();

        heuristicScaleLookup = new Dictionary<Heuristic, float>();
        entityScoringLookup = new Dictionary<(Heuristic, EntityType), float>();

        if (null == manager)
            manager = PathOSManager.instance;

        if (null == logger)
            logger = OGLogManager.instance;

        modifiableHeuristicScales.Clear();

        foreach (HeuristicScale curScale in heuristicScales)
        {
            modifiableHeuristicScales.Add(curScale);
            heuristicScaleLookup.Add(curScale.heuristic, curScale.scale);
        }

        //TODO: Clean this up...
        for (int i = 0; i < modifiableHeuristicScales.Count; i++)
        {
            if (modifiableHeuristicScales[i].heuristic == Heuristic.CAUTION)
            {
                cautionIndex = i;
            }
            else if (modifiableHeuristicScales[i].heuristic == Heuristic.AGGRESSION)
            {
                aggressionIndex = i;
            }
            else if (modifiableHeuristicScales[i].heuristic == Heuristic.ADRENALINE)
            {
                adrenalineIndex = i;
            }
        }

        foreach (HeuristicWeightSet curSet in manager.heuristicWeights)
        {
            for (int j = 0; j < curSet.weights.Count; ++j)
            {
                entityScoringLookup.Add((curSet.heuristic, curSet.weights[j].entype), curSet.weights[j].weight);
            }
        }

        float avgAggressionScore = 0.2f *
        (entityScoringLookup[(Heuristic.AGGRESSION, EntityType.ET_HAZARD_ENEMY_LOW)] +
        (entityScoringLookup[(Heuristic.AGGRESSION, EntityType.ET_HAZARD_ENEMY_MED)]) +
        (entityScoringLookup[(Heuristic.AGGRESSION, EntityType.ET_HAZARD_ENEMY_HIGH)]) +
        (entityScoringLookup[(Heuristic.AGGRESSION, EntityType.ET_HAZARD_ENEMY_BOSS)]) +
        entityScoringLookup[(Heuristic.AGGRESSION, EntityType.ET_HAZARD_ENVIRONMENT)]);

        float avgAdrenalineScore = 0.2f
            * (entityScoringLookup[(Heuristic.ADRENALINE, EntityType.ET_HAZARD_ENEMY_LOW)] +
              (entityScoringLookup[(Heuristic.AGGRESSION, EntityType.ET_HAZARD_ENEMY_MED)]) +
              (entityScoringLookup[(Heuristic.AGGRESSION, EntityType.ET_HAZARD_ENEMY_HIGH)]) +
              (entityScoringLookup[(Heuristic.AGGRESSION, EntityType.ET_HAZARD_ENEMY_BOSS)]) +
              entityScoringLookup[(Heuristic.ADRENALINE, EntityType.ET_HAZARD_ENVIRONMENT)]);

        float avgCautionScore = 0.2f
            * (entityScoringLookup[(Heuristic.CAUTION, EntityType.ET_HAZARD_ENEMY_LOW)] +
            (entityScoringLookup[(Heuristic.AGGRESSION, EntityType.ET_HAZARD_ENEMY_MED)]) +
            (entityScoringLookup[(Heuristic.AGGRESSION, EntityType.ET_HAZARD_ENEMY_HIGH)]) +
            (entityScoringLookup[(Heuristic.AGGRESSION, EntityType.ET_HAZARD_ENEMY_BOSS)]) +
            entityScoringLookup[(Heuristic.CAUTION, EntityType.ET_HAZARD_ENVIRONMENT)]);

        float hazardScore = heuristicScaleLookup[Heuristic.AGGRESSION] * avgAggressionScore
            + heuristicScaleLookup[Heuristic.ADRENALINE] * avgAdrenalineScore
            + heuristicScaleLookup[Heuristic.CAUTION] * avgCautionScore;

        hazardPenalty = -hazardScore;

        visitThresholdSqr = visitThreshold * visitThreshold;

        //Duration of working memory for game entities is scaled by experience level.
        forgetTime = Mathf.Lerp(PathOS.Constants.Memory.FORGET_TIME_MIN,
            PathOS.Constants.Memory.FORGET_TIME_MAX,
            experienceScale);

        //Capacitiy of working memory is also scaled by experience level.
        stmSize = Mathf.RoundToInt(Mathf.Lerp(PathOS.Constants.Memory.MEM_CAPACITY_MIN,
            PathOS.Constants.Memory.MEM_CAPACITY_MAX,
            experienceScale));

        //Base look time is scaled by curiosity.
        baseLookTime = Mathf.Lerp(PathOS.Constants.Behaviour.LOOK_TIME_MIN_EXPLORE,
            PathOS.Constants.Behaviour.LOOK_TIME_MAX,
            heuristicScaleLookup[Heuristic.CURIOSITY]);

        float memPathScale = (heuristicScaleLookup[Heuristic.CAUTION]
            + 1.0f - heuristicScaleLookup[Heuristic.CURIOSITY])
            * 0.5f;

        memPathChance = Mathf.Lerp(PathOS.Constants.Behaviour.MEMORY_NAV_CHANCE_MIN,
            PathOS.Constants.Behaviour.MEMORY_NAV_CHANCE_MAX,
            memPathScale);

        lookTime = baseLookTime;
    }

    private void Start()
    {
        LogAgentData();
        PerceptionUpdate();

        //Stochastic initialization of look time.
        lookTimer = Random.Range(0.0f, lookTime);
    }

    private void OnDestroy()
    {
        // GABO: Resets global time scale when destroyed (prevents affecting Time.timeScale beyond agent lifetime
        // when using Agent Batching)
        if (name.Contains("Temporary Batch Agent"))
        {
            Time.timeScale = 1.0f;
        }
    }

    private void LogAgentData()
    {
        if (logger != null)
        {
            string header = "";

            header += "HEURISTICS,";
            header += "EXPERIENCE," + experienceScale + ",";

            foreach (HeuristicScale scale in modifiableHeuristicScales)
            {
                header += scale.heuristic + "," + scale.scale + ",";
            }

            logger.WriteHeader(this.gameObject, header);
        }
    }

    public Vector3 GetPosition()
    {
        return navAgent.transform.position;
    }

    public void RecalibratePath()
    {
        navAgent.ResetPath();
        ResetDestinationSelf();
        //ComputeNewDestination();
    }

    public void ResetCamera()
    {
        if (cameraObject != null) cameraObject.transform.position = new Vector3(transform.position.x, 15.0f, transform.position.z);
    }

    public void ToggleCameraFollow()
    {
        cameraFollow = !cameraFollow;
    }

    private void UpdateLookTime()
    {
        lookTime = baseLookTime;

        //Actual look time can fluctuate based on the agent's caution and the 
        //danger in the current area.
        float lookTimeScale = memory.ScoreHazards(GetPosition()) *
            heuristicScaleLookup[Heuristic.CAUTION];

        lookTime = Mathf.Min(baseLookTime,
            Mathf.Lerp(PathOS.Constants.Behaviour.LOOK_TIME_MAX,
            PathOS.Constants.Behaviour.LOOK_TIME_MIN_CAUTION,
            lookTimeScale));
    }

    private float RouteComputeTimeCalculated()
    {
        return PathOS.Constants.Navigation.ROUTE_COMPUTE_BASE
            + PathOS.Constants.Memory.RETRIEVAL_TIME * memory.entities.Count;
    }

    //Used by the Inspector to ensure scale widgets will appear for all defined heuristics.
    //This SHOULD NOT be called by anything else.
    public void RefreshHeuristicList()
    {
        Dictionary<Heuristic, float> weights = new Dictionary<Heuristic, float>();

        for (int i = 0; i < modifiableHeuristicScales.Count; ++i)
        {
            weights.Add(modifiableHeuristicScales[i].heuristic, modifiableHeuristicScales[i].scale);
        }

        modifiableHeuristicScales.Clear();

        foreach (Heuristic heuristic in System.Enum.GetValues(typeof(Heuristic)))
        {
            float weight = 0.0f;

            if (weights.ContainsKey(heuristic))
                weight = weights[heuristic];

            modifiableHeuristicScales.Add(new HeuristicScale(heuristic, weight));
        }
    }

    public void UpdateWeightsBasedOnHealth()
    {
        //if (health <= 50.0f)
        //{
        //Variables for calculations
        float newCaution = 0, newAggression = 0, newAdrenaline = 0;
        float h = 1.0f - (health / 100.0f);

        //Updates the caution
        newCaution = Mathf.Lerp(heuristicScaleLookup[Heuristic.CAUTION], 1.0f, h);
        if (newCaution > 1.0f) newCaution = 1.0f;
        modifiableHeuristicScales[cautionIndex].scale = newCaution;

        //Cuts aggression/adrenaline by half
        newAggression = Mathf.Lerp(heuristicScaleLookup[Heuristic.AGGRESSION], (heuristicScaleLookup[Heuristic.AGGRESSION] * 0.25f), h);
        if (newAggression <= 0) newAggression = 0.0f;
        modifiableHeuristicScales[aggressionIndex].scale = newAggression;

        newAdrenaline = Mathf.Lerp(heuristicScaleLookup[Heuristic.ADRENALINE], (heuristicScaleLookup[Heuristic.ADRENALINE] * 0.25f), h);
        if (newAdrenaline <= 0) newAdrenaline = 0.0f;
        modifiableHeuristicScales[adrenalineIndex].scale = newAdrenaline;
        //}
        //else
        //{
        //    modifiableHeuristicScales[cautionIndex].scale = heuristicScaleLookup[Heuristic.CAUTION];
        //    modifiableHeuristicScales[aggressionIndex].scale = heuristicScaleLookup[Heuristic.AGGRESSION];
        //    modifiableHeuristicScales[adrenalineIndex].scale = heuristicScaleLookup[Heuristic.ADRENALINE];
        //}
    }

    //Update the agent's target position.
    private void ComputeNewDestination()
    {
        //Base target = our existing destination.
        TargetDest dest = new TargetDest(currentDest);

        //Clear the list of candidate destinations.
        destList.Clear();

        float maxScore = -10000.0f;

        pastCumulativeEntityScore = cumulativeEntityScore;
        cumulativeEntityScore = 0.0f;

        //Potential entity goals.
        EntityMemory currentGoalMemory = null;

        //Used in the calculation of exploration directions.
        Vector3 XZForward = transform.forward;
        XZForward.y = 0.0f;
        XZForward.Normalize();

        //Optimization: Score current goal first to reduce
        //extra computation, since the current goal receives a score bonus.
        if (currentDest.entity != null)
        {
            currentGoalMemory = memory.GetMemory(currentDest.entity);

            if (null == currentGoalMemory)
            {
                NPDebug.LogError("Something went wrong! Targeting " +
                    currentDest.entity.entityRef.objectRef.name +
                    " but it could not be found in agent memory!",
                    typeof(PathOSAgent));
            }
            else
                ScoreEntity(currentGoalMemory, ref maxScore);
        }
        else
        {
            Vector3 goalForward = currentDest.pos - GetPosition();
            goalForward.y = 0.0f;

            if (goalForward.sqrMagnitude > 0.1f)
            {
                goalForward.Normalize();
                bool goalVisible = Mathf.Abs(Vector3.Angle(XZForward, goalForward)) < (eyes.XFOV() * 0.5f);
                ScoreExploreDirection(GetPosition(), goalForward, goalVisible, ref maxScore,
                    true, currentDest.pos);
            }
        }

        for (int i = 0; i < memory.entities.Count; ++i)
        {
            if (!ReferenceEquals(currentGoalMemory, memory.entities[i]))
                ScoreEntity(memory.entities[i], ref maxScore);
        }

        //Potential directional goals.

        //Memorized paths.
        //Treated as not visible since they are based on the player's "idea" of the space.
        for (int i = 0; i < memory.paths.Count; ++i)
        {
            ScoreExploreDirection(memory.paths[i].originPoint,
                memory.paths[i].direction,
                false, ref maxScore);
        }

        //Only considering the XZ plane.
        float halfX = eyes.XFOV() * 0.5f;
        int steps = (int)(halfX / exploreDegrees);

        ScoreExploreDirection(GetPosition(), XZForward, true, ref maxScore);

        for (int i = 1; i <= steps; ++i)
        {
            ScoreExploreDirection(GetPosition(), Quaternion.AngleAxis(i * exploreDegrees, Vector3.up) * XZForward,
                true, ref maxScore);
            ScoreExploreDirection(GetPosition(), Quaternion.AngleAxis(i * -exploreDegrees, Vector3.up) * XZForward,
                true, ref maxScore);
        }

        //Behind the agent (from memory).
        Vector3 XZBack = -XZForward;

        ScoreExploreDirection(GetPosition(), XZBack, false, ref maxScore);
        halfX = (360.0f - eyes.XFOV()) * 0.5f;
        steps = (int)(halfX / invisibleExploreDegrees);

        for (int i = 1; i <= steps; ++i)
        {
            ScoreExploreDirection(GetPosition(), Quaternion.AngleAxis(i * invisibleExploreDegrees, Vector3.up) * XZBack,
                false, ref maxScore);
            ScoreExploreDirection(GetPosition(), Quaternion.AngleAxis(i * -invisibleExploreDegrees, Vector3.up) * XZBack,
                false, ref maxScore);
        }

        //If no destinations are added to the list,
        //the old target will be used.
        if (destList.Count != 0)
            dest = PathOS.ScoringUtility.PickTarget(destList, maxScore);

        //Only recompute goal routing if our new goal is different
        //from the previous goal.
        if (currentDest.entity != dest.entity ||
            Vector3.SqrMagnitude(currentDest.pos - dest.pos)
            > PathOS.Constants.Navigation.GOAL_EPSILON_SQR)
        {
            ++changeTargetCount;

            currentDest = dest;

            float memChanceRoll = Random.Range(0.0f, 1.0f);
            onMemPath = false;

            if (memChanceRoll <= memPathChance)
                onMemPath = memory.memoryMap.NavigateAStar(GetPosition(), currentDest.pos, ref memPathWaypoints);

            if (onMemPath)
            {
                navAgent.SetDestination(memPathWaypoints[0]);
                pathResolved = false;
                memWaypoint.x = memPathWaypoints[0].x;
                memWaypoint.z = memPathWaypoints[0].z;
            }
            else
                RouteDestination();

            //Once something has been selected as a destination,
            //commit it to long-term memory.
            if (null != currentDest.entity)
                memory.CommitLTM(currentDest.entity);
        }

        assessedGoalsInit = true;

        if (verboseDebugging)
            NPDebug.LogMessage("Position: " + navAgent.transform.position +
                ", Destination: " + currentDest);
    }

    public PerceivedEntity GetDestinationEntity()
    {
        return currentDest.entity;
    }

    //maxScore is updated if the entity achieves a higher score.
    private void ScoreEntity(EntityMemory memory, ref float maxScore)
    {
        //A previously visited entity shouldn't be targeted.
        //Likewise, an entity found to be unreachable shouldn't be targeted.
        if (memory.visited || memory.unreachable)
            return;

        bool isFinalGoal = memory.entity.entityType == EntityType.ET_GOAL_COMPLETION;

        float bias = 0.0f;

        //Special circumstances for the final goal - since it marks the end of play
        //for a player.
        if (isFinalGoal)
        {
            //If mandatory goals remain, the final goal can't be targeted.
            if (this.memory.MandatoryGoalsLeft() || !assessedGoalsInit)
                return;

            bias += Mathf.Lerp(PathOS.Constants.Behaviour.FINAL_GOAL_BONUS_MIN,
                PathOS.Constants.Behaviour.FINAL_GOAL_BONUS_MAX,
                heuristicScaleLookup[Heuristic.EFFICIENCY]);

            //Penalize for the agent's assessment of benefit for all unvisited
            //positive entities.
            bias -= pastCumulativeEntityScore;
        }

        Vector3 toEntity = memory.RecallPos() - GetPosition();

        float distFactor = (toEntity.sqrMagnitude < PathOS.Constants.Behaviour.DIST_SCORE_FACTOR_SQR) ?
            1.0f : PathOS.Constants.Behaviour.DIST_SCORE_FACTOR_SQR / toEntity.sqrMagnitude;

        //Weighted scoring function.
        //Bias added to account for entity's type.
        float entityBias = 0.0f;

        foreach (HeuristicScale heuristicScale in modifiableHeuristicScales)
        {
            (Heuristic, EntityType) key = (heuristicScale.heuristic, memory.entity.entityType);

            if (!entityScoringLookup.ContainsKey(key))
            {
                NPDebug.LogError("Couldn't find key " + key.ToString() + " in heuristic scoring lookup!", typeof(PathOSAgent));
                continue;
            }

            entityBias += heuristicScale.scale * entityScoringLookup[key]
                * distFactor;
        }

        bias += entityBias;

        float score = ScoreDirection(GetPosition(), toEntity, bias, toEntity.magnitude);

        //Bias for preferring interactive objects (if they are favourable).
        if (entityBias > 0.0f && score > 0.0f)
            score += PathOS.Constants.Behaviour.INTERACTIVITY_BIAS;

        if (!isFinalGoal && score > 0.0f)
            cumulativeEntityScore += score;

        //Bias for preferring the goal we have already set.
        //(If we haven't already reached it).
        if (memory.entity == currentDest.entity
            && Vector3.SqrMagnitude(GetPosition() - currentDest.pos)
            > PathOS.Constants.Navigation.GOAL_EPSILON_SQR)
            score += PathOS.Constants.Behaviour.EXISTING_GOAL_BIAS;

        //Check if the destination should be added to the candidate list.
        if (score > maxScore
            || (maxScore - score)
            < PathOS.Constants.Behaviour.SCORE_UNCERTAINTY_THRESHOLD)
        {
            TargetDest newDest = new TargetDest();
            newDest.score = score;

            //We only need to update the destination position
            //if we're targeting an entity other than the current target.
            if (memory.entity == currentDest.entity)
            {
                newDest.pos = currentDest.pos;
                newDest.accurate = currentDest.accurate;
            }
            else
            {
                //Check for reachability.
                Vector3 realPos = Vector3.zero;

                // GABO TODO DEBUG: reachability
                //bool reachable = PathOSNavUtility.GetClosestPointWalkable(memory.entity.ActualPosition(),
                //    navAgent.height * PathOS.Constants.Navigation.NAV_SEARCH_RADIUS_FAC,
                //    ref realPos);
                bool reachable = PathOSNavUtility.CanAgentReachTarget(
                    navAgent,
                    memory.entity.ActualPosition(),
                    navAgent.height * PathOS.Constants.Navigation.NAV_SEARCH_RADIUS_FAC,
                    ref realPos);
                // FIN DEBUG

                if (reachable)
                    reachable = Vector3.SqrMagnitude(
                        PathOSNavUtility.XZPos(realPos) -
                        PathOSNavUtility.XZPos(memory.entity.ActualPosition()))
                        < visitThresholdSqr;

                if (!reachable)
                {
                    memory.MakeUnreachable();
                    return;
                }

                //If the entity is visible/always known to the player, ensure 
                //its position is set to the actual position of the entity.
                if (memory.entity.visible || memory.entity.entityRef.alwaysKnown)
                {
                    newDest.pos = realPos;
                    newDest.accurate = true;
                }
                //Otherwise, fetch its position from memory.
                //(Imperfect recall, done when the decision is made).
                else
                {
                    Vector3 guessPos = Vector3.zero;

                    // GABO TODO DEBUG
                    //reachable = PathOSNavUtility.GetClosestPointWalkable(
                    //memory.RecallPos(),
                    //navAgent.height * PathOS.Constants.Navigation.NAV_SEARCH_RADIUS_FAC,
                    //ref guessPos);
                    reachable = PathOSNavUtility.CanAgentReachTarget(
                        navAgent,
                        memory.RecallPos(),
                        navAgent.height * PathOS.Constants.Navigation.NAV_SEARCH_RADIUS_FAC,
                        ref guessPos);
                    // FIN DEBUG

                    newDest.pos = (reachable) ? guessPos : realPos;
                    newDest.accurate = !reachable;
                }
            }

            //Only update maxScore if the new score is actually higher.
            //(Prevent over-accumulation of error.)
            //This will only execute if the destination is reachable.
            if (score > maxScore)
                maxScore = score;

            newDest.entity = memory.entity;
            destList.Add(newDest);
        }
    }

    //maxScore is updated if the direction achieves a higher score.
    void ScoreExploreDirection(Vector3 origin, Vector3 dir, bool visible, ref float maxScore,
        bool overridePos = false, Vector3 overrideDest = default)
    {
        float distance = 0.0f;
        Vector3 newTarget = origin;

        if (overridePos && overrideDest != null)
        {
            newTarget = overrideDest;
        }
        else
        {
            if (visible)
            {
                //Grab the "extent" of the direction on the navmesh from the perceptual system.
                NavMeshHit hit = eyes.ExploreVisibilityCheck(GetPosition(), dir);
                distance = hit.distance;
                newTarget = hit.position;
            }
            else
            {
                //Grab the "extent" of the direction on our memory model of the navmesh.
                PathOSNavUtility.NavmeshMemoryMapper.NavmeshMemoryMapperCastHit hit;
                memory.memoryMap.RaycastMemoryMap(origin, dir, eyes.navmeshCastDistance, out hit);
                distance = hit.distance;

                // GABO TODO DEBUG
                //bool reachable = PathOSNavUtility.GetClosestPointWalkable(
                //    origin + distance * dir, exploreTargetMargin, ref newTarget);
                bool reachable = PathOSNavUtility.CanAgentReachTarget(
                    navAgent,
                    origin + distance * dir,
                    exploreTargetMargin,
                    ref newTarget);
                // FIN DEBUG

                //Disqualify a target if the agent has determined it to be unreachable.
                if (!reachable || IsUnreachable(newTarget))
                    return;
            }
        }

        float bias = 0.0f;

        //Bias for preferring the goal we have already set.
        //(If we haven't reached it already.)
        if (Vector3.SqrMagnitude(newTarget - currentDest.pos) < PathOS.Constants.Navigation.GOAL_EPSILON_SQR
            && (GetPosition() - currentDest.pos).magnitude > exploreThreshold)
        {
            bias += PathOS.Constants.Behaviour.EXISTING_GOAL_BIAS;
        }

        float score = ScoreDirection(origin, dir, bias, distance);

        //Same inclusion logic as for entity goals.
        if (score > maxScore
            || (maxScore - score)
            < PathOS.Constants.Behaviour.SCORE_UNCERTAINTY_THRESHOLD)
        {
            if (score > maxScore)
                maxScore = score;

            TargetDest newDest = new TargetDest();
            newDest.score = score;

            //If we're originating from where we stand, target the "end" point.
            //Else, target the "start" point, and the agent will re-assess its 
            //options when it gets there.
            if (Vector3.SqrMagnitude(origin - GetPosition())
                < PathOS.Constants.Navigation.EXPLORE_PATH_POS_THRESHOLD_FAC
                * exploreThreshold)
                newDest.pos = newTarget;
            else
                newDest.pos = origin;

            newDest.accurate = true;
            newDest.entity = null;

            destList.Add(newDest);
        }

        memory.AddPath(new ExploreMemory(origin, dir, newTarget, score));
    }

    float ScoreDirection(Vector3 origin, Vector3 dir, float bias, float maxDistance)
    {
        dir.Normalize();

        //Score base = bias.
        float score = bias;

        //Add to the score based on our curiosity and the potential to 
        //"fill in our map" as we move in this direction.
        //This is similar to the scaling created by assessing an exploration direction.
        PathOSNavUtility.NavmeshMemoryMapper.NavmeshMemoryMapperCastHit hit;
        memory.memoryMap.RaycastMemoryMap(origin, dir, maxDistance, out hit);

        score += (heuristicScaleLookup[Heuristic.CURIOSITY])
            * hit.numUnexplored / PathOSNavUtility.NavmeshMemoryMapper.maxCastSamples
            * hit.distance / eyes.navmeshCastDistance;

        //Enumerate over all entities the agent knows about, and use them
        //to affect our assessment of the potential target.
        for (int i = 0; i < memory.entities.Count; ++i)
        {
            if (memory.entities[i].visited || memory.entities[i].unreachable)
                continue;

            //Vector to the entity.
            Vector3 entityVec = memory.entities[i].RecallPos() - origin;

            //Scale our factor by inverse square of distance.
            float distFactor = (entityVec.sqrMagnitude < PathOS.Constants.Behaviour.DIST_SCORE_FACTOR_SQR) ?
            1.0f : PathOS.Constants.Behaviour.DIST_SCORE_FACTOR_SQR / entityVec.sqrMagnitude;

            Vector3 dir2entity = entityVec.normalized;

            float dot = Vector3.Dot(dir, dir2entity);
            dot = Mathf.Clamp(dot, 0.0f, 1.0f);

            //Weighted scoring function.
            foreach (HeuristicScale heuristicScale in modifiableHeuristicScales)
            {
                (Heuristic, EntityType) key = (heuristicScale.heuristic,
                    memory.entities[i].entity.entityType);

                if (!entityScoringLookup.ContainsKey(key))
                {
                    NPDebug.LogError("Couldn't find key " + key.ToString() + " in heuristic scoring lookup!", typeof(PathOSAgent));
                    continue;
                }

                bias += heuristicScale.scale * entityScoringLookup[key] * dot * distFactor;
            }
        }

        return score;
    }

    private void Update()
    {
        //Inactive state toggle for debugging purposes (or if the agent is finished).
        if (freezeAgent || completed)
            return;

        if (timeScale <= 0.0f) timeScale = 1.0f;

        // GABO: Ignoring this line for temporary batch agents, since you're not supposed
        // to control their timeScale in the inspector or when batching ends, while also
        // allowing use of PathOSBatchingWindow's time scale slider which doesn't work
        // properly when this line is set since entering Game Mode calls this object
        // default timeScale for some reason.
        if (!name.Contains("Temporary Batch Agent"))
        {
            Time.timeScale = timeScale;
        }

        if (health <= 0 && !dead) dead = true;

        //If we've reached our destination, reset the number of times
        //we've "changed our mind" without doing anything.
        if (changeTargetCount > 0
            && (Vector3.SqrMagnitude(GetPosition() - currentDest.pos) < PathOS.Constants.Navigation.GOAL_EPSILON_SQR
                || (currentDest.entity != null && memory.Visited(currentDest.entity))))
        {
            changeTargetCount = 0;

            if (currentDest.entity != null)
            {
                CalculateHealth(currentDest.entity.entityType);
            }
        }

        //Update spatial memory.
        memory.memoryMap.Fill(navAgent.transform.position);

        //Update of periodic actions.
        routeTimer += Time.deltaTime;
        perceptionTimer += Time.deltaTime;

        if (!lookingAround)
            lookTimer += Time.deltaTime;

        //Rerouting update.
        if (routeTimer >= RouteComputeTimeCalculated())
        {
            routeTimer = 0.0f;

            float rerouteChance = changeTargetCount
                * PathOS.Constants.Behaviour.GOAL_INDECISION_CHANCE;

            float rerouteRoll = Random.Range(0.0f, 1.0f);

            if (rerouteRoll >= rerouteChance)
                ComputeNewDestination();
        }

        //Memory path update.
        if (onMemPath)
        {
            Vector3 curXZ = GetPosition();
            curXZ.y = 0.0f;

            if (Vector3.SqrMagnitude(curXZ - memWaypoint)
                < PathOS.Constants.Navigation.WAYPOINT_EPSILON_SQR)
            {
                memPathWaypoints.RemoveAt(0);

                if (memPathWaypoints.Count == 0)
                {
                    onMemPath = false;
                    RouteDestination();
                }
                else
                {
                    navAgent.SetDestination(memPathWaypoints[0]);
                    pathResolved = false;
                    memWaypoint.x = memPathWaypoints[0].x;
                    memWaypoint.z = memPathWaypoints[0].z;
                }
            }
        }
        else if (currentDest.entity != null && !currentDest.accurate
            && currentDest.entity.visible)
            MakeEntityDestinationAccurate();

        //Targeting update. This prevents the agent from getting stuck.
        if (!pathResolved && NavmeshPathIncomplete())
        {
            //If we're following a memory path,
            //abort and route to the final target on the Navmesh.
            if (onMemPath)
            {
                onMemPath = false;
                RouteDestination();
            }
            //If we're dealing with an entity...
            else if (currentDest.entity != null)
            {
                PerceivedEntity entity = currentDest.entity;

                if (!currentDest.accurate)
                    MakeEntityDestinationAccurate();

                else
                {
                    float adjVisitSqr = (entity.entityRef.overrideVisitRadius) ?
                        entity.entityRef.visitRadiusSqr : visitThresholdSqr;

                    //Compress unreachability check to XZ plane.
                    Vector3 agentPos = GetPosition();
                    agentPos.y = 0.0f;

                    Vector3 targetPos = entity.perceivedPos;
                    targetPos.y = 0.0f;

                    if (Vector3.SqrMagnitude(agentPos - targetPos)
                        >= adjVisitSqr)
                        memory.MakeUnreachable(entity);

                    //Reset the number of times we've changed our mind
                    //without doing anything (since we tried to get here).
                    changeTargetCount = 0;
                }
            }
            //If we're dealing with an exploration target...
            else
            {
                //This will prevent the agent from retargeting the current destination.
                AddUnreachable(currentDest.pos);
                changeTargetCount = 0;
            }

            pathResolved = true;
        }

        //Perception update.
        //This will allow the agent's eyes to "process" nearby entities
        //and also update the time threshold for looking around based 
        //on nearby hazards.
        if (perceptionTimer >= PathOS.Constants.Perception.PERCEPTION_COMPUTE_TIME)
        {
            perceptionTimer = 0.0f;
            PerceptionUpdate();
        }

        //Look-around update.
        if (lookTimer >= lookTime)
        {
            lookTimer = 0.0f;
            lookingAround = true;
            StartCoroutine(LookAround());
        }

        //Set the agent's completion flag.
        if (manager.endOnCompletionGoal
            && memory.FinalGoalCompleted())
        {
            completed = true;
            gameObject.SetActive(false);
        }

        //Camera follow update
        if (cameraFollow)
        {
            if (cameraObject != null) cameraObject.transform.position = new Vector3(transform.position.x, 15.0f, transform.position.z);
        }
    }

    private void RouteDestination()
    {
        navAgent.SetDestination(currentDest.pos);
        pathResolved = false;
    }
    private void ResetDestinationSelf()
    {
        currentDest.pos = GetPosition();
        currentDest.entity = null;
        currentDest.accurate = true;
    }

    private void MakeEntityDestinationAccurate()
    {
        // GABO TODO DEBUG: Reachability
        //bool reachable = PathOSNavUtility.GetClosestPointWalkable(
        //            currentDest.entity.ActualPosition(),
        //            navAgent.height * PathOS.Constants.Navigation.NAV_SEARCH_RADIUS_FAC,
        //            ref currentDest.pos);
        bool reachable = PathOSNavUtility.CanAgentReachTarget(
                navAgent,
                currentDest.entity.ActualPosition(),
                navAgent.height * PathOS.Constants.Navigation.NAV_SEARCH_RADIUS_FAC,
                ref currentDest.pos);
        // FIN DEBUG

        if (reachable)
            reachable = Vector3.SqrMagnitude(
                PathOSNavUtility.XZPos(currentDest.pos) -
                PathOSNavUtility.XZPos(currentDest.entity.ActualPosition()))
                < visitThresholdSqr;

        if (!reachable)
        {
            memory.MakeUnreachable(currentDest.entity);
            ResetDestinationSelf();
        }

        currentDest.accurate = true;
        RouteDestination();
    }

    private void AddUnreachable(Vector3 target)
    {
        for (int i = 0; i < unreachableReference.Count; ++i)
        {
            if (Vector3.SqrMagnitude(target - unreachableReference[i])
                < PathOS.Constants.Navigation.UNREACHABLE_POS_SIMILARITY_SQR)
                return;
        }

        unreachableReference.Add(target);
    }

    public bool IsUnreachable(Vector3 target)
    {
        for (int i = 0; i < unreachableReference.Count; ++i)
        {
            if (Vector3.SqrMagnitude(target - unreachableReference[i])
                < PathOS.Constants.Navigation.UNREACHABLE_POS_CHECK_SQR)
                return true;
        }

        return false;
    }

    private void PerceptionUpdate()
    {
        UpdateLookTime();
    }

    private bool NavmeshPathIncomplete()
    {
        return !navAgent.pathPending && !navAgent.hasPath
            && navAgent.pathStatus == NavMeshPathStatus.PathPartial
            && !navAgent.isPathStale;
    }

    //Inelegant and brute-force "animation" of the agent to look around.
    //In the future this should add non-determinism and preferably be abstracted somewhere else.
    //And cleaned up. Probably.
    IEnumerator LookAround()
    {
        navAgent.isStopped = true;
        navAgent.updateRotation = false;

        //Simple 90-degree sweep centred on current heading.
        Quaternion home = transform.rotation;
        Quaternion right = Quaternion.AngleAxis(lookDegrees, Vector3.up) * home;
        Quaternion left = Quaternion.AngleAxis(-lookDegrees, Vector3.up) * home;

        float lookingTime = 0.5f;
        float lookingTimer = 0.0f;

        while (lookingTimer < lookingTime)
        {
            transform.rotation = Quaternion.Slerp(home, right, lookingTimer / lookingTime);
            lookingTimer += Time.deltaTime;
            yield return null;
        }

        lookingTimer = 0.0f;

        while (lookingTimer < lookingTime)
        {
            lookingTimer += Time.deltaTime;
            yield return null;
        }

        lookingTimer = 0.0f;

        while (lookingTimer < lookingTime)
        {
            transform.rotation = Quaternion.Slerp(right, left, lookingTimer / lookingTime);
            lookingTimer += Time.deltaTime;
            yield return null;
        }

        lookingTimer = 0.0f;

        while (lookingTimer < lookingTime)
        {
            lookingTimer += Time.deltaTime;
            yield return null;
        }

        lookingTimer = 0.0f;

        while (lookingTimer < lookingTime)
        {
            transform.rotation = Quaternion.Slerp(left, home, lookingTimer / lookingTime);
            lookingTimer += Time.deltaTime;
            yield return null;
        }

        lookingTimer = 0.0f;
        lookingAround = false;
        navAgent.updateRotation = true;
        navAgent.isStopped = false;
    }

    //todo: clean this up
    public void TakeHazardDamage()
    {
        health -= GetEnemyDamage(hazardDamage.min, hazardDamage.max);
    }

    //Computes the player health when interacting with enemies or resources
    //Needs to be improved/edited
    private void CalculateHealth(EntityType entityType)
    {
        switch (entityType)
        {
            case EntityType.ET_HAZARD_ENEMY_LOW:
                health -= GetEnemyDamage(lowEnemyDamage.min, lowEnemyDamage.max);
                break;
            case EntityType.ET_HAZARD_ENEMY_MED:
                health -= GetEnemyDamage(medEnemyDamage.min, medEnemyDamage.max);
                break;
            case EntityType.ET_HAZARD_ENEMY_HIGH:
                health -= GetEnemyDamage(highEnemyDamage.min, highEnemyDamage.max);
                break;
            case EntityType.ET_HAZARD_ENEMY_BOSS:
                health -= GetEnemyDamage(bossEnemyDamage.min, bossEnemyDamage.max);
                break;
            case EntityType.ET_HAZARD_ENVIRONMENT:
                health -= GetEnemyDamage(hazardDamage.min, hazardDamage.max);
                break;
            case EntityType.ET_RESOURCE_PRESERVATION_LOW:
                health += GetHealthGain(lowHealthGain.min, lowHealthGain.max);
                break;
            case EntityType.ET_RESOURCE_PRESERVATION_MED:
                health += GetHealthGain(medHealthGain.min, medHealthGain.max);
                break;
            case EntityType.ET_RESOURCE_PRESERVATION_HIGH:
                health += GetHealthGain(highHealthGain.min, highHealthGain.max);
                break;
            default:
                break;
        }

        //Making sure the health values don't get messed up
        if (health < 0) health = 0;
        else if (health > 100) health = 100;

        //Updates weights based on the player's health
        UpdateWeightsBasedOnHealth();
    }

    private float GetHealthGain(float min, float max)
    {
        return Random.Range(min, max);
    }

    //Get damage values
    private float GetEnemyDamage(float min, float max)
    {
        float experienceAdjustment = 1.0f - experienceScale;
        experienceAdjustment = experienceAdjustment <= 0 ? 0.1f : experienceAdjustment;

        return Random.Range(
            min * experienceAdjustment,
            max * experienceAdjustment);
    }
    public Vector3 GetTargetPosition()
    {
        return currentDest.pos;
    }

    public bool IsTargeted(PerceivedEntity entity)
    {
        return currentDest.entity == entity;
    }

    public float GetHealth()
    {
        return health;
    }

    public bool IsDead()
    {
        return dead;
    }

    // GABO: Set all unreachable positions (memory entities not included) as possibly reachable again
    public void ResetUnreachablePositionReferences()
    {
        if (unreachableReference.Count > 0)
        {
            unreachableReference.Clear();
        }
    }
}
