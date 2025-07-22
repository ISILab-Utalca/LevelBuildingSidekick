using System.Collections.Generic;
using System.Linq;
using ISILab.Extensions;
using ISILab.LBS.Components;
using ISILab.LBS.Internal;
using ISILab.LBS.Modules;
using LBS.Bundles;
using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Behaviours
{
    
    [System.Serializable]
    [RequieredModule(typeof(TileMapModule),
        typeof(ConnectedTileMapModule),
        typeof(SectorizedTileMapModule),
        typeof(ConnectedZonesModule))]
    public class SchemaBehaviour : LBSBehaviour
    {
        #region READONLY-FIELDS
        
        [JsonIgnore]
        private static List<string> connections = new List<string>() // esto puede ser remplazado despues (!)
        {
            "Empty", // for clearing a wall
            "Wall", // default wall connection 
            "Door", // within wall connection
            "Window" // within wall connection
        };
        
        #endregion

        #region FIELDS
        [JsonIgnore]
        private TileMapModule tileMap => OwnerLayer.GetModule<TileMapModule>();
        [JsonIgnore]
        private ConnectedTileMapModule tileConnections => OwnerLayer.GetModule<ConnectedTileMapModule>();
        [JsonIgnore]
        private SectorizedTileMapModule areas => OwnerLayer.GetModule<SectorizedTileMapModule>();

        [SerializeField]
        private string pressetInsideStyle = "Castle_Wooden";
        [SerializeField]
        private string pressetOutsideStyle = "Castle_Brick";
        
        #endregion

        #region META-FIELDS
        private Zone roomToSet;
        [JsonIgnore]
        public Zone RoomToSet
        {
            get => roomToSet; 
            set => roomToSet = value; 
        }
        [JsonIgnore]
        public string conectionToSet;
        #endregion

        #region PROEPRTIES
        [JsonIgnore]
        public Bundle PressetInsideStyle
        {
            get => LBSAssetsStorage.Instance.Get<Bundle>().Find(b => b.Name == pressetInsideStyle);
            set => pressetInsideStyle = value.Name;
        }

        [JsonIgnore]
        public Bundle PressetOutsideStyle
        {
            get => LBSAssetsStorage.Instance.Get<Bundle>().Find(b => b.Name == pressetOutsideStyle);
            set => pressetOutsideStyle = value.Name;
        }

        [JsonIgnore]
        public bool ValidArea => OwnerLayer.GetModule<SectorizedTileMapModule>() is null;
        
        [JsonIgnore]
        public List<Zone> Zones => areas.Zones;

        [JsonIgnore]
        public List<Zone> ZonesWithTiles => areas.ZonesWithTiles;
        
        [JsonIgnore]
        public List<LBSTile> Tiles => tileMap.Tiles;

        public static List<string> Connections => connections;

        [JsonIgnore]
        public List<Vector2Int> Directions => ISILab.Commons.Directions.Bidimencional.Edges;
        #endregion

        #region CONSTRUCTORS
        public SchemaBehaviour(VectorImage icon, string name, Color colorTint) : base(icon, name, colorTint) { }
        #endregion

        #region METHODS
        
        public override void OnGUI()
        {

        }
        
        public override void OnAttachLayer(LBSLayer layer)
        {
            OwnerLayer = layer;
        }

        public override void OnDetachLayer(LBSLayer layer)
        {
            throw new System.NotImplementedException();
        }

        public LBSTile AddTile(Vector2Int position, Zone zone)
        {
            if (tileMap.Contains(position)) return null;
                
            var tile = new LBSTile(position);//, "Tile: " + position.ToString());
            
            tileMap.AddTile(tile);
            areas.AddTile(tile, zone);
            
            RequestTilePaint(tile);
            
            return tile;
        }

        public Zone AddZone()
        {
            string prefix = "Zone: ";
            int counter = 0;
            string suffix = " (" + OwnerLayer.Name + ")";
            string name = prefix + counter;
            IEnumerable<string> names = areas.Zones.Select(z => z.ID);
            while (names.Contains(name))
            {
                counter++;
                name = prefix;
                
                if (counter < 10) name += "0" + counter;
                else   name += counter;
               
            }

            var c = new Color().RandomColorHSV();
            var zone = new Zone(name, c);

            areas.AddZone(zone);
            return zone;
        }

        public void RemoveZone(Zone zone)
        {
            var tiles = areas.GetTiles(zone);
            foreach (var tile in tiles)
            {
                RequestTileRemove(tile);
            }
            
            tileMap.RemoveTiles(tiles);
            areas.RemoveZone(zone);
        }

        public void RemoveTile(Vector2Int position)
        {
            var tile = tileMap.GetTile(position);
            
            RequestTileRemove(tile);
            
            tileMap.RemoveTile(tile);
            tileConnections.RemoveTile(tile);
            areas.RemovePair(tile);
        }

        public void RequestFullRepaint(List<LBSTile> olds, List<LBSTile> news)
        {
            olds.ForEach(t => RequestTileRemove(t));
            news.ForEach(t => RequestTilePaint(t));
        }

        public void SetConnection(LBSTile tile, int direction, string connection, bool editedByIA)
        {
            var t = tileConnections.GetPair(tile);
            t.SetConnection(direction, connection, editedByIA);
        }

        public void AddConnections(LBSTile tile, List<string> connections, List<bool> editedByIA)
        {
            tileConnections.AddPair(tile, connections, editedByIA);
        }

        public LBSTile GetTile(Vector2Int position)
        {
            return tileMap.GetTile(position);
        }

        public List<LBSTile> GetTiles(Zone zone)
        {
            return areas.GetTiles(zone);
        }

        public Rect GetBound(Zone zone)
        {
            return areas.GetBounds(zone);
        }

        public List<string> GetConnections(LBSTile tile)
        {
            var pair = tileConnections.GetPair(tile);
            return pair.Connections;
        }

        public Zone GetZone(LBSTile tile)
        {
            var pair = areas.GetPairTile(tile);
            return pair.Zone;
        }

        public Zone GetZone(Vector2 position)
        {
            return GetZone(GetTile(position.ToInt()));
        }

        public List<LBSTile> GetTileNeighbors(LBSTile tile, List<Vector2Int> dirs)
        {
            var tor = new List<LBSTile>();
            foreach (var dir in dirs)
            {
                var t = GetTile(dir + tile.Position);
                tor.Add(t);
            }
            return tor;
        }

        public void RecalculateWalls()
        {
            foreach (var tile in Tiles)
            {
                var currZone = GetZone(tile);

                var currConnects = GetConnections(tile);
                var neigs = GetTileNeighbors(tile, Directions);

                var edt = tileConnections.GetPair(tile).EditedByIA;

                for (int i = 0; i < Directions.Count; i++)
                {
                    if (!edt[i])
                        continue;

                    if (neigs[i] == null)
                    {
                        if (currConnects[i] != "Door")
                        {
                            SetConnection(tile, i, "Wall", true);
                        }
                        continue;
                    }

                    var otherZone = GetZone(neigs[i]);
                    if (otherZone.Equals(currZone))
                    {


                        SetConnection(tile, i, "Empty", true);
                    }
                    else
                    {
                        if (currConnects[i] != "Door")
                        {
                            SetConnection(tile, i, "Wall", true);
                        }
                    }
                }
            }
        }

        public override object Clone()
        {
            return new SchemaBehaviour(this.Icon, this.Name, this.ColorTint);
        }

        public override bool Equals(object obj)
        {
            var other = obj as SchemaBehaviour;

            if (other == null) return false;

            if (!this.Name.Equals(other.Name)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        #endregion
    }
}