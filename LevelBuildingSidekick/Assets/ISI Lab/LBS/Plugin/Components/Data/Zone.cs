using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISILab.LBS.Internal;
using LBS.Bundles;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace ISILab.LBS.Components
{

    [System.Serializable]
    public class Zone : ICloneable
    {
        #region FIELDS
        [SerializeField, JsonRequired]
        protected string id = "Zone";
        [SerializeField, JsonRequired]
        protected Color color;
        [SerializeField, JsonRequired]
        protected float borderThickness;
        [SerializeField, JsonRequired]
        protected Vector2 pivot;
        [SerializeField, JsonRequired]
        protected List<Vector2Int> positions = new List<Vector2Int>();
        //[ScriptableObjectReference(typeof(LBSIdentifier), "Interior Styles")]
        [SerializeField, JsonRequired]
        private List<string> insideStyles = new List<string>();

        [SerializeField, JsonRequired]
        private List<string> outsideStyles = new List<string>();
        
        #endregion

        #region PROPERTIES

        [JsonIgnore]
        public string ID
        {
            get => id;
            set => id = value;
        }

        [JsonIgnore]
        public Color Color
        {
            get => color;
            set => color = value;
        }

        [JsonIgnore]
        public float BorderThickness
        {
            get => borderThickness;
            set => borderThickness = value;
        }
        
        [JsonIgnore]
        public Vector2 Pivot
        {
            get => pivot;
            set => pivot = value;
        }
        
        [JsonIgnore]
        public List<string> InsideStyles
        {
            get => insideStyles;
            set => insideStyles = value;
        }

        [JsonIgnore]
        public List<string> OutsideStyles
        {
            get => outsideStyles;
            set => outsideStyles = value;
        }
        #endregion

        #region CONSTRUCTORS

        public Zone() { }

        public Zone(string id, Color color)
        {
            this.id = id;
            this.color = color;
        }

        #endregion

        #region METHODS
        
        public void AddPosition(Vector2Int tilePosition)
        {
            positions.Add(tilePosition); 
        }

        public Vector2Int GetSize()
        {
            if (positions.Count == 0)
                return Vector2Int.zero;

            int minX = int.MaxValue, minY = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue;

            foreach (var pos in positions)
            {
                if (pos.x < minX) minX = pos.x;
                if (pos.y < minY) minY = pos.y;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.y > maxY) maxY = pos.y;
            }

            return new Vector2Int(maxX - minX + 1, maxY - minY + 1);
        }
        public object Clone()
        {
            var clone = new Zone(this.id, this.color);
            clone.pivot = new Vector2(this.pivot.x, this.pivot.y);
            clone.insideStyles = new List<string>(this.insideStyles);
            clone.outsideStyles = new List<string>(this.outsideStyles);
            return clone;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Zone;

            if (other == null) return false;

            if (!id.Equals(other.id)) return false;

            if (!color.Equals(other.color)) return false;

            //if(!pivot.Equals(other.pivot)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion


    }

    public static class ZoneExtension
    {
        public static List<Bundle> GetInsideBundles(this Zone zone)
        {
            var bundles = new List<Bundle>();
            var allBundles = LBSAssetsStorage.Instance.Get<Bundle>().ToList();
            foreach (var tags in zone.InsideStyles)
            {
                bundles.Add(allBundles.Find(b => b.name.Equals(tags)));
            }
            return bundles;
        }

        public static List<Bundle> GetOutsideBundles(this Zone zone)
        {
            var bundles = new List<Bundle>();
            var allBundles = LBSAssetsStorage.Instance.Get<Bundle>().ToList();
            foreach (var tags in zone.OutsideStyles)
            {
                bundles.Add(allBundles.Find(b => b.name.Equals(tags)));
            }
            return bundles;
        }
    }
}