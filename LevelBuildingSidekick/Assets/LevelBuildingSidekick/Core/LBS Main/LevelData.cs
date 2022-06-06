using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "LevelBuildingSidekick/Data/Level Data")]
    public class LevelData : ScriptableObject
    {
        public HashSet<string> tags;
        public HashSet<GameObject> floorTiles;
        public HashSet<GameObject> wallTiles;
        public HashSet<GameObject> doorTiles;
        public Vector2Int levelSize;
    }
}

