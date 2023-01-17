using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Components.Teselation
{
    public class AreaModule<T> : TeselationModule where T : Area
    {
        //FIELDS

        [SerializeField, JsonRequired, SerializeReference]
        List<T> areas;

        //PROPERTIES

        [JsonIgnore]
        public int AreaCount => areas.Count;

        //METHODS

        public bool AddArea(T area)
        {
            if (areas.Contains(area))
                return false;
            areas.Add(area);
            return true;
        }

        public T GetArea(int index)
        {
            if (areas.ContainsIndex(index))
                return areas[index];
            return null;
        }

        public T GetArea(string id)
        {
            return areas.Find(a => a.ID.Equals(id));
        }

        public bool Remove(T area)
        {
            return areas.Remove(area);
        }

        public T RemoveAt(int index)
        {
            if (!areas.ContainsIndex(index))
                return null;
            var a = areas[index];
            areas.Remove(a);
            return a;
        }

        public override void Clear()
        {
            areas.Clear();
        }

        public override object Clone()
        {
            throw new System.NotImplementedException();
        }

        public override void Print()
        {
            throw new System.NotImplementedException();
        }


    }
}

