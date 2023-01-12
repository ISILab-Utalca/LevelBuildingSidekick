using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components;

namespace LBS
{
    [System.Serializable]
    public class LevelDataOld
    {
        #region FIELDS

        [SerializeField, JsonRequired]
        private List<string> tags = new List<string>();

        [SerializeField, JsonRequired, SerializeReference]
        private List<ItemCategory> levelObjects = new List<ItemCategory>();

        [SerializeField, JsonRequired]
        private int x, y, z;
        [SerializeField, JsonRequired]
        private float tileSize = 2f;
        [SerializeField, JsonRequired]
        private float wallThickness = .1f; // (!) esto no deveria ir aqui sino

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSRepresentationData> representations = new List<LBSRepresentationData>(); //eliminar (!!!)

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSLayer> layers = new List<LBSLayer>();

        #endregion

        #region PROPERTIES

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

        [JsonIgnore]
        public float TileSize => tileSize;
        [JsonIgnore]
        public float WallThickness => wallThickness;

        #endregion

        #region METHODS

        [Obsolete("Esta funcion no se esta ocupanbdo actualemente pero se puede volver a usar a futuro")]
        public List<GameObject> RequestLevelObjects(string category)
        { 
            if(levelObjects.Find((i) => i.category == category) == null)
            {
                levelObjects.Add(new ItemCategory(category));
            }
            return levelObjects.Find((i) => i.category == category).items;

        }

        public void RemoveNullRepresentation() // (!) parche, no deberia poder añadirse nulls
        {
            var r = new List<LBSRepresentationData>();
            foreach (var rep in representations)
            {
                if (rep != null)
                    r.Add(rep);
            }
            representations = r;
        }


        /// <summary>
        /// Add a new representation, if a representation of
        /// the type delivered already exists, it overwrites it.
        /// </summary>
        /// <param name="rep"></param>
        public void AddRepresentation(LBSRepresentationData rep) 
        {
            if(rep == null)
            {
                return;
            }
            //RemoveNullRepresentation();
            // puede que necesitemos guardar reps del mismo tipo por lo que hay que revisar esta funcion denuevi (!!)
            var currentRep = representations.Find(r => r?.GetType() == rep.GetType());
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
        public T GetRepresentation<T>() where T : LBSRepresentationData
        {
            var rep = (T)representations.Find(r => (r is T));

            if(rep != null)
                return rep;

            rep = Activator.CreateInstance(typeof(T)) as T;
            representations.Add(rep);

            return rep;
        }

        public T GetRepresentation<T>(string label) where T : LBSRepresentationData
        {
            var rep = (T)representations.Find(r => (r is T && r.Label == label));

            if (rep != null)
                return rep;

            rep = Activator.CreateInstance(typeof(T), new object[] { label}) as T;
            rep.Label = label;
            representations.Add(rep);

            return rep;
        }

        #endregion
    }
}

