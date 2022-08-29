using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using LevelBuildingSidekick;
using System;
using LevelBuildingSidekick.Schema;
using Newtonsoft.Json;
using UnityEditor;

namespace LevelBuildingSidekick.Graph
{
    [System.Serializable]
    public abstract class LBSNodeData : Data
    {
        [HideInInspector, JsonRequired]
        private int x, y;
        [JsonRequired]
        private string label = ""; // "ID" or "name"
        public int radius; // esto deberia ir aqui (??)

        [HideInInspector, JsonIgnore]
        public Action<LBSNodeData> OnChange; // (!!!)

        //public RoomCharacteristicsData room;

        [JsonIgnore]
        public string Label
        {
            get => label;
            set { 
                label = value;
                OnChange?.Invoke(this);
            }
        }

        //[JsonIgnore] 
        //public Texture2D sprite { get => Resources.Load("Textures/Circle") as Texture2D; } // -> static?

        [JsonIgnore]
        public override Type ControllerType => typeof(LBSNodeController);

        //[JsonIgnore]
        //public Rect Rect
        //{
        //    get
        //    {
        //        return new Rect(new Vector2(x - radius, y - radius), Vector2.one * radius * 2);
        //    }
        //}

        [JsonIgnore]
        public Func<string, bool> Exist { get; internal set; } //?

        /// <summary>
        /// Empty constructor, necessary for serialization with json.
        /// </summary>
        public LBSNodeData() { }

        public LBSNodeData(string label, Vector2 position, int radius)
        {
            this.label = label;
            x = (int)position.x;
            y = (int)position.y;
            this.radius = radius;
        }

        [JsonIgnore]
        public Vector2Int Centroid
        {
            get
            {
                return (Position + (Vector2Int.one * Radius));
            }
        }

        [JsonIgnore]
        public Vector2Int Position
        {
            get
            {
                return new Vector2Int(x,y);
            }
            set
            {
                x = value.x;
                y = value.y;
            }
        }

        [JsonIgnore]
        public int Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value;
            }
        }
    }
}

public enum ProportionType
{
    RATIO,
    SIZE
}

