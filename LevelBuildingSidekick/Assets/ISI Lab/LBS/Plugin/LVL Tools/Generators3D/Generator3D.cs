using LBS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components;
using LBS.Generator;
using Newtonsoft.Json;

namespace LBS.Generator
{
    [System.Serializable]
    public class Generator3D
    {
        [System.Serializable]
        public struct Settings
        {
            [SerializeField]
            public Vector2 scale;
            [SerializeField]
            public Vector2 resize;
            [SerializeField]
            public Vector3 position;
            [SerializeField]
            public string name;
        }

        #region FIELDS
        public Settings settings;
        #endregion

        [SerializeReference]
        private List<GeneratorRule> rules = new List<GeneratorRule>();

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
        public void AddRule(GeneratorRule rule)
        {
            rules.Add(rule);
            rule.generator3D = this;
        }

        public void RemoveRule(GeneratorRule rule)
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

        public GameObject Generate(LBSLayer layer, Settings settings)
        {
            var name = settings.name;
            var parent = new GameObject(name);

            if (rules.Count <= 0)
            {
                Debug.Log("[ISILab]: Generator contain 0 rules to generate map");
                return parent;
            }

            for (int i = 0; i < rules.Count; i++)
            {
                var ruleParent = new GameObject("sub_" + name + "_" + i);
                ruleParent = rules[i].Generate(layer, settings);
                ruleParent.SetParent(parent);
            }

            return parent;
        }
        #endregion


    }

    [SerializeField]
    public abstract class GeneratorRule
    {
        [JsonIgnore]
        internal Generator3D generator3D;

        public GeneratorRule() { }

        public abstract GameObject Generate(LBSLayer layer, Generator3D.Settings settings);

        public abstract bool CheckIfIsPosible(LBSLayer layer, out string msg); // mejorar nombre (!!!)
    }
}

public static class GameObjectExtension
{
    public static void SetParent(this GameObject gameObjct, GameObject other)
    {
        gameObjct.transform.parent = other.transform;
    }
}

