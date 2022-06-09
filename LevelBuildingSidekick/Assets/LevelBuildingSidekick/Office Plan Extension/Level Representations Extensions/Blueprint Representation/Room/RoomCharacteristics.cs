using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick.Blueprint
{
    [System.Serializable]
    public class RoomCharacteristics
    {
        public string label;
        public Vector2Int width;
        public Vector2Int height;
        public Vector2Int aspectRatio;
        public ProportionType proportionType;

        //Should be in children class
        [NonReorderable]
        public List<GameObject> floorTiles;
        //[SerializeField]
        [NonReorderable]
        public List<GameObject> wallTiles;
        //[SerializeField]
        [NonReorderable]
        public List<GameObject> doorTiles;

        public List<string> tags;
    }
}
