using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Tools;
using System;

namespace LBS.Components
{
    [System.Serializable]
    public class LBSLayer
    {

        [SerializeField, JsonRequired]
        private string id;

        [SerializeField, JsonRequired, SerializeReference]
        private List<Transformer> transformers;

        [SerializeField]
        [JsonRequired, SerializeReference]
        private List<LBSModule> modules;

        [SerializeField]
        [JsonRequired]
        private bool visible;

        [JsonIgnore]
        public bool IsVisible
        {
            get => visible;
            set => visible = value;
        }
        public string ID
        {
            get => id;
            set => id = value;
        }

        //EVENTS
        public Action OnChanged;

        //CONSTRUCTORS
        public LBSLayer()
        {
            modules = new List<LBSModule>();
            IsVisible = true;
            ID = GetType().Name;
        }

        public LBSLayer(List<LBSModule> modules, string ID, bool visible)
        {
            this.modules = modules;
            modules.ForEach(m => m.OnChanged += this.OnChanged);
            this.ID = ID;
            IsVisible = visible;
        }

        //METHODS
        public bool AddModule(LBSModule module)
        {
            if(modules.Contains(module))
            {
                return false;
            }
            modules.Add(module);
            module.OnChanged += this.OnChanged;
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
            module.OnChanged += this.OnChanged;
            return true;
        }

        public bool RemoveModule(LBSModule module)
        {
            var b = modules.Remove(module);
            module.OnChanged -= this.OnChanged;
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
            module.OnChanged -= this.OnChanged;
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
            foreach (var module in modules)
            {
                if (module is T)
                {
                    if(ID.Equals("") || module.ID.Equals(ID))
                        return module as T;
                }
            }
            return null;
        }

        public List<T> GetModules<T>(string ID = "") where T : LBSModule
        {
            List<T> mods = new List<T>();
            foreach (var mod in modules)
            {
                if (ID.Equals("") || mod.ID.Equals(ID))
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
    }
}

