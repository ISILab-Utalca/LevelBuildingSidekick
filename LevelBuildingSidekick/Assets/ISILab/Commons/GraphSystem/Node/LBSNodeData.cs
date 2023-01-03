using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using LBS;
using System;
using LBS.Schema;
using Newtonsoft.Json;
using UnityEditor;

namespace LBS.Graph
{
    [System.Serializable]
    public abstract class LBSNodeData
    {
        #region FIELDS
        [HideInInspector, JsonRequired]
        private int x, y;

        [HideInInspector, JsonRequired]
        private float width, height;

        [SerializeField, JsonRequired]
        private string label = ""; // "ID" or "name"
        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public Vector2 Centroid => (Position + (new Vector2(width, height)/2));

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
        public float Width
        {
            get => width;
            set => width = value;
        }

        [JsonIgnore]
        public float Height
        {
            get => height;
            set => height = value;
        }

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
        #endregion

        [HideInInspector, JsonIgnore]
        internal Action<LBSNodeData> OnChange; // explicarle esto al gabo pa ver que opina (!!!)


        /// <summary>
        /// Empty constructor, necessary for serialization with json.
        /// </summary>
        public LBSNodeData() 
        {
            label = "Undefined";
            width = 1;
            height = 1;
            x = 0;
            y = 0;
        }

        public LBSNodeData(string label, Vector2 position)
        {
            this.label = label;
            x = (int)position.x;
            y = (int)position.y;
            width = 1;
            height = 1;
        }
    }
}

public enum ProportionType
{
    SIZE,
    RATIO
}

