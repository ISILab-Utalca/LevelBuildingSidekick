using LevelBuildingSidekick.Graph;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick.Schema
{
    [System.Serializable]
    public class RoomCharacteristicsData : LBSNodeData
    {
        [HideInInspector, JsonIgnore]
        public static readonly string[] prefabCategories = { "Walls", "Floors", "Doors" };

        [SerializeField, JsonRequired]
        private IntRange rangeWidth = new IntRange(1, 1);
        [SerializeField, JsonRequired]
        private IntRange rangeHeight = new IntRange(1, 1);
        [SerializeField, JsonRequired]
        private AspectRatioValue aspectRatio = new AspectRatioValue(1, 1);

        [SerializeField, JsonRequired]
        private ProportionType proportionType = ProportionType.RATIO;

        //public List<string> neighbors; // No deberia existir aca (!)

        [SerializeField, JsonIgnore]
        private List<ItemCategory> prefabs; // esto para que (??)

        [SerializeField, JsonRequired]
        private List<string> tags;

        [JsonIgnore]
        public ProportionType ProportionType => proportionType;

        [JsonIgnore]
        public IntRange RangeWidth => rangeWidth;

        [JsonIgnore]
        public IntRange RangeHeight => rangeHeight;

        [JsonIgnore]
        public AspectRatioValue AspectRatio => aspectRatio;

        public RoomCharacteristicsData(string label, Vector2 position, int radius) : base(label, position, radius)
        {

        }

        [System.Serializable]
        public struct IntRange
        {
            [SerializeField,JsonRequired]
            public int min;
            [SerializeField, JsonRequired]
            public int max;

            [JsonIgnore]
            public int Middle => (int)((min + max)/2f);

            public IntRange(int min, int max)
            {
                this.min = min;
                this.max = max;
            }
        }

        [System.Serializable]
        public struct AspectRatioValue // mejorar nombre (?)
        {
            [SerializeField, JsonRequired]
            public int width;
            [SerializeField, JsonRequired]
            public int heigth;

            public AspectRatioValue(int width, int heigth)
            {
                this.width = width;
                this.heigth = heigth;
            }
        }
    }
}
