using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Components
{
    [System.Serializable]
    public class LBSLayer
    {
        //FIELDS

        [SerializeField, JsonRequired, SerializeReference]
        string id;

        [SerializeField, JsonRequired, SerializeReference]
        List<LBSModule> modules;

        [SerializeField, JsonRequired, SerializeReference]
        bool visible;

        //PROPERTIES
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
            this.ID = ID;
            IsVisible = visible;
        }

        //MEHTODS
        public bool AddModule(LBSModule module)
        {
            if(modules.Contains(module))
            {
                return false;
            }
            modules.Add(module);
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
            return true;
        }

        public bool RemoveModule(LBSModule module)
        {
            return modules.Remove(module);
        }

        public LBSModule RemoveModuleAt(int index)
        {
            if(!modules.ContainsIndex(index))
            {
                return null;
            }
            var mod = modules[index];
            modules.RemoveAt(index);
            return mod;
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
            foreach (var mod in modules)
            {
                if (mod is T)
                {
                    if(ID.Equals("") || mod.ID.Equals(ID))
                        return mod as T;
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


    }
}

