using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelBuildingSidekick
{
    [System.Serializable]
    //[CreateAssetMenu(menuName = "LevelBuildingSidekick/Data/Level Data")]
    public class LevelData : Data
    {
        // public string levelName;
        public List<string> tags;
        public List<ItemCategory> levelObjects;

        public int x, y, z;        

        //[SerializeReference]
        public List<LBSRepesentationData> representations = new List<LBSRepesentationData>();

        [JsonIgnore]
        public override Type ControllerType => throw new NotImplementedException();

        [JsonIgnore]
        public Vector3 Size
        {
            get
            {
                return new Vector3(x, y, z);
            }
            set
            {
                x = (int)value.x;
                y = (int)value.y;
                z = (int)value.z;
            }
        }

        public List<GameObject> RequestLevelObjects(string category)
        { 
            if(levelObjects.Find((i) => i.category == category) == null)
            {
                levelObjects.Add(new ItemCategory(category));
            }
            return levelObjects.Find((i) => i.category == category).items;
        }

        public void AddRepresentation(LBSRepesentationData rep) 
        {
            var currentRep = representations.Find(r => r.GetType() == rep.GetType());
            if(currentRep != null)
            {
                representations.Remove(currentRep);
            }
            representations.Add(rep);
        }

        // En un futuro vamos a agrupar representaciones de nivel del mismo tipo, 
        //en ese caso debería haber un getRepresentation group o algo similar que 
        //devuelva el grupo completo de representaciones de tipo T
        public T GetRepresentation<T>() where T : LBSRepesentationData
        {
            foreach(var r in representations)
            {
                if(r is T)
                {
                    return r as T;
                }
            }

            T representation = Activator.CreateInstance(typeof(T)) as T;
            representations.Add(representation);

            return representation;
        }
    }
}

