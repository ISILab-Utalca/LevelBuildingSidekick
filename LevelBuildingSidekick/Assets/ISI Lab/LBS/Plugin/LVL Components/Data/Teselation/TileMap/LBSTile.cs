using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Components.TileMap
{
    [System.Serializable]
    public class LBSTile
    {
        [SerializeField, JsonRequired, SerializeReference]
        int x, y;

        int sides = 4;

        public Vector2Int Position
        {
            get => new Vector2Int(x,y);
            set { x = value.x; y = value.y; }
        }

        public int Sides => sides;

        public LBSTile() 
        {
            x = 0;
            y = 0;
            sides = 4; 
        }

        public LBSTile(Vector2 position, int sides = 4) 
        {
            x = (int)position.x;
            y = (int)position.y;
            this.sides = sides; 
        }
    }
}

