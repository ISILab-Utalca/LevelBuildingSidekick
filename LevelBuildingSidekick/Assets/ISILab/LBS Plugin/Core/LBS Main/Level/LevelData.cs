using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick
{
    [System.Serializable]
    public class LevelData : Data
    {
        // public string levelName;
        [SerializeField, JsonRequired]
        private List<string> tags;

        [SerializeField, JsonRequired, SerializeReference]
        private List<ItemCategory> levelObjects;

        [SerializeField, JsonRequired]
        private int x, y, z;        

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSRepesentationData> representations = new List<LBSRepesentationData>();

        [JsonIgnore]
        public override Type ControllerType => throw new NotImplementedException();

        [JsonIgnore]
        public Vector3 Size
        {
            get => new Vector3(x, y, z);
            set
            {
                x = (int)value.x;
                y = (int)value.y;
                z = (int)value.z;
            }
        }

        [Obsolete("Esta funcion no se esta ocupanbdo actualemente pero se puede volver a usar a futuro")]
        public List<GameObject> RequestLevelObjects(string category)
        { 
            if(levelObjects.Find((i) => i.category == category) == null)
            {
                levelObjects.Add(new ItemCategory(category));
            }
            return levelObjects.Find((i) => i.category == category).items;

        }


        /// <summary>
        /// Add a new representation, if a representation of
        /// the type delivered already exists, it overwrites it.
        /// </summary>
        /// <param name="rep"></param>
        public void AddRepresentation(LBSRepesentationData rep) 
        {
            // puede que necesitemos  guardar reps del mismo tipo por lo que hay que revisar esta funcion denuevi (!!)
            var currentRep = representations.Find(r => r.GetType() == rep.GetType());
            if(currentRep != null)
            {
                representations.Remove(currentRep);
            }
            representations.Add(rep);
        }

        /// <summary>
        /// gets a level representation of the type indicated,
        /// if representation of the type is not found, creates it and return.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetRepresentation<T>() where T : LBSRepesentationData
        {
            var rep = (T)representations.Find(r => (r is T));

            if(rep != null)
                return rep;

            rep = Activator.CreateInstance(typeof(T)) as T;
            representations.Add(rep);

            return rep;
        }
    }
}

