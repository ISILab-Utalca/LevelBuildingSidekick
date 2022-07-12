using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick.Blueprint
{
    [System.Serializable]
    public class RoomCharacteristics
    {
        public string label;
        public Vector2Int width = Vector2Int.one;
        public Vector2Int height = Vector2Int.one;
        public Vector2Int aspectRatio = Vector2Int.one;
        public ProportionType proportionType = ProportionType.RATIO;

        public Dictionary<string, HashSet<GameObject>> prefabs;

        public string[] prefabCategories = { "Walls", "Floors", "Doors"};

        [NonReorderable]
        public HashSet<string> tags;
    }
}
