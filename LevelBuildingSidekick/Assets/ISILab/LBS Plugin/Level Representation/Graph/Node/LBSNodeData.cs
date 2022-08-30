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
        [SerializeField, JsonRequired]
        private string label = ""; // "ID" or "name"

        public int radius; // esto deberia ir aqui (??)

        [HideInInspector, JsonIgnore]
        internal Action<LBSNodeData> OnChange; // explicarle esto al gabo pa ver que opina (!!!)

        [JsonIgnore]
        public string Label
        {
            get => label;
            set 
            { 
                label = value;
                OnChange?.Invoke(this);
            }
        }

        [JsonIgnore]
        public override Type ControllerType => typeof(LBSNodeController);

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
        public Vector2Int Centroid => (Position + (Vector2Int.one * Radius));


        [JsonIgnore]
        public Vector2Int Position
        {
            get => new Vector2Int(x, y);

            set
            {
                x = value.x;
                y = value.y;
            }
        }

        [JsonIgnore]
        public int Radius
        {
            get => radius;
            set => radius = value;
        }
    }
}

public enum ProportionType
{
    RATIO,
    SIZE
}

