using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace LBS.Components.TileMap
{
    [System.Serializable]
    public class PopulationTiles : LBSTile
    {
        #region FIELDS

        [SerializeField, JsonRequired, SerializeReference]
        int population;

        #endregion

        #region PROPERTIES

        public int Population { get { return population; } set { population = value; } }

        #endregion

        #region CONSTRUCTOR

        public PopulationTiles() : base()
        {
            population = 0;
        }

        public PopulationTiles(Vector2 position, string id, int sides = 4, int population = 0) : base(position, id, sides)
        {
            this.population = population;
        }

        #endregion

        #region METHODS

        public void AddPopulation(int value)
        {
            population += value;
        }

        public void RemovePopulation(int value)
        {
            population -= value;
        }

        #endregion
    }
}