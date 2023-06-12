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

namespace LBS.Components
{
    [System.Serializable]
    public class LBSLayer : ICloneable
    {
        #region FIELDS
        [SerializeField, JsonRequired]
        private string id;

        [SerializeField, JsonRequired]
        private string name;

        [SerializeField, JsonRequired]
        private bool visible;

        [SerializeField, JsonRequired]
        [PathTexture]
        public string iconPath; // esto tiene que estar en la layerTemplate (?)

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSModule> modules = new List<LBSModule>();

        [JsonRequired]//[SerializeField, JsonRequired, SerializeReference] // SerializeReference (?), SerializeField (?)
        private LBSLevelData parent;

        [SerializeField, JsonRequired]
        public Generator3D.Settings settingsGen3D;

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSBehaviour> behaviours = new List<LBSBehaviour>();

        [SerializeField, JsonRequired, SerializeReference]
        [ScriptableObjectReference(typeof(LBSLayerAssistant))]
        private List<string> assitantsAI = new List<string>(); // por que uso un string cuando puedo usar la clase (???)

        [SerializeField, JsonRequired]
        [ScriptableObjectReference(typeof(LBSLayerAssistant))]
        private string assistant;

        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public List<LBSBehaviour> Behaviours // array (?)
        {
            get => new List<LBSBehaviour>(behaviours);
        }

        [JsonIgnore]
        public List<string> AssitantAI
        {
            get => new List<string>(assitantsAI); // cambiar de string al objeto
        }


        [JsonIgnore]
        public LBSLevelData Parent
        {
            get => parent;
            set => parent = value;
        }

        [JsonIgnore]
        public bool IsVisible
        {
            get => visible;
            set => visible = value;
        }

        [JsonIgnore]
        public string ID
        {
            get => id;
            set => id = value;
        }

        [JsonIgnore]
        public Vector2Int TileSize
        {
            get
            { 
                return new Vector2Int((int)settingsGen3D.scale.x, (int)settingsGen3D.scale.y); 
            }
            set
            {
                settingsGen3D.scale.x = value.x;
                settingsGen3D.scale.y = value.y;
                OnTileSizeChange?.Invoke(value);
            }
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
        public LBSLayerAssistant Assitant
        {
            get => Utility.DirectoryTools.GetScriptable<LBSLayerAssistant>(assistant);
            set => assistant = value.name;
        }
        #endregion

        #region EVENTS
        public event Action<Vector2Int> OnTileSizeChange;

        private event Action<LBSLayer> onModuleChange;
        public event Action<LBSLayer> OnModuleChange
        {
            add => onModuleChange += value;
            remove => onModuleChange -= value;
        }
        #endregion

        #region  CONSTRUCTORS
        public LBSLayer()
        {
            modules = new List<LBSModule>();
            
            IsVisible = true;
            ID = GetType().Name;
        }

        public LBSLayer(List<LBSModule> modules, string ID, bool visible, string name, string iconPath, Vector2Int tileSize)
        {
            modules.ForEach(m => {
                AddModule(m);
            });

            this.ID = ID;
            IsVisible = visible;
            this.name = name;
            this.iconPath = iconPath;
            this.TileSize = tileSize;
        }
        #endregion

        #region  METHODS
        public void AddBehaviour(LBSBehaviour behaviour)
        {
            if (this.behaviours.Contains(behaviour))
            {
                Debug.Log("[ISI Lab]: This layer already contains the behavior " + behaviour.GetType().Name + ".");
                return;
            }

            this.behaviours.Add(behaviour);

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

        public void Reload()
        {
            foreach (var module in modules)
            {
                module.OnReload(this);
                module.Owner = this;
                module.OnChanged = (mo) =>
                {
                    this.onModuleChange?.Invoke(this);
                };
            }
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

        public bool RemoveModule(LBSModule module)
        {
            var removed = modules.Remove(module);
            if(removed)
            {
                module.Owner = null;
                module.OnChanged -= (mo) => { this.onModuleChange(this); };
            }
            module.OnDetach(this);
            return removed;
        }

        public LBSModule RemoveModuleAt(int index)
        {
            var module = modules[index];
            RemoveModule(module);
            return module;
        }

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
                    if(ID.Equals("") || module.Key.Equals(ID))
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
                    if (ID.Equals("") || module.Key.Equals(ID))
                    {
                        return module;
                    }
                }
            }
            return null;

        }

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

        public void HideModule(int index)
        {
            modules[index].IsVisible = false;
        }

        public void ShowModule(int index)
        {
            modules[index].IsVisible = true;
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

            index = modules.FindIndex(m => m is T && m.Key.Equals(key));

            modules[index].OnDetach(this);
            modules[index] = module;
            modules[index].OnAttach(this);
            modules[index].Owner = this;
        }

        public Vector2Int ToFixedPosition(Vector2 position) // esto tiene que ir en una extension
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
            var modules = this.modules.Select(m => {
                var module = m.Clone() as LBSModule;
                return module;
                }).ToList();

            var layer = new LBSLayer(modules, this.id, this.visible, this.name, this.iconPath,this.TileSize);
            layer.Assitant = Assitant;
           // layer.TileSize = TileSize;

            return layer;
        }

        #endregion
    }
}

