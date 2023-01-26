using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Tools.Transformer;
using System;
using System.Linq;

namespace LBS.Components
{
    [System.Serializable]
    public class LBSLayer : ICloneable
    {

        [SerializeField, JsonRequired]
        private string id;

        [SerializeField, JsonRequired]
        private string name;
        
        [SerializeField, JsonRequired]
        private bool visible;

        [SerializeField, JsonRequired]
        public string iconPath;

        [SerializeField, JsonRequired, SerializeReference]
        private List<Transformer> transformers;

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSModule> modules;

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

        //EVENTS
        [JsonIgnore]
        public Action<LBSLayer> OnChanged;

        //CONSTRUCTORS
        public LBSLayer()
        {
            modules = new List<LBSModule>();
            transformers = new List<Transformer>();
            IsVisible = true;
            ID = GetType().Name;
        }

        public LBSLayer(List<LBSModule> modules, List<Transformer> transformers, string ID, bool visible, string name, string iconPath)
        {
            this.modules = modules;
            modules.ForEach(m => m.OnChanged += (mo) => { this.OnChanged(this); });
            this.transformers = transformers;
            this.ID = ID;
            IsVisible = visible;
            this.name = name;
            this.iconPath = iconPath;
        }

        //METHODS
        public bool AddModule(LBSModule module)
        {
            if(modules.Contains(module))
            {
                return false;
            }
            modules.Add(module);
            module.OnChanged += (mo) => { this.OnChanged(this); };
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
            module.OnChanged += (mo) => { this.OnChanged(this); };
            return true;
        }

        public bool RemoveModule(LBSModule module)
        {
            var b = modules.Remove(module);
            module.OnChanged -= (mo) => { this.OnChanged(this); };
            return b;
        }

        public LBSModule RemoveModuleAt(int index)
        {
            if(!modules.ContainsIndex(index))
            {
                return null;
            }
            var module = modules[index];
            modules.RemoveAt(index);
            module.OnChanged -= (mo) => { this.OnChanged(this); };
            return module;
        }

        public bool MoveModule(int index, LBSModule module)
        {
            if (!modules.Contains(module))
            {
                return false;
            }
            if(!modules.ContainsIndex(index))
            {
                return false;
            }
            modules.Remove(module);
            modules.Insert(index, module);
            return true;
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
                Debug.Log(module.GetType().Name + " - " + typeof(T).Name);
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

        public List<T> GetModules<T>(string ID = "") where T : LBSModule
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
            if (!modules.ContainsIndex(index))
            {
                return;
            }
            modules[index].IsVisible = false;
        }

        public void ShowModule(int index)
        {
            if (!modules.ContainsIndex(index))
            {
                return;
            }
            modules[index].IsVisible = true;
        }

        public bool AddTransformer(Transformer transformer)
        {
            if (transformers.Contains(transformer))
            {
                return false;
            }

            transformers.Add(transformer);
            transformer.OnAdd();
            return true;
        }

        public bool RemoveTransformer(Transformer transformer)
        {
            if(transformers.Contains(transformer))
            {
                transformer.OnRemove();
            }
            return transformers.Remove(transformer);
        }

        public Transformer RemoveTransformerAt(int index)
        {
            if(!transformers.ContainsIndex(index))
            {
                return null;
            }
            var transf = transformers[index];
            transf.OnRemove();
            transformers.RemoveAt(index);
            return transf;
        }

        public object Clone()
        {
            var modules = new List<LBSModule>(this.modules);
            var transformers = new List<Transformer>(this.transformers);
            var layer = new LBSLayer(modules,transformers,this.id,this.visible, this.name,this.iconPath);
            return layer;
        }
    }
}

