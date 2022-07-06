using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "LevelBuildingSidekick/Data/Level Data")]
    public class LevelData : Data
    {
        public HashSet<string> tags = new HashSet<string>();
        public HashSet<string> Tags
        {
            get
            {
                if(tags == null)
                {
                    tags = new HashSet<string>();
                }
                return tags;
            }
        }

        public Dictionary<string, HashSet<GameObject>> levelObjects = new Dictionary<string, HashSet<GameObject>>();

        public Vector2Int levelSize;

        public override Type ControllerType => typeof(LevelController);
    }
}

