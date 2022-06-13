using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using System.Linq;

namespace LevelBuildingSidekick
{
    [System.Serializable]
    public class LevelController : Controller
    {
        public LevelController(Data data) : base(data)
        {
        }

        public HashSet<string> Tags
        {
            get
            {
                if ((Data as LevelData == null))
                {
                    return null;
                }
                return (Data as LevelData).tags;
            }
            set { }
        }
        public HashSet<GameObject> FloorTiles
        {
            get
            {
                if ((Data as LevelData == null))
                {
                    return null;
                }
                return (Data as LevelData).floorTiles;
            }
        }
        public HashSet<GameObject> WallTiles
        {
            get
            {
                if ((Data as LevelData == null))
                {
                    return null;
                }
                return (Data as LevelData).wallTiles;
            }
        }
        public HashSet<GameObject> DoorTiles
        {
            get
            {
                if ((Data as LevelData == null))
                {
                    return null;
                }
                return (Data as LevelData).doorTiles;
            }
        }

        public Vector2Int LevelSize
        {
            get
            {
                if ((Data as LevelData == null))
                {
                    return Vector2Int.zero;
                }
                return (Data as LevelData).levelSize;
            }
            set
            {
                (Data as LevelData).levelSize = value;
            }
        }

        public override void LoadData()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}


