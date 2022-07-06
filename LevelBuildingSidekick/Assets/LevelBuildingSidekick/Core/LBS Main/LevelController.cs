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
            View = new LevelView(this);
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
            set 
            {
                (Data as LevelData).tags = value;
            }
        }

        public Dictionary<string, HashSet<GameObject>> LevelObjects
        {
            get
            {
                if ((Data as LevelData).levelObjects == null)
                {
                    (Data as LevelData).levelObjects = new Dictionary<string, HashSet<GameObject>>();
                }
                return (Data as LevelData).levelObjects;
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
            //throw new System.NotImplementedException();
        }

        public override void Update()
        {
            //throw new System.NotImplementedException();
        }

        public HashSet<GameObject> RequestLevelObjects(string category)
        {
            if (!LevelObjects.ContainsKey(category))
            {
                LevelObjects.Add(category, new HashSet<GameObject>());
            }
            if (LevelObjects[category] == null)
            {
                LevelObjects[category] = new HashSet<GameObject>();
            }
            return LevelObjects[category];
        }
    }
}


