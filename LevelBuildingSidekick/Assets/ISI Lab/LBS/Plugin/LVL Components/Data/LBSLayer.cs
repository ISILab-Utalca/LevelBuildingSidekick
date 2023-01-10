using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Components
{
    public class LBSLayer
    {
        List<LBSModule> modules;

        bool visible;
        public bool IsVisible
        {
            get => visible;
            set => visible = value;
        }

        public bool AddModule(LBSModule module)
        {
            if(modules.Contains(module))
            {
                return false;
            }
            modules.Add(module);
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

        public LBSModule GetModule(int index)
        {
            return modules[index];
        }

        public T GetModule<T>() where T : LBSModule
        {
            foreach(var mod in modules)
            {
                if (mod is T)
                    return mod as T;
            }
            return null;
        }

        public List<T> GetModules<T>() where T : LBSModule
        {
            List<T> mods = new List<T>();
            foreach (var mod in modules)
            {
                if (mod is T)
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

