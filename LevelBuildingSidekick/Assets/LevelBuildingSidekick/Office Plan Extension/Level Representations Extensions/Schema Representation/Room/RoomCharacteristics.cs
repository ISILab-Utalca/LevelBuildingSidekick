using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick.Schema
{
    [System.Serializable]
    public class RoomCharacteristics
    {
        public int minWidth = 1;
        public int maxWidth = 1;
        public int minHeight = 1;
        public int maxHeight = 1;
        public int xAspectRatio = 1;
        public int yAspectRatio = 1;
        public ProportionType proportionType = ProportionType.RATIO;
        public List<int> neighbors; // No debería existir aca (!)

        public List<ItemCategory> prefabs;

        public string[] prefabCategories = { "Walls", "Floors", "Doors"};

        [NonReorderable]
        public HashSet<string> tags; // cambiar a una lista (!!!) 
    }
}
