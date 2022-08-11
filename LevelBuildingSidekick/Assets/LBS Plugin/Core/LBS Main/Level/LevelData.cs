using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick
{
    [System.Serializable]
    //[CreateAssetMenu(menuName = "LevelBuildingSidekick/Data/Level Data")]
    public class LevelData : Data
    {
        public string levelName;

        public List<string> tags;

        public List<ItemCategory> levelObjects;

        public Vector2Int size;

        public List<LBSRepesentationData> representations = new List<LBSRepesentationData>();

        public override Type ControllerType => throw new NotImplementedException();
        //public override Type ControllerType => typeof(LevelController);

        public List<GameObject> RequestLevelObjects(string category)
        { 
            if(levelObjects.Find((i) => i.category == category) == null)
            {
                levelObjects.Add(new ItemCategory(category));
            }
            return levelObjects.Find((i) => i.category == category).items;
        }

    }
}

