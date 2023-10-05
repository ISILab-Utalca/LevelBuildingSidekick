using LBS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components;
using LBS.Generator;
using Newtonsoft.Json;
using System;
using Utility;

namespace LBS.Generator
{
    [System.Serializable]
    public class Generator3D
    {
        [System.Serializable]
        public class Settings // Esto puede estar fuera de generator3D y ser setting de layers en general (!)
        {
            [SerializeField]
            [JsonConverter(typeof(Vector2Converter))]
            public Vector2 scale = new Vector2(2, 2);

            [SerializeField]
            [JsonConverter(typeof(Vector2Converter))]
            public Vector2 resize = new Vector2(0, 0);

            [SerializeField]
            [JsonConverter(typeof(Vector3Converter))]
            public Vector3 position = new Vector3(0, 0, 0);

            [SerializeField]
            public string name = "DEFAULT";

            public override bool Equals(object obj)
            {
                var other = obj as Generator3D.Settings;

                // check if other have the same type
                if (other is not Generator3D.Settings) return false;

                // cheack if scale is the same
                if (!this.scale.Equals(other.scale)) return false;

                // cheack if resize is the same
                if (!this.resize.Equals(other.resize)) return false;

                // cheack if position is the same
                if (!this.position.Equals(other.position)) return false;

                // cheack if name is the same
                if (this.name != other.name) return false;

                return true;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override string ToString()
            {
                return base.ToString();
            }
        }

        #region FIELDS
        [SerializeField]
        public Settings settings;

        [JsonRequired, SerializeReference]
        private List<LBSGeneratorRule> rules = new List<LBSGeneratorRule>();
        #endregion

        #region PROPERTIES
        /*
        public Vector2 Resize
        {
            get => resize;
            set => resize = value;
        }

        public Vector2 Scale
        {
            get => scale;
            set => scale = value;
        }

        public Vector3 Position
        {
            get => position;
            set => position = value;
        }

        public string ObjName
        {
            get => objName;
            set => objName = value;
        }
        */
        #endregion

        #region METHODS
        public void AddRule(LBSGeneratorRule rule)
        {
            rules.Add(rule);
            rule.generator3D = this;
        }

        public void RemoveRule(LBSGeneratorRule rule)
        {
            if (rules.Remove(rule))
                rule.generator3D = null;
        }

        public bool CheckIfIsPosible(LBSLayer layer)
        {
            foreach (var rule in rules)
            {
                var msg = "";
                var msgs = rule.CheckViability(layer); 
                if (msgs.Count > 0)
                {
                    Debug.Log(msg);
                    return false;
                }
            }
            return true;
        }

        public GameObject Generate(LBSLayer layer,List<LBSGeneratorRule> rules, Settings settings)
        {
            var name = settings.name;
            var parent = new GameObject(name);
            this.rules = rules;

            if (this.rules.Count <= 0)
            {
                Debug.Log("[ISILab]: Generator contain 0 rules to generate map");
                return parent;
            }

            for (int i = 0; i < this.rules.Count; i++)
            {
                var ruleParent = rules[i].Generate(layer, settings); 
                ruleParent.SetParent(parent);
            }

            return parent;
        }
        #endregion

    }
}