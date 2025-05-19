using System;
using System.Collections;
using System.Collections.Generic;
using ISILab.LBS.Assistants;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Generators;
using ISILab.LBS.Modules;
using ISILab.Macros;
using LBS;
using LBS.Components;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS
{
    [System.Serializable]
    public class LBSLevelData
    {
        #region FIELDS
        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSLayer> layers = new List<LBSLayer>();

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSLayer> quests = new List<LBSLayer>();
        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public List<LBSLayer> Layers => layers;

        [JsonIgnore]
        public List<LBSLayer> Quests => quests;

        [JsonIgnore]
        public int LayerCount => layers.Count;
        #endregion

        #region EVENTS
        public event Action<LBSLevelData> OnChanged;

        [JsonIgnore]
        public Action OnReload;

        #endregion

        #region METHODS
        public void Reload()
        {
            foreach (var layer in layers)
            {
                layer.Reload();
                layer.OnAddModule += (layer, module) => this.OnChanged?.Invoke(this);
                layer.Parent = this;
            }

            foreach (var q in quests)
            {
                q.Reload();
                q.OnAddModule += (layer, module) => this.OnChanged?.Invoke(this);
                q.Parent = this;
            }

            OnReload?.Invoke();
        }
        
        public LBSLayer GetLayer(int index)
        {
            return layers[index];
        }

        /// <summary>
        /// Retrieves a representation from the list of layers by its ID
        /// and returns it. If the representation is not found or an exception
        /// is thrown, returns null.
        /// </summary>
        /// <param name="id">The ID of the representation to retrieve</param>
        /// <returns>The representation with the specified ID or null</returns>
        public LBSLayer GetLayer(string id)
        {
            try
            {
                return layers.Find(l => l.ID == id);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return null;
        }

        /// <summary>
        /// Add a new representation, if a representation of
        /// the type delivered already exists, it overwrites it.
        /// </summary>
        /// <param name="layer"></param>
        public void AddLayer(LBSLayer layer)
        {
            if (layer == null || layers.Contains(layer))
                return;

            layers.Insert(0, layer);
            layer.OnAddModule += (layer, module) => this.OnChanged?.Invoke(this);
            layer.Parent = this;
            this.OnChanged?.Invoke(this);
        }

        /// <summary>
        /// Removes a layer from the list of layers and unsubscribes
        /// the OnChanged event of the removed layer.
        /// </summary>
        /// <param name="layer">The layer to remove</param>
        public void RemoveLayer(LBSLayer layer)
        {
            layers.Remove(layer);
            layer.OnAddModule -= (layer, module) => this.OnChanged(this);
            this.OnChanged?.Invoke(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldLayer"></param>
        /// <param name="newLayer"></param>
        public void ReplaceLayer(LBSLayer oldLayer, LBSLayer newLayer)
        {
            var index = layers.IndexOf(oldLayer);
            RemoveLayer(oldLayer);
            layers.Insert(index, newLayer);
            this.OnChanged?.Invoke(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public LBSLayer RemoveAt(int index)
        {
            var layer = layers[index];
            layers.RemoveAt(index);
            layer.OnAddModule -= (layer, module) => this.OnChanged(this);
            this.OnChanged?.Invoke(this);
            return layer;
        }

        public LBSLayer AddQuest(string name)
        {
            var quest = new LBSLayer();

            quest.ID = name;
            quest.Name = name;
            quest.iconGuid = "Assets/ISI Lab/Commons/Assets2D/Resources/Icons/Quest_Icon/IconQuestTitle2.png";
            quest.TileSize = new Vector2Int(2, 2);
            quest.AddGeneratorRule(new QuestRuleGenerator());

            string questBehaviorGuidIcon = "49b9448c876b36c4ba26740d7deae035";
            var grammarIcon = LBSAssetMacro.LoadAssetByGuid<VectorImage>(questBehaviorGuidIcon);
            var behaviour = new QuestBehaviour(grammarIcon, "Quest", Settings.LBSSettings.Instance.view.behavioursColor);
            var assistant = new GrammarAssistant(grammarIcon, "Grammar", Settings.LBSSettings.Instance.view.assistantColor);
            quest.AddAssistant(assistant);
            quest.AddBehaviour(behaviour);
            quests.Add(quest);

            this.OnChanged?.Invoke(this);
            return quest;
        }

        public QuestGraph RemoveQuestAt(int index)
        {
            var q = quests[index];
            quests.RemoveAt(index);

            var qg = q.GetModule<QuestGraph>();

            this.OnChanged?.Invoke(this);
            return qg;
        }


        public override bool Equals(object obj)
        {
            // cast other object
            var other = obj as LBSLevelData;

            // check if other object are the same type
            if (other == null) return false;

            // get amount of layers
            var lCount = other.layers.Count;

            // check if amount of layers are EQUALS
            if (this.layers.Count != lCount) return false;

            // check if contain EQUALS layers
            for (int i = 0; i < lCount; i++)
            {
                var l1 = this.layers[i];
                var l2 = other.layers[i];

                if (!l1.Equals(l2)) return false;
            }

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
        #endregion
    }
}
