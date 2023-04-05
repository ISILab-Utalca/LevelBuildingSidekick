using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Tools.Transformer;
using System;
using System.Linq;
using LBS.AI;

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
        public string iconPath; // (?) esto tiene que estar en la layertemplate

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSModule> modules = new List<LBSModule>();

        //[SerializeField, JsonRequired]
        //[ScriptableToString(typeof(CompositeBundle))]
        //private List<string> bundles = new List<string>();

        [SerializeField, JsonRequired]
        [ScriptableToString(typeof(LBSLayerAssistant))]
        private string assistant;

        #endregion

        #region PROPERTIES

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

        /*
        [JsonIgnore]
        public CompositeBundle Bundle
        {
            get
            {
                var bundle = ScriptableObject.CreateInstance<CompositeBundle>();

                foreach(var b in bundles)
                {
                    bundle.Add(Utility.DirectoryTools.GetScriptable<Bundle>(b));
                }

                return bundle;
            }
        }
        */

        public event Action<LBSLayer> OnModuleChange 
        {   
            add => onModuleChange += value;
            remove => onModuleChange -= value; 
        }

        #endregion

        #region EVENTS
        private event Action<LBSLayer> onModuleChange;
        #endregion

        #region  CONSTRUCTORS
        public LBSLayer()
        {
            modules = new List<LBSModule>();
            
            IsVisible = true;
            ID = GetType().Name;
        }

        public LBSLayer(List<LBSModule> modules, string ID, bool visible, string name, string iconPath)
        {
            modules.ForEach(m => {
                AddModule(m);
            });

            this.ID = ID;
            IsVisible = visible;
            this.name = name;
            this.iconPath = iconPath;
        }
        #endregion

        #region  METHODS
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
        }

        public object Clone()
        {
            var modules = this.modules.Select(m => {
                var module = m.Clone() as LBSModule;
                return module;
                }).ToList();
            //var transformers = this.GetTransformers(); // (??) usar clone en vez de pasar la lista?
            var layer = new LBSLayer(modules,/* transformers.Select(t => t.GetType()).ToList(),*/ this.id, this.visible, this.name, this.iconPath);
            layer.Assitant = Assitant;

            return layer;
        }

        #endregion
    }
}

