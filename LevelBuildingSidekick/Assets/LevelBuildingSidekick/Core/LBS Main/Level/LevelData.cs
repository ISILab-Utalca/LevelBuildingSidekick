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

        public List<Data> steps = new List<Data>();

        public override Type ControllerType => typeof(LevelController);
    }
}

