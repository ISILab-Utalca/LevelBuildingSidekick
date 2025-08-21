﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
PathOSUI.cs 
PathOSUI (c) Nine Penguins (Samantha Stahlke) 2019
*/

namespace PathOS
{
    public class UI
    {
        /* Style Constraints */
        public static float shortLabelWidth = 24.0f;
        public static float mediumLabelWidth = 36.0f;
        public static float longLabelWidth = 200.0f;
        public static float shortFloatfieldWidth = 40.0f;

        public static Color mapUnknown = Color.black;
        public static Color mapSeen = Color.blue;
        public static Color mapVisited = Color.green;
        public static Color mapObstacle = Color.red;

        //Cut off the initial part of a string.
        //(Used for displaying filepaths).
        public static void TruncateStringHead(string longText,
            ref string shortText, int maxLen)
        {
            shortText = longText.Substring(
                        Mathf.Max(0, longText.Length - maxLen));

            if (longText.Length > maxLen)
                shortText = "..." + shortText;
        }

        //Used to truncate floating-point values in input fields.
        public static float RoundFloatfield(float val)
        {
            return Mathf.Round(val * 1000.0f) / 1000.0f;
        }

        public static string GetFormattedTimestamp()
        {
            return System.DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH'-'mm'-'ss");
        }

        public static SortedList<Heuristic, string> heuristicLabels =
            new SortedList<Heuristic, string>()
            {
                { Heuristic.ACHIEVEMENT,    "Achievement" },
                { Heuristic.ADRENALINE,     "Adrenaline" },
                { Heuristic.AGGRESSION,     "Aggression" },
                { Heuristic.CAUTION,        "Caution" },
                { Heuristic.COMPLETION,     "Completion" },
                { Heuristic.CURIOSITY,      "Curiosity" },
                { Heuristic.EFFICIENCY,     "Efficiency" }
            };

        public static Dictionary<string, Heuristic> heuristicLookup =
            new Dictionary<string, Heuristic>()
            {
                { "Achievement",    Heuristic.ACHIEVEMENT },
                { "Adrenaline",     Heuristic.ADRENALINE },
                { "Aggression",     Heuristic.AGGRESSION },
                { "Caution",        Heuristic.CAUTION    },
                { "Completion",     Heuristic.COMPLETION },
                { "Curiosity",      Heuristic.CURIOSITY  },
                { "Efficiency",     Heuristic.EFFICIENCY }
            };

        public static SortedList<EntityType, string> entityLabels =
            new SortedList<EntityType, string>()
            {
                { EntityType.ET_NONE,                   "Null Type" },
                { EntityType.ET_GOAL_OPTIONAL,          "Optional Goal" },
                { EntityType.ET_GOAL_MANDATORY,         "Mandatory Goal" },
                { EntityType.ET_GOAL_COMPLETION,        "Final Goal" },
                { EntityType.ET_RESOURCE_ACHIEVEMENT,   "Collectible" },
                { EntityType.ET_RESOURCE_PRESERVATION_LOW,  "Self-Preservation Low" },
                { EntityType.ET_RESOURCE_PRESERVATION_MED,  "Self-Preservation Medium" },
                { EntityType.ET_RESOURCE_PRESERVATION_HIGH,  "Self-Preservation High" },
                { EntityType.ET_HAZARD_ENEMY_LOW,           "Enemy Low" },
                { EntityType.ET_HAZARD_ENEMY_MED,           "Enemy Medium" },
                { EntityType.ET_HAZARD_ENEMY_HIGH,           "Enemy High" },
                { EntityType.ET_HAZARD_ENEMY_BOSS,           "Boss" },
                { EntityType.ET_HAZARD_ENVIRONMENT,     "Environment Hazard" },
                { EntityType.ET_POI,                    "POI" },
                { EntityType.ET_POI_NPC,                "NPC" }
            };

        public static string[] entityPopupList =
        {
            entityLabels[EntityType.ET_NONE],
            entityLabels[EntityType.ET_GOAL_OPTIONAL],
            entityLabels[EntityType.ET_GOAL_MANDATORY],
            entityLabels[EntityType.ET_GOAL_COMPLETION],
            entityLabels[EntityType.ET_RESOURCE_ACHIEVEMENT],
            entityLabels[EntityType.ET_RESOURCE_PRESERVATION_LOW],
            entityLabels[EntityType.ET_RESOURCE_PRESERVATION_MED],
            entityLabels[EntityType.ET_RESOURCE_PRESERVATION_HIGH],
            entityLabels[EntityType.ET_HAZARD_ENEMY_LOW],
            entityLabels[EntityType.ET_HAZARD_ENEMY_MED],
            entityLabels[EntityType.ET_HAZARD_ENEMY_HIGH],
            entityLabels[EntityType.ET_HAZARD_ENEMY_BOSS],
            entityLabels[EntityType.ET_HAZARD_ENVIRONMENT],
            entityLabels[EntityType.ET_POI],
            entityLabels[EntityType.ET_POI_NPC]
        };

        public static Dictionary<string, EntityType> entityLookup =
            new Dictionary<string, EntityType>()
            {
                { "Null Type",          EntityType.ET_NONE },
                { "Optional Goal",      EntityType.ET_GOAL_OPTIONAL },
                { "Mandatory Goal",     EntityType.ET_GOAL_MANDATORY },
                { "Final Goal",         EntityType.ET_GOAL_COMPLETION },
                { "Collectible",        EntityType.ET_RESOURCE_ACHIEVEMENT },
                { "Self-Preservation Low",  EntityType.ET_RESOURCE_PRESERVATION_LOW },
                { "Self-Preservation Medium",  EntityType.ET_RESOURCE_PRESERVATION_MED },
                { "Self-Preservation High",  EntityType.ET_RESOURCE_PRESERVATION_HIGH },
                { "Enemy Low",              EntityType.ET_HAZARD_ENEMY_LOW },
                { "Enemy Medium",              EntityType.ET_HAZARD_ENEMY_MED },
                { "Enemy High",              EntityType.ET_HAZARD_ENEMY_HIGH },
                { "Boss",               EntityType.ET_HAZARD_ENEMY_BOSS },
                { "Environment Hazard", EntityType.ET_HAZARD_ENVIRONMENT },
                { "POI",                EntityType.ET_POI },
                { "NPC",                EntityType.ET_POI_NPC }
            };
    }
}