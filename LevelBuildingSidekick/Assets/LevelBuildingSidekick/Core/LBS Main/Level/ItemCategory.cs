using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick
{
    [System.Serializable]
    public class ItemCategory
    {
        public string category;
        public List<GameObject> items; // cambiar a string (!!!)

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

        public ItemCategory() { }
    }
}
