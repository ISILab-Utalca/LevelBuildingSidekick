using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Tools.Transformer;
using System;
using System.Linq;
using LBS.AI;
using LBS.Settings;
using LBS.Generator;
using LBS.Behaviours;
using LBS.Assisstants;

namespace LBS.Components
{
    [System.Serializable]
    public class LBSLayer : ICloneable
    {
        #region META-FIELDS
        [SerializeField, JsonRequired]
        private bool visible = true;

        [SerializeField, JsonRequired]
        private bool blocked = false;

        [PathTexture]
        [SerializeField, JsonRequired]
        public string iconPath = "Icon/Default";
        #endregion

        #region FIELDS
        [SerializeField, JsonRequired]
        private string id = "Default ID"; // asegurarse que se usa (!)

        [SerializeField, JsonRequired]
        private string name = "Layer name";

        [JsonRequired]
        private LBSLevelData parent; // esto es ignorable??

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSModule> modules = new List<LBSModule>();

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSBehaviour> behaviours = new List<LBSBehaviour>();

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSAssistantAI> assitants = new List<LBSAssistantAI>();

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSGeneratorRule> generatorRules = new List<LBSGeneratorRule>();

        [SerializeField, JsonRequired]
        private Generator3D.Settings settings;
        #endregion

        #region META-PROPERTIES
        [JsonIgnore]
        public bool IsVisible
        {
            get => visible;
            set => visible = value;
        }

        [JsonIgnore]
        public bool IsBlocked
        {
            get => blocked;
            set => blocked = value;
        }
        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public LBSLevelData Parent
        {
            get => parent;
            set => parent = value;
        }
        
        [JsonIgnore]
        public string ID
        {
            get => id;
            set => id = value;
        }

        [JsonIgnore]
        public string Name
        {
            get => name;
            set => name = value;
        }

        [JsonIgnore]
        public List<LBSModule> Modules
        {
            get => new List<LBSModule>(modules);
        }

        [JsonIgnore]
        public List<LBSBehaviour> Behaviours
        {
            get => new List<LBSBehaviour>(behaviours);
        }

        [JsonIgnore]
        public List<LBSAssistantAI> Assitants
        {
            get => new List<LBSAssistantAI>(assitants);
        }

        [JsonIgnore]
        public List<LBSGeneratorRule> GeneratorRules
        {
            get => new List<LBSGeneratorRule>(generatorRules);
        }

        [JsonIgnore]
        public Generator3D.Settings Settings
        {
            get => settings;
            set => settings = value;
        }

        [JsonIgnore]
        public Vector2Int TileSize
        {
            get
            {
                return new Vector2Int(
                    (int)settings.scale.x,
                    (int)settings.scale.y);
            }
            set
            {
                settings.scale.x = value.x;
                settings.scale.y = value.y;
                OnTileSizeChange?.Invoke(value);
            }
        }
        #endregion

        #region EVENTS
        public event Action<Vector2Int> OnTileSizeChange;
        public event Action<LBSLayer> onModuleChange;
        #endregion

        #region  CONSTRUCTORS
        public LBSLayer()
        {
            modules = new List<LBSModule>();
            
            IsVisible = true;
            ID = GetType().Name;
        }

        public LBSLayer(
            IEnumerable<LBSModule> modules,
            IEnumerable<LBSAssistantAI> assistant,
            IEnumerable<LBSGeneratorRule> rules,
            IEnumerable<LBSBehaviour> behaviours,
            string ID, bool visible, string name, string iconPath, Vector2Int tileSize)
        {
            foreach (var m in modules)
            {
                AddModule(m);
            }

            foreach(var a in assistant)
            {
                AddAssistant(a);
            }

            foreach(var r in rules)
            {
                AddGeneratorRule(r);
            }

            foreach(var b in behaviours)
            {
                AddBehaviour(b);
            }

            this.ID = ID;
            IsVisible = visible;
            this.name = name;
            this.iconPath = iconPath;
            this.TileSize = tileSize;
        }
        #endregion

        #region  METHODS
        public void Reload()
        {
            foreach (var module in modules)
            {
                module.Reload(this);
                module.Owner = this;
                module.OnChanged = (mo) =>
                {
                    this.onModuleChange?.Invoke(this);
                };
            }

            foreach(var assistant in assitants)
            {
                //assistant.Reload();
                assistant.Owner = this;
            }

            foreach (var rule in generatorRules)
            {
                // Implement Reload and SetOwner ??
            }

            
            foreach (var behaviour in behaviours)
            {
                behaviour.Owner = this;
            }

        }

        public void AddBehaviour(LBSBehaviour behaviour)
        {
            if (this.behaviours.Contains(behaviour))
            {
                Debug.Log("[ISI Lab]: This layer already contains the behavior " + behaviour.GetType().Name + ".");
                return;
            }

            this.behaviours.Add(behaviour);
            behaviour.Init(this);

            var reqModules = behaviour.GetRequieredModules();
            foreach (var type in reqModules)
            {
                // aqui podria ser importante preguntar por una key en particular por si
                // existen dos modulos del mismo tipo pero para cosas diferetnes (!!)
                if (!modules.Any(e => e.GetType() == type))         
                {
                    this.AddModule(Activator.CreateInstance(type) as LBSModule);
                }
            }
        }

        public void RemoveBehaviour(LBSBehaviour behaviour)
        {
            this.behaviours.Remove(behaviour);
        }

        public void AddGeneratorRule(LBSGeneratorRule rule)
        {
            this.generatorRules.Add(rule);
        }

        public bool RemoveGeneratorRule(LBSGeneratorRule rule)
        {
            return this.generatorRules.Remove(rule);
        }

        public void AddAssistant(LBSAssistantAI assistant)
        {
            if (this.assitants.Find( a => assistant.GetType().Equals(a.GetType())) != null)
            {
                Debug.Log("[ISI Lab]: This layer already contains the assistant " + assistant.GetType().Name + ".");
                return;
            }

            this.assitants.Add(assistant);
            assistant.Owner = this;
        }

        public bool RemoveAssitant(LBSAssistantAI assistant)
        {
            assistant.Owner = null;
            return this.assitants.Remove(assistant);
        }

        public LBSAssistantAI GetAssistant(int index)
        {
            return assitants[index];
        }

        public bool AddModule(LBSModule module)
        {
            if(modules.Contains(module))
            {
                return false;
            }
            modules.Add(module);
            module.Owner = this;
            module.OnChanged += (mo) => 
            { 
                this.onModuleChange?.Invoke(this); 
            };
            module.OnAttach(this);
            return true;
        }

        public bool RemoveModule(LBSModule module)
        {
            var removed = modules.Remove(module);
            if (removed)
            {
                module.Owner = null;
                module.OnChanged -= (mo) => { this.onModuleChange(this); };
            }
            module.OnDetach(this);
            return removed;
        }

        /*
        public bool InsertModule(int index, LBSModule module)
        {
            if (modules.Contains(module))
            {
                return false;
            }
            if (!(modules.ContainsIndex(index) || index == modules.Count))
            {
                return false;
            }
            modules.Insert(index, module);
            module.Owner = this;
            module.OnChanged += (mo) => { this.onModuleChange(this); };
            return true;
        }
        */

        /*
        public LBSModule RemoveModuleAt(int index)
        {
            var module = modules[index];
            RemoveModule(module);
            return module;
        }
        */

        public LBSModule GetModule(int index)
        {
            return modules[index];
        }

        public T GetModule<T>(string ID = "") where T : LBSModule
        {
            var t = typeof(T);
            foreach (var module in modules)
            {
                if (module is T || Utility.Reflection.IsSubclassOfRawGeneric(t,module.GetType()))
                {
                    if(ID.Equals("") || module.ID.Equals(ID))
                    {
                        return module as T;
                    }
                }
            }
            return null;
        }

        public object GetModule(Type type ,string ID = "")
        {
            foreach (var module in modules)
            {
                if (module.GetType().Equals(type) || Utility.Reflection.IsSubclassOfRawGeneric(type, module.GetType()))
                {
                    if (ID.Equals("") || module.ID.Equals(ID))
                    {
                        return module;
                    }
                }
            }
            return null;

        }

        /*
        public List<T> GetModules<T>(string ID = "") where T : LBSModule  // (?) sobra?
        {
            List<T> mods = new List<T>();
            foreach (var mod in modules)
            {
                if (ID.Equals("") || mod.Key.Equals(ID))
                    mods.Add(mod as T);
            }
            return mods;
        }
        */

        public void BlockLayer(bool value)
        {
            blocked = value;
        }

        public void ShowModule(int index,bool value)
        {
            modules[index].IsVisible = value;
        }

        internal void SetModule<T>(T module, string key = "") where T : LBSModule
        {
            var index = -1;
            if (key.Equals(""))
            {
                index = modules.FindIndex(m => m is T);
                modules[index] = module;
                return;
            }

            index = modules.FindIndex(m => m is T && m.ID.Equals(key));

            modules[index].OnDetach(this);
            modules[index] = module;
            modules[index].OnAttach(this);
            modules[index].Owner = this;
        }

        public Vector2Int ToFixedPosition(Vector2 position) // esto tiene que ir en una extension (?)
        {
            Vector2 pos = position / (TileSize * LBSSettings.Instance.TileSize);

            if (pos.x < 0)
                pos.x -= 1;

            if (pos.y < 0)
                pos.y -= 1;

            return pos.ToInt();
        }

        public object Clone()
        {
            var modules = this.modules.Select(m => m.Clone() as LBSModule);
            var assistants = this.assitants.Select(a => a.Clone() as LBSAssistantAI);
            var rules = this.generatorRules.Select(r => r.Clone() as LBSGeneratorRule);
            var behaviours = this.behaviours.Select(b => b.Clone() as LBSBehaviour);

            var layer = new LBSLayer(modules, assistants, rules, behaviours, this.id, this.visible, this.name, this.iconPath, this.TileSize);
            return layer;
        }
        #endregion
    }
}

