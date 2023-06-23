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
        public struct Settings
        {
            [SerializeField]
            [JsonConverter(typeof(Vector2Converter))]
            public Vector2 scale;
            [SerializeField]
            [JsonConverter(typeof(Vector2Converter))]
            public Vector2 resize;
            [SerializeField]
            [JsonConverter(typeof(Vector3Converter))]
            public Vector3 position;
            [SerializeField]
            public string name;
        }

        #region FIELDS
        [SerializeField]
        public Settings settings;
        #endregion

        [SerializeReference]
        private List<LBSGeneratorRule> rules = new List<LBSGeneratorRule>();

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
                if (!rule.CheckIfIsPosible(layer, out msg))
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

