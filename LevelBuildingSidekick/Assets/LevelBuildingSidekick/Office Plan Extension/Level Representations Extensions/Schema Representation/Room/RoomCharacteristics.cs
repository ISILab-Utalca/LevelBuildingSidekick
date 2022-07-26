using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick.Blueprint
{
    [System.Serializable]
    public class RoomCharacteristics
    {
        public string label;
        public Vector2Int widthRange = Vector2Int.one;
        public Vector2Int heightRange = Vector2Int.one;
        public Vector2Int aspectRatio = Vector2Int.one;
        public ProportionType proportionType = ProportionType.RATIO;
        public List<int> IDs;

        public List<ItemCategory> prefabs;

        public string[] prefabCategories = { "Walls", "Floors", "Doors"};

        [NonReorderable]
        public HashSet<string> tags;
    }
}
