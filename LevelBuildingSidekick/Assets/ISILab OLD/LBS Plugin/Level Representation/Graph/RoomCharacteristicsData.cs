using LBS.Graph;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LBS.Schema
{
    [System.Serializable]
    public class RoomCharacteristicsData : LBSNodeData
    {
        #region InspectorDrawer
        private class NodeScriptable : GenericScriptable<LBSNodeData> { };
        [CustomEditor(typeof(GenericScriptable<LBSNodeData>))]
        [CanEditMultipleObjects]
        private class NodeScriptableEditor : GenericScriptableEditor { };
        #endregion
        // lista de strings que represente a los RoomElementBundle
        [HideInInspector, JsonIgnore]
        public static readonly string[] prefabCategories = { "Walls", "Floors", "Doors" };

        [SerializeField, JsonRequired]
        private IntRange rangeWidth = new IntRange(1, 1);

        [SerializeField, JsonRequired]
        private IntRange rangeHeight = new IntRange(1, 1);

        [SerializeField, JsonRequired]
        private AspectRatioValue aspectRatio = new AspectRatioValue(1, 1);

        [SerializeField, JsonRequired]
        [Tooltip("Choose size to adjust the width and height of the rooms.")]
        private ProportionType proportionType = ProportionType.SIZE;

        [SerializeField, JsonIgnore]
        private List<ItemCategory> prefabs = new List<ItemCategory>(); // esto para que (??)

        [SerializeField, JsonRequired]
        [ScriptableToString(typeof(RoomElementBundle))]
        [Tooltip("Assign a list of prefabs as a bundle to a node.")]
        public List<string> bundlesNames = new List<string>();

        [SerializeField, JsonRequired]
        private List<string> tags = new List<string>();

        [JsonIgnore]        
        public ProportionType ProportionType => proportionType;

        [JsonIgnore]
        public IntRange RangeWidth => rangeWidth;

        [JsonIgnore]
        public IntRange RangeHeight => rangeHeight;

        [JsonIgnore]
        public AspectRatioValue AspectRatio => aspectRatio;

        public RoomCharacteristicsData(string label, Vector2 position, int radius) : base(label, position)
        {
            bundlesNames.Add(Utility.DirectoryTools.GetScriptables<RoomElementBundle>()[0].name);
            proportionType = ProportionType.SIZE;
        }

        [System.Serializable]
        public struct IntRange
        {
            //[Range(1,16)]
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


        // test
        [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
        public class MinMaxAttribute : PropertyAttribute
        {
            public int min, max;

            public MinMaxAttribute(int min, int max)
            {
                this.min = min;
                this.max = max;
            }
        }


        [CustomPropertyDrawer(typeof(MinMaxAttribute))]
        public sealed class MinMaxDrawer : PropertyDrawer
        {
           // var rangeAttribute = (RangeExAttribute)base.attribute;
        }

    }
}
