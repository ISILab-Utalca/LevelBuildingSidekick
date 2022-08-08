using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

namespace LevelBuildingSidekick
{
    [System.Serializable]
    public class ItemCategory
    {
        public string category;
        [JsonIgnore]
        public List<GameObject> items; // cambiar a string (!!!)
        private List<string> itemNames;
        public List<string> ItemNames
        {
            get
            {
                if(items != null)
                {
                    if(items.Count == 0)
                    {
                        itemNames = new List<string>();
                    }
                    else
                    {
                        itemNames = items.Select(go => go.name).ToList();
                    }
                }
                return itemNames;
            }
            set
            {
                itemNames = value;
            }
        }

        public ItemCategory(string _category, List<GameObject> _items)
        {
            category = _category;
            items = _items;
        }

        public ItemCategory(string _category)
        {
            category = _category;
            items = new List<GameObject>();
        }

        public ItemCategory() 
        {
            items = new List<GameObject>();
        }
    }
}
