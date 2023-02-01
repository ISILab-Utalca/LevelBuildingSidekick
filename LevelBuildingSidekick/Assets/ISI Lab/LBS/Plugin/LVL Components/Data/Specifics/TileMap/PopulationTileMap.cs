using System.Collections.Generic;
using System.Linq;

namespace LBS.Components.TileMap
{
    [System.Serializable]
    public class PopulationTileMap<T> : AreaTileMap<T> where T : PopulationTiledArea
    {
        #region FIELDS

        public Dictionary<string, int> PopulationData { get; set; }

        #endregion

        #region CONSTRUCTOR

        public PopulationTileMap() : base()
        {
            PopulationData = new Dictionary<string, int>();
        }

        public PopulationTileMap(List<TiledArea> areas, string key) : base(areas, key)
        {
            PopulationData = new Dictionary<string, int>();
        }

        #endregion

        #region METHODS

        public void AddPopulation(string id, int population)
        {
            PopulationData[id] = population;
        }

        public void RemovePopulation(string id)
        {
            PopulationData.Remove(id);
        }

        public int GetPopulation(string id)
        {
            return PopulationData[id];
        }

        public override object Clone()
        {
            var ptm = new PopulationTileMap<T>();
            var nAreas = areas.Select(a => a.Clone() as T).ToList();
            foreach (var nArea in nAreas)
            {
                ptm.AddArea(nArea);
            }
            ptm.PopulationData = new Dictionary<string, int>(PopulationData);

            return ptm;
        }

        #endregion

    }

}