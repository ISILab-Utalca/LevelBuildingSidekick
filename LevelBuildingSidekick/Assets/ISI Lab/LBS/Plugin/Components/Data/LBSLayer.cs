using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISILab.Commons.Utility;
using ISILab.Extensions;
using ISILab.LBS;
using ISILab.LBS.Assistants;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Generators;
using ISILab.LBS.Modules;
using ISILab.LBS.Settings;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace LBS.Components
{
    [Serializable]
    public class LBSLayer : ICloneable
    {
        #region META-FIELDS
        [SerializeField, JsonRequired]
        private bool visible = true;

        [SerializeField, JsonRequired]
        private bool blocked = false;

        [FormerlySerializedAs("iconGUID")] [FormerlySerializedAs("iconPath")] [SerializeField, JsonRequired]
        public string iconGuid = "Icon/Default";
        #endregion

        #region FIELDS
        [SerializeField, JsonRequired]
        private string id = "Default ID";

        [SerializeField, JsonRequired]
        private string name = "Layer name";

        [JsonIgnore]
        private LBSLevelData _parent;

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSModule> modules = new();

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSBehaviour> behaviours = new();

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSAssistant> assistants = new();

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSGeneratorRule> generatorRules = new();

        [SerializeField, JsonRequired]
        private Generator3D.Settings settings = new();
       
        [SerializeField, JsonRequired]
        public int index;

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
        public bool IsLocked
        {
            get => blocked;
            set => blocked = value;
        }

        [JsonIgnore]
        public LBSLevelData Parent
        {
            get => _parent;
            set => _parent = value;
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
        public List<LBSModule> Modules => new(modules);

        [JsonIgnore]
        public List<LBSBehaviour> Behaviours => new(behaviours);

        [JsonIgnore]
        public List<LBSAssistant> Assistants => new(assistants);

        [JsonIgnore]
        public List<LBSGeneratorRule> GeneratorRules => new(generatorRules);


        [JsonIgnore]
        public Generator3D.Settings Settings
        {
            get => settings;
            set => settings = value;
        }

        [JsonIgnore]
        public Vector2Int TileSize
        {
            get =>
                new((int)settings.scale.x,
                    (int)settings.scale.y);
            set
            {
                settings.scale.x = value.x;
                settings.scale.y = value.y;
                OnTileSizeChange?.Invoke(value);
            }
        }
        #endregion

        #region EVENTS

        public event Action OnChangeName;
        public event Action OnChange; // call whenever needing to update a change on a single layer
        public event Action<Vector2Int> OnTileSizeChange;
        public event Action<LBSLayer, LBSModule> OnAddModule;
        public event Action<LBSLayer, LBSModule> OnReplaceModule;
        public event Action<LBSLayer, LBSModule> OnRemoveModule;
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
            IEnumerable<LBSAssistant> assistant,
            IEnumerable<LBSGeneratorRule> rules,
            IEnumerable<LBSBehaviour> behaviours,
            string ID, bool visible, string name, string iconGuid, Vector2Int tileSize)
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
            this.iconGuid = iconGuid;
            TileSize = tileSize;
        }
        #endregion

        #region  METHODS
        public void ReplaceModule(LBSModule oldModule, LBSModule newModule)
        {
            var index = modules.IndexOf(oldModule);
            RemoveModule(oldModule);
            modules.Insert(index, newModule);
            OnReplaceModule?.Invoke(this, newModule);
        }

        public void Reload()
        {
            foreach (var module in modules)
            {
                module.OnAttach(this);
            }

            foreach(var assistant in assistants)
            {
                assistant.OnAttachLayer(this);
            }

            foreach (var rule in generatorRules)
            {
                // rule.OnAttachLayer(this);
            }

            foreach (var behaviour in behaviours)
            {
                behaviour.OnAttachLayer(this);
            }
        }

        public void AddBehaviour(LBSBehaviour behaviour)
        {
            if (behaviours.Contains(behaviour))
            {
                Debug.Log("[ISI Lab]: This layer already contains the behavior " + behaviour.GetType().Name + ".");
                return;
            }

            behaviours.Add(behaviour);

            // check if the layer have necessary 'Modules'
            var reqModules = behaviour.GetRequiredModules();
            foreach (var type in reqModules)
            {
                if (!modules.Any(e => e.GetType() == type))         
                {
                    AddModule(Activator.CreateInstance(type) as LBSModule);
                }
            }

            behaviour.OnAttachLayer(this);
        }

        public void RemoveBehaviour(LBSBehaviour behaviour)
        {
            behaviours.Remove(behaviour);
            behaviour.OnDetachLayer(this);
        }

        public void AddGeneratorRule(LBSGeneratorRule rule)
        {
            generatorRules.Add(rule);
        }

        public bool RemoveGeneratorRule(LBSGeneratorRule rule)
        {
            return generatorRules.Remove(rule);
        }

        public void AddAssistant(LBSAssistant assistant)
        {
            if (assistants.Find( a => assistant.GetType().Equals(a.GetType())) != null)
            {
                Debug.Log("[ISI Lab]: This layer already contains the assistant " + assistant.GetType().Name + ".");
                return;
            }

            assistants.Add(assistant);

            var reqModules = assistant.GetRequieredModules();
            foreach (var type in reqModules)
            {
                if (!modules.Any(e => e.GetType() == type))
                {
                    AddModule(Activator.CreateInstance(type) as LBSModule);
                }
            }
            assistant.OnAttachLayer(this);

        }

        public void RemoveAssitant(LBSAssistant assistant)
        {
            assistants.Remove(assistant);
            assistant.OnDetachLayer(this);
        }

        public LBSAssistant GetAssistant(int index)
        {
            return assistants[index];
        }

        public bool AddModule(LBSModule module)
        {
            if(modules.Contains(module))
            {
                return false;
            }

            modules.Add(module);
            module.OnAttach(this);

            OnAddModule?.Invoke(this, module);

            return true;
        }

        public bool RemoveModule(LBSModule module)
        {
            var removed = modules.Remove(module);
            module.OnDetach(this);
            OnRemoveModule?.Invoke(this,module);
            return removed;
        }

        public LBSModule GetModule(int index)
        {
            return modules[index];
        }

        public LBSModule GetModule(string ID)
        {
            foreach (var module in modules)
            {
                if(module.ID == ID)
                    return module;
            }
            return null;
        }

        public T GetModule<T>(string ID = "") where T : LBSModule
        {
            var t = typeof(T);
            foreach (var module in modules)
            {
                if (module is T || Reflection.IsSubclassOfRawGeneric(t,module.GetType()))
                {
                    if(ID.Equals("") || module.ID.Equals(ID))
                    {
                        return module as T;
                    }
                }
            }
            return null;
        }

        public T GetAssistant<T>(string ID = "") where T : LBSAssistant
        {
            var t = typeof(T);
            foreach (var a in assistants)
            {
                if (a is T || Reflection.IsSubclassOfRawGeneric(t, a.GetType()))
                {
                    if (ID.Equals("") || a.Name.Equals(ID))
                    {
                        return a as T;
                    }
                }
            }
            return null;
        }

        public object GetModule(Type type ,string ID = "")
        {
            foreach (var module in modules)
            {
                if (module.GetType().Equals(type) || Reflection.IsSubclassOfRawGeneric(type, module.GetType()))
                {
                    if (ID.Equals("") || module.ID.Equals(ID))
                    {
                        return module;
                    }
                }
            }
            return null;

        }
        public T GetBehaviour<T>(string ID = "") where T : LBSBehaviour
        {
            var t = typeof(T);
            foreach (var b in behaviours)
            {
                if (b is T || Reflection.IsSubclassOfRawGeneric(t, b.GetType()))
                {
                    if (ID.Equals("") || b.Name.Equals(ID))
                    {
                        return b as T;
                    }
                }
            }
            return null;
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
            modules[index].OwnerLayer = this;
        }

        // esto tiene que ir en una extension (?)
        public Vector2Int ToFixedPosition(Vector2 position) 
        {
            Vector2 pos = position / (TileSize * LBSSettings.Instance.general.TileSize);

            if (pos.x < 0)
                pos.x -= 1;

            if (pos.y < 0)
                pos.y -= 1;

            pos = new Vector2(pos.x, -pos.y);

            return pos.ToInt();
        }

        public Vector2 FixedToPosition(Vector2Int position, bool invertY = false) 
        {
            var tileSizeX = TileSize.x * LBSSettings.Instance.general.TileSize.x;
            var tileSizeY = TileSize.y * LBSSettings.Instance.general.TileSize.y;
            if(invertY) tileSizeY = -tileSizeY;
            return new Vector2(position.x * tileSizeX, position.y * tileSizeY);
        }
        
        public (Vector2Int,Vector2Int) ToFixedPosition(Vector2 startPos, Vector2 endPos)
        {
            var sPos = ToFixedPosition(startPos);
            var ePos = ToFixedPosition(endPos);

            var min = new Vector2Int(
                (sPos.x < ePos.x) ? sPos.x : ePos.x,
                (sPos.y < ePos.y) ? sPos.y : ePos.y);

            var max = new Vector2Int(
                (sPos.x > ePos.x) ? sPos.x : ePos.x,
                (sPos.y > ePos.y) ? sPos.y : ePos.y);

            return (min, max);
        }
        

        public object Clone()
        {
            var modules = this.modules.Clone(); // CloneRef
            var assistants = this.assistants.Select(a => a.Clone() as LBSAssistant);
            var rules = generatorRules.Select(r => r.Clone() as LBSGeneratorRule);
            var behaviours = this.behaviours.Select(b => b.Clone() as LBSBehaviour);

            var layer = new LBSLayer(modules, assistants, rules, behaviours, id, visible, name, iconGuid, TileSize);
            return layer;
        }

        public override bool Equals(object obj)
        {
            var other = obj as LBSLayer;

            // check if the same type
            if (other == null) return false;

            // check if have the same ID
            if(other.id != id) return false;

            // check if have the same name
            if (other.name != name) return false;

            // get amount of modules
            var mCount = other.modules.Count;

            // cheack amount of modules
            if (modules.Count != mCount) return false;

            for (int i = 0; i < mCount; i++)
            {
                var m1 = other.modules[i];
                var m2 = modules[i];
                if (!m1.Equals(m2)) return false;
            }

            // get amount of modules
            var bCount = other.behaviours.Count;
            // cheack amount of behaviours
            if (behaviours.Count != bCount) return false;

            for (int i = 0; i < bCount; i++)
            {
                var b1 = other.behaviours[i];
                var b2 = behaviours[i];

                if (!b1.Equals(b2)) return false;
            }

            // get amount of modules
            var aCount = other.assistants.Count;

            // cheack amount of behaviours
            if (assistants.Count != aCount) return false;

            for (int i = 0; i < aCount; i++)
            {
                var a1 = other.assistants[i];
                var a2 = assistants[i];

                if (!a1.Equals(a2)) return false;
            }

            // get amount of modules
            var gCount = other.generatorRules.Count;

            // cheack amount of behaviours
            if (generatorRules.Count != gCount) return false;

            for (int i = 0; i < gCount; i++)
            {
                var g1 = other.generatorRules[i];
                var g2 = generatorRules[i];

                if (!g1.Equals(g2)) return false;
            }
            // check if have EQUALS settings
            if (!settings.Equals(other.settings)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void CleanGraphViews()
        {
            foreach (var behaviour in behaviours)
            {
                
            }
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
        public void OnChangeUpdate()
        {
            OnChange?.Invoke();
        }
        #endregion

        public void InvokeNameChanged()
        {
            OnChangeName?.Invoke();
        }
    }

}

