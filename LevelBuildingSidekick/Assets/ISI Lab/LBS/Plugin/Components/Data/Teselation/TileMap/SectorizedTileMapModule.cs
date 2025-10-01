using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISILab.Commons;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Components;
using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Modules
{
    [System.Serializable]
    public class SectorizedTileMapModule : LBSModule, ISelectable
    {
        #region FIELDS
        [SerializeField, JsonRequired, SerializeReference]
        private List<Zone> zones = new List<Zone>();

        [SerializeField, JsonRequired, SerializeReference]
        private List<TileZonePair> pairs = new List<TileZonePair>();

        private int[,] zonesProximity;
        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public List<TileZonePair> PairTiles => new List<TileZonePair>(pairs);

        [JsonIgnore]
        public List<Zone> Zones => new(zones);

        [JsonIgnore]
        public List<Zone> ZonesWithTiles => pairs.Select(t => t.Zone).Distinct().ToList();

        public int[,] ZonesProximity { get => zonesProximity; set => zonesProximity = value; }

        public List<Zone> SelectedZones { get; set; } = new List<Zone>();

        [JsonIgnore]
        private List<Vector2Int> Dirs => Directions.Bidimencional.Edges;

        [JsonIgnore]
        private List<Vector2Int> DirsDiag => Directions.Bidimencional.Diagonals;
        #endregion

        #region EVENTS
        public event Action<SectorizedTileMapModule, Zone> OnAddZone;
        public event Action<SectorizedTileMapModule, Zone> OnRemoveZone;
        public event Action<SectorizedTileMapModule, TileZonePair> OnAddPair;
        public event Action<SectorizedTileMapModule, TileZonePair> OnRemovePair;
        #endregion

        #region CONSTRUCTORS
        public SectorizedTileMapModule()
        {

        }

        public SectorizedTileMapModule(List<Zone> zones, List<TileZonePair> tiles, string id = "TilesToAreaModule") : base(id)
        {
            //Debug.Log("Constructed Sectorized Tilemap Module.");
            foreach (var zone in zones)
            {
                AddZone(zone);
            }

            foreach (var t in tiles)
            {
                AddPair(t);
            }
        }
        #endregion

        #region METHODS
        public void MoveArea(Zone zone, Vector2Int dir)
        {
            var tiles = GetTiles(zone);

            var old = new List<LBSTile>();

            //var poss = new List<Vector2Int>();
            foreach (var t in tiles)
            {
                old.Add(t.Clone() as LBSTile);
                t.Position += new Vector2Int(dir.x, dir.y);
                //poss.Add(t.Position + dir);
            }

            OnChanged?.Invoke(this, old.Cast<object>().ToList(), tiles.Cast<object>().ToList());

            RecalcPivotZone(zone, tiles);
        }

        private void RecalcPivotZone(Zone zone)
        {
            var tiles = GetTiles(zone);

            var pos = tiles.GetBounds();

            zone.Pivot = pos.center;
        }

        private void RecalcPivotZone(Zone zone, List<LBSTile> tiles)
        {
            var pos = tiles.GetBounds();

            zone.Pivot = pos.center;
        }

        public void AddPair(TileZonePair pair)
        {
            var current = GetPairTile(pair.Tile.Position);
            if (current != null)
            {
                pairs.Remove(current);
                OnRemovePair?.Invoke(this, current);
            }
            pairs.Add(pair);

            OnChanged?.Invoke(this, null, new List<object>() { pair });
            OnAddPair?.Invoke(this, pair);

            RecalcPivotZone(pair.Zone);
        }

        public void AddTile(LBSTile tile, Zone zone)
        {
            var pair = new TileZonePair(tile, zone);
            OnChanged?.Invoke(this, null, new List<object>() { pair });
            AddPair(pair);
        }

        public void AddZone(Zone zone)
        {
            zones.Add(zone);
            OnChanged?.Invoke(this, null, new List<object>() { zone });
            OnAddZone?.Invoke(this, zone);
        }

        public Zone GetZone(LBSTile tile)
        {
            var p = GetPairTile(tile);
            if (p == null)
                return null;
            return p.Zone;
        }

        public void RemoveZone(Zone zone)
        {
            zones.Remove(zone);
            OnRemoveZone?.Invoke(this, zone);

            var toRemove = new List<TileZonePair>();
            foreach (var pair in pairs)
            {
                if (pair.Zone == zone)
                    toRemove.Add(pair);
            }
            OnChanged?.Invoke(this, toRemove.Cast<object>().ToList(), null);

            foreach (var pair in toRemove)
            {
                pairs.Remove(pair);
                OnRemovePair?.Invoke(this, pair);
            }
        }

        public TileZonePair GetPairTile(LBSTile tile)
        {
            if (pairs.Count <= 0)
                return null;

            foreach (var pair in pairs)
            {
                if (pair.Tile.Equals(tile))
                    return pair;
            }
            return null;
            //return pairs.Find(t => t.Tile.Equals(tile));
        }

        public TileZonePair GetPairTile(Vector2Int pos)
        {
            return pairs.Find(t => t.Tile.Position == pos);
        }

        public List<LBSTile> GetTiles(Zone zone)
        {
            var tiles = new List<LBSTile>();
            foreach (var pair in pairs)
            {
                if (pair.Zone.Equals(zone))
                {
                    tiles.Add(pair.Tile);
                }
            }
            return tiles;
        }

        public Rect GetZoneBounds(Zone zone, out List<LBSTile> tiles)
        {
            tiles = GetTiles(zone);
            return tiles.GetBounds();
        }

        public bool IsRectangular(Zone zone, Rect bounds, List<LBSTile> tiles)
        {
            List<LBSTile> rectTiles = new List<LBSTile>(tiles);
            for(int i = (int)bounds.x; i < bounds.width + bounds.x; i++)
            {
                for(int j = (int)bounds.y; j < bounds.height + bounds.y; j++)
                {
                    if(!rectTiles.Remove(rectTiles.Find(t => t.x == i && t.y == j)))
                        return false;
                    //if(rectTiles.RemoveAll(t => t.x == i && t.y == j) < 1)
                    //    return false;
                }
            }
            return true;
        }

        public void RemovePair(LBSTile tile)
        {
            var t = GetPairTile(tile);
            pairs.Remove(t);
            OnChanged?.Invoke(this, new List<object>() { t }, null);
            OnRemovePair?.Invoke(this, t);
        }

        public void RemovePair(int index)
        {
            var pair = pairs[index];
            pairs.RemoveAt(index);
            OnChanged?.Invoke(this, new List<object>() { pair }, null);
            OnRemovePair?.Invoke(this, pair);
        }

        public bool Contains(LBSTile tile)
        {
            if (pairs.Count <= 0)
                return false;
            return pairs.Any(t => t.Tile.Equals(tile));
        }

        public bool Contains(Vector2Int pos)
        {
            if (pairs.Count <= 0)
                return false;
            return pairs.Any(t => t.Tile.Position == pos);
        }

        public Rect GetBounds(Zone zone)
        {
            return GetTiles(zone).GetBounds();
        }

        public Vector2 ZoneCentroid(Zone zone)
        {
            return GetBounds(zone).center;
        }

        public void RecalculateZonesProximity() => RecalculateZonesProximity(GetBounds());

        public void RecalculateZonesProximity(Rect selection)
        {
            if(OwnerLayer == null) return;

            var tilemap = OwnerLayer.GetModule<TileMapModule>();
            if (tilemap == null) return;
            var connectedTM = OwnerLayer.GetModule<ConnectedTileMapModule>();
            if(connectedTM == null) return;

            var zonesToCalc = new List<Zone>(ZonesWithTiles);
            for(int i = 0; i < zonesToCalc.Count; i++)
            {
                if(!selection.Overlaps(GetBounds(zonesToCalc[i])))
                {
                    zonesToCalc.RemoveAt(i);
                    i--;
                }
            }
            SelectedZones = new List<Zone>(zonesToCalc);

            int size = zonesToCalc.Count;
            zonesProximity = new int[size, size];
            // Fill with 0 and infinite distances
            for(int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    zonesProximity[i, j] = i == j ? 0 : int.MaxValue;
                }
            }

            // Find neighbours and set distances to 1
            Dictionary<Zone, List<LBSTile>> zoneTiles = zonesToCalc.Select(z => KeyValuePair.Create(z, GetTiles(z))).ToDictionary(x => x.Key, x => x.Value);
            for(int i = 0; i < size; i++)
            {
                List<LBSTile> tilesWithDoors = zoneTiles[zonesToCalc[i]].FindAll(t => selection.Contains(t.Position) && connectedTM.GetConnections(t).Any(c => c.Equals("Door")));
                foreach(LBSTile t in tilesWithDoors)
                {
                    foreach(Vector2Int dir in Dirs)
                    {
                        if (!connectedTM.GetConnections(t)[Dirs.IndexOf(dir)].Equals("Door"))
                            continue;

                        LBSTile neigh = tilemap.GetTileNeighbor(t, dir);
                        if (neigh == null || !selection.Contains(neigh.Position) || !connectedTM.GetConnections(neigh)[Dirs.IndexOf(-dir)].Equals("Door"))
                            continue;

                        Zone otherZone = GetZone(neigh);
                        if(otherZone == null || otherZone.Equals(zonesToCalc[i]))
                            continue;

                        for(int j = 0; j < size; j++)
                        {
                            if (zonesProximity[i, j] != int.MaxValue)
                                continue;
                            if(otherZone.Equals(zonesToCalc[j]))
                            {
                                zonesProximity[i, j] = zonesProximity[j, i] = 1;
                            }
                        }
                    }
                }
            }

            for(int k = 1; k < size - 1; k++) // Find all distances equal to k + 1
            {
                for (int i = 0; i < size - 1; i++) 
                {
                    for(int j = 0; j < size; j++)
                    {
                        if (zonesProximity[i, j] == k) // Find zones at distance of k
                        {
                            for(int l = 0; l < size; l++) // Search for l neighbour zones of j
                            {
                                if (zonesProximity[j, l] == 1) // If j and l are neighbours
                                {
                                    // Set distance from i to l as k + 1 unless previously assigned distance is lower
                                    zonesProximity[i, l] = zonesProximity[l, i] = Mathf.Min(zonesProximity[i, l], k + 1);
                                }
                            }
                        }
                    }
                }
            }
            string log = "";
            for(int i = 0; i < size; i++)
            {
                log += "[";
                for(int j = 0; j < size; j++)
                {
                    log += zonesProximity[i, j];
                    log += j < size - 1 ? ", " : "";
                }
                log += "]\n";
            }
            //Debug.Log("ZONES PROXIMITY RECALCULATED\n"+log);
        }

        public void BuildFromExterior(ConnectedTileMapModule connectedTM, ConnectedTileMapModule zoneConnected)
        {
            if (connectedTM == null)
            {
                Debug.LogError("Could not interpret zones. Connected Tile Map was null.");
                return;
            }

            List<string> floorTags = OwnerLayer.GetBehaviour<ExteriorBehaviour>().NavigableTags;

            if (floorTags == null || floorTags.Count == 0)
            {
                Debug.LogError("Cannot build zones. Floor tags were null or empty.");
                return;
            }

            switch (connectedTM.GridType)
            {
                case ConnectedTileMapModule.ConnectedTileType.EdgeBased:
                    BuildFromEdgeBasedExterior(connectedTM, floorTags);
                    break;
                case ConnectedTileMapModule.ConnectedTileType.VertexBased:
                    BuildFromVertexBasedExterior(connectedTM, floorTags, zoneConnected);
                    break;
            }
        }

        private void BuildFromEdgeBasedExterior(ConnectedTileMapModule connectedTM, List<string> floorTags)
        {
            throw new NotImplementedException("It's currently not possible to interpret zones from Edge-based Exterior Layers");
        }

        private void BuildFromVertexBasedExterior(ConnectedTileMapModule connectedTM, List<string> floorTags, ConnectedTileMapModule zoneConnected)
        {
            Clear();

            var tilemap = connectedTM.OwnerLayer.GetModule<TileMapModule>();
            List<TileConnectionsPair> tiles = connectedTM.Pairs; // Todos los tiles de Connected Module
            List<TileGroup> tileGroups = new(); // Grupos de tiles que conformaran las zonas

            List<Vector2Int> dirs = Directions.Bidimencional.Edges;

            HashSet<TileConnectionsPair> toRemove = new();

            while (tiles.Count > 0) // Buscar los tiles que puedan constituir zonas
            {
                // Elige un tile y lo marca como revisado
                TileConnectionsPair current = tiles[0];
                tiles.Remove(current);

                // Si no califica como tile navegable
                if (!current.IsFloor(floorTags))
                {
                    // Se debe eliminar del nuevo modulo y revisar el siguiente tile
                    toRemove.Add(current);
                    continue;
                }

                // Si califica como navegable, se crea a partir de ese tile un nuevo grupo que representara una zona
                tileGroups.Insert(0, new TileGroup(current.Tile));

                // Se buscaran todos los tiles que pertenecen a la misma zona
                List<TileConnectionsPair> found = new List<TileConnectionsPair>() { current };

                while (found.Count > 0)
                {
                    current = found[0];
                    found.Remove(current);
                    Vector2Int currentPos = current.Tile.Position;

                    // Por cada tile vecino
                    foreach (Vector2Int dir in dirs)
                    {
                        TileConnectionsPair neighbourTile = connectedTM.Pairs.Find(p => p.Tile.Position == currentPos + dir);
                        // Si ya fue revisado previamente, pasa al siguiente
                        if (!tiles.Remove(neighbourTile)) continue;
                        // Se verifica tambien si es navegable, y se elimina del modulo de no ser el caso
                        if (!neighbourTile.IsFloor(floorTags))
                        {
                            toRemove.Add(neighbourTile);
                            continue;
                        }
                        // Se agrega a la lista para buscar mas vecinos
                        found.Add(neighbourTile);
                        // Finalmente se agrega al grupo que se convertira en una zona
                        tileGroups[0].originalTiles.Add(neighbourTile.Tile);
                    }
                }
            }

            // Elimina del nuevo modulo todos los tiles marcados como no transitables
            foreach(TileConnectionsPair tile in toRemove)
                zoneConnected.RemoveTile(tile);

            // Por cada grupo
            foreach (TileGroup tileGroup in tileGroups)
            {
                // Se crea una zona temporal
                Color zoneColor = new Color().RandomColorHSV();
                Zone zone = new Zone(zoneColor.ToString(), zoneColor);
                AddZone(zone);
                tileGroup.zone = zone;

                List<Vector2Int> positions = tileGroup.originalTiles.Select(t => t.Position).ToList();

                // (tile, direccion)
                var annexed = new List<(LBSTile, int)>();
                var paths = new List<(LBSTile, int)>();
                    
                // Por cada tile
                foreach (LBSTile tile in tileGroup.originalTiles)
                {
                    // Se agrega el tile a la zona
                    AddPair(new TileZonePair(tile, zone));

                    for(int i = 0; i < dirs.Count; i++)
                    {
                        // Se comprueba si los vecinos forman parte del mismo grupo
                        // De ser el caso, se setea como vacio la conexion con el vecino
                        // En caso contrario, se setea como muro
                        Vector2Int neighPos = tile.Position + dirs[i];

                        string toSet = "Empty";
                        if (positions.Contains(neighPos))
                        {
                            toSet = "Empty";
                        }
                        else
                        {
                            // Revisar si se extiende el tile
                            LBSTile t = tilemap.GetTile(neighPos);
                                //connectedTM.Pairs.Find(p => p.Tile.Position.Equals(neighPos)).Tile;
                            if(t is not null)
                            {
                                int count = CountConnectionFloorTags(tile, i);
                                if (count == 0) toSet = "Wall";
                                if (count == 1 && !annexed.Contains((t, i)) && !paths.Remove((t, i))) paths.Add((t, i)); // Un tile anexado nunca es camino  &&  Si hay dos sospechas de camino, es una esquina y no un camino.
                                if (count == 2)
                                {
                                    annexed.Add((t, i));
                                    zoneConnected.AddPair(t, new List<string>() { "Empty", "Empty", "Empty", "Empty", }, new List<bool>(){ false, false, false, false, });
                                }
                            }
                            else
                            {
                                toSet = "Wall";
                            }
                            
                        }
                        zoneConnected.SetConnection(tile, i, toSet, false);
                    }
                }
                tileGroup.annexedTiles.AddRange(annexed);
                
                paths.RemoveAll(t_d => annexed.Select(anx => anx.Item1).Contains(t_d.Item1));
                tileGroup.pathTiles.AddRange(paths);
                string log = "Sospechas de caminos:\n";
                foreach(var path in paths)
                {
                    log += $"Tile: {path.Item1} | Direccion: {path.Item2}\n";
                }
                Debug.Log(log);
            }
            foreach (TileGroup tileGroup in tileGroups)
            {
                // Agregar tiles anexados
                foreach((LBSTile, int) annexedTile in tileGroup.annexedTiles)
                {
                    AddPair(new TileZonePair(annexedTile.Item1, tileGroup.zone));
                    for(int i = 0; i < dirs.Count; i++)
                    {
                        // Siempre habra muro hacia adelante desde el origen del tile anexado
                        if(i == annexedTile.Item2)
                        {
                            zoneConnected.SetConnection(annexedTile.Item1, i, "Wall", false);
                            continue;
                        }
                        // Siempre dejar vacio en la direccion de origen
                        if (i == annexedTile.Item2) continue;

                        // Lo demas es para comprobar los costados

                        // Si el tile vecino no existe en zoneConnected, es que no pertenece a ninguna zona. Colocar muro
                        LBSTile neighbour = zoneConnected.GetPair(tilemap.GetTileNeighbor(annexedTile.Item1, dirs[i]))?.Tile;
                        if (neighbour == null)
                        {
                            zoneConnected.SetConnection(annexedTile.Item1, i, "Wall", false);
                            continue;
                        }
                        // Si el tile vecino es de la zona de origen, dejar vacio
                        if (tileGroup.ExtendedTiles.Contains(neighbour)) continue;
                        // Revisar si el tile vecino corresponde a un tile no anexado de otra zona (Creo que a este punto es la unica posibilidad)
                        //foreach(TileGroup otherGroup in tileGroups)
                        //{
                        //    if (otherGroup.Equals(tileGroup)) continue;
                        //    if (otherGroup.originalTiles.Contains(neighbour))
                        //    {
                        //        zoneConnected.SetConnection(annexedTile.Item1, i, "Door", false);
                        //        zoneConnected.SetConnection(neighbour, (i+2)%4, "Door", false);
                        //        break;
                        //    }
                        //}
                        zoneConnected.SetConnection(annexedTile.Item1, i, "Door", false);
                        zoneConnected.SetConnection(neighbour, (i+2)%4, "Door", false);
                    }
                }

                // Comprobar caminos
                List<(LBSTile, int)> pathTiles = tileGroup.pathTiles;
                for(int i = 0; i < pathTiles.Count; i++)
                {
                    List<LBSTile> newPath = new List<LBSTile>();
                    bool validPath = true;
                    bool newZoneReached = false;
                    LBSTile current = pathTiles[i].Item1;
                    int direction = pathTiles[i].Item2;
                    do
                    {
                        //if(CountConnectionFloorTags(current, direction) != 1)
                        // TODO: comparar si los siguientes tiles tienen el mismo tipo de conexion.
                        //      No basta solo contar los floor tags
                    } while (validPath && !newZoneReached);
                }
            }

            Print();
            zoneConnected.Print();

            return; /// END OF METHOD

            // Local functions

            int CountConnectionFloorTags(LBSTile tile, int dir)
            {
                List<string> conns = connectedTM.GetConnections(tile);
                (int, int) inds = (dir == 0 ? 3 : dir - 1, dir);
                int count = 0;
                if (floorTags.Contains(conns[inds.Item1])) count++;
                if (floorTags.Contains(conns[inds.Item2])) count++;

                return count;
            }
        }

        class TileGroup
        {
            public List<LBSTile> originalTiles = new();
            public Zone zone;
            public List<(LBSTile, int)> annexedTiles = new();
            public List<(LBSTile, int)> pathTiles = new();

            public List<LBSTile> ExtendedTiles
            {
                get
                {
                    var ret = new List<LBSTile>(originalTiles);
                    ret.AddRange(new List<LBSTile>(annexedTiles.Select(t => t.Item1)));
                    return ret;
                }
            } 

            public TileGroup(LBSTile first)
            {
                originalTiles.Add(first);
            }

            public override bool Equals(object obj)
            {
                if(obj is not TileGroup other) return false;
                return Equals(zone, other.zone);
            }

            public override int GetHashCode()
            {
                return zone.GetHashCode();
            }
        }

        private List<bool> CheckNeighborhood(Vector2Int position, List<Vector2> directions)
        {
            var neighborhood = new List<bool>();
            for (int i = 0; i < directions.Count; i++)
            {
                var otherPos = position + directions[i];
                neighborhood.Add(GetPairTile(otherPos.ToInt()) != null);
            }
            return neighborhood;
        }

        private List<Zone> CheckZonesInNeighborhood(Vector2Int position, List<Vector2Int> directions)
        {
            var neighborhood = new List<Zone>();
            for (int i = 0; i < directions.Count; i++)
            {
                var otherPos = position + directions[i];
                var t = GetPairTile(otherPos);
                if (t == null)
                    neighborhood.Add(null);
                else
                    neighborhood.Add(t.Zone);
            }
            return neighborhood;
        }

        private int NeighborhoodValue(Vector2Int position, List<Vector2Int> directions) // (!) el nombre es malisimo mejorar, esta tambien es de la clase de las tablas del gabo
        {
            var value = 0;
            var t = GetPairTile(position);
            if (t == null)
                return -1;
            var zones = CheckZonesInNeighborhood(position, directions);
            for (int i = 0; i < directions.Count; i++)
            {
                var otherPos = position + directions[i];
                if (zones[i] == null || !zones[i].Equals(t.Zone))
                {
                    value += Mathf.RoundToInt(Mathf.Pow(2, i));
                }
            }

            return value;
        }

        public bool IsConvexCorner(Vector2 pos, List<Vector2Int> directions)
        {
            var s = NeighborhoodValue(pos.ToInt(), directions);
            if (s != 0)
            {
                if (s % 3 == 0 || s == 7 || s == 11 || s == 13 || s == 14)
                    return true;
            }
            return false;
        }

        public bool IsConcaveCorner(Vector2 pos, List<Vector2Int> directions)
        {
            var s = NeighborhoodValue(pos.ToInt(), directions);
            if (s == 1 || s == 2 || s == 4 || s == 8)
                return true;
            return false;
        }

        public bool IsWall(Vector2 pos, List<Vector2Int> directions)
        {
            var s = NeighborhoodValue(pos.ToInt(), directions);
            if (s == 1 || s == 2 || s == 4 || s == 8)
                return true;
            return false;

        }

        internal List<LBSTile> GetConvexCorners(Zone zone) // (??)  esto solo funciona para "4 conected", deberia estar en una clase aparte?, si en la clase de las tablas del gabo
        {
            var corners = new List<LBSTile>();
            foreach (var t in pairs)
            {
                if (t.Zone != zone)
                    continue;

                if (IsConvexCorner(t.Tile.Position, Dirs))
                {
                    corners.Add(t.Tile);
                    //corners.Add(t.Clone() as LBSTile);
                }
            }
            return corners;
        }

        internal List<LBSTile> GetConcaveCorners(Zone zone) // (!) Tambien es de la clase de las tablas del gabo 
        {

            var corners = new List<LBSTile>();

            foreach (var t in pairs)
            {
                if (t.Zone != zone)
                    continue;

                if (!IsConcaveCorner(t.Tile.Position, DirsDiag))
                    continue;

                for (int i = 0; i < Dirs.Count; i++)
                {
                    var other = GetPairTile(t.Tile.Position + Dirs[i]);
                    if (other == null)
                        continue;
                    if (IsWall(other.Tile.Position, Dirs))
                    {
                        corners.Add(other.Tile);
                        //corners.Add(other.Clone() as LBSTile);
                    }
                }
            }
            return corners;
        }

        internal List<WallData> GetVerticalWalls(Zone zone) // (!) Tambien es de la clase de las tablas del gabo 
        {
            var walls = new List<WallData>();

            var convexCorners = GetConvexCorners(zone);
            var allCorners = GetConcaveCorners(zone);
            allCorners.AddRange(convexCorners);

            foreach (var current in convexCorners)
            {
                LBSTile other = null;
                int lessDist = int.MaxValue;
                foreach (var candidate in allCorners)
                {
                    if (current == candidate)
                        continue;

                    var tile = current;
                    if (tile.Position.x - candidate.Position.x != 0)
                        continue;

                    var dist = Mathf.Abs(tile.Position.y - candidate.Position.y);
                    if (dist < lessDist)
                    {
                        lessDist = dist;
                        other = candidate;
                    }
                }

                if (other == null)
                    other = current;

                if (walls.Any(w => (w.First == other.Position) && (w.Last == current.Position)))
                    continue;

                var wallTiles = new List<Vector2Int>();
                var oth = other.Position;
                var end = Mathf.Max(current.Position.y, oth.y);
                var start = Mathf.Min(current.Position.y, oth.y);
                for (int i = 0; i <= end - start; i++)
                {
                    wallTiles.Add(new Vector2Int(current.Position.x, start + i));
                }
                var dir = (current.Position.x >= ZoneCentroid(GetZone(current)).x) ? Vector2Int.right : Vector2Int.left;

                var wall = new WallData(this.id, dir, wallTiles);
                walls.Add(wall);
            }
            return walls;
        }

        internal List<WallData> GetHorizontalWalls(Zone zone)
        {
            var walls = new List<WallData>();

            var convexCorners = GetConvexCorners(zone);
            var allCorners = GetConcaveCorners(zone);
            allCorners.AddRange(convexCorners);

            foreach (var current in convexCorners)
            {
                LBSTile other = null;
                int lessDist = int.MaxValue;
                foreach (var candidate in allCorners)
                {
                    if (current == candidate)
                        continue;

                    var tile = current;
                    if (tile.Position.y - candidate.Position.y != 0)
                        continue;

                    var dist = Mathf.Abs(tile.Position.x - candidate.Position.x);
                    if (dist < lessDist)
                    {
                        lessDist = dist;
                        other = candidate;
                    }
                }

                if (other == null)
                    other = current;

                if (walls.Any(w => (w.First == other.Position) && (w.Last == current.Position)))
                    continue;

                var wallTiles = new List<Vector2Int>();
                var oth = other.Position;
                var end = Mathf.Max(current.Position.x, oth.x);
                var start = Mathf.Min(current.Position.x, oth.x);
                for (int i = 0; i <= end - start; i++)
                {
                    wallTiles.Add(new Vector2Int(start + i, current.Position.y));
                }
                var dir = (current.Position.y >= ZoneCentroid(GetZone(current)).y) ? Vector2Int.up : Vector2Int.down;
                var wall = new WallData(this.id, dir, wallTiles);
                walls.Add(wall);
            }
            return walls;
        }

        public float GetRoomDistance(Zone r1, Zone r2, List<LBSTile> tiles1, List<LBSTile> tiles2) // O2 - manhattan
        {
            var lessDist = float.MaxValue;

            //var tiles1 = GetTiles(r1);
            //var tiles2 = GetTiles(r2);

            //var tileWalls1 = room1.GetWalls().SelectMany(x => x.Tiles).ToList();
            //var tileWalls2 = room2.GetWalls().SelectMany(x => x.Tiles).ToList();

            for (int i = 0; i < tiles1.Count; i++)
            {
                for (int j = 0; j < tiles2.Count; j++)
                {
                    //var v = tiles1[i].Position - tiles2[j].Position;
                    //var dist = Mathf.Abs(v.x) + Mathf.Abs(v.y);
                    var dist = Vector2.SqrMagnitude(tiles1[i].Position - tiles2[j].Position);
                    if (dist <= lessDist)
                    {
                        lessDist = dist;
                    }
                }
            }

            return lessDist;
        }

        public List<WallData> GetWalls(Zone zone)
        {
            var horizontal = GetHorizontalWalls(zone);
            var vertical = GetVerticalWalls(zone);

            return horizontal.Concat(vertical).ToList();
        }

        public Zone GetZone(string name)
        {
            foreach (var zone in zones)
            {
                if (zone.ID == name)
                    return zone;
            }
            return null;
        }

        public Zone GetZone(Vector2Int position)
        {
            foreach (var pair in pairs)
            {
                if (pair.Tile.Position == position)
                {
                    return pair.Zone;
                }
            }

            return null;
        }

        public List<object> GetSelected(Vector2Int position)
        {
            var pos = OwnerLayer.ToFixedPosition(position);
            var r = new List<object>();
            var zone = GetZone(pos);

            if (zone != null)
            {
                r.Add(zone);
            }

            return r;
        }

        public void UpdateZonePositions()
        {
            // Initialize auxiliary lists
            var positions = new List<Vector2Int>[zones.Count];
            for (int i = 0; i < zones.Count; i++)
            {
                positions[i] = new List<Vector2Int>();
            }
            
            // Save positions in auxiliary lists
            foreach (var tile in PairTiles)
            {
                int index = Zones.IndexOf(tile.Zone);
                positions[index].Add(tile.Tile.Position);
            }
            
            // Replace positions in zones
            for (int i = 0; i < zones.Count; i++)
            {
                zones[i].ClearPositions();
                zones[i].AddPositionRange(positions[i]);
            }
        }
        
        public override Rect GetBounds()
        {
            if (pairs.Count == 0)
            {
                return default(Rect);
            }

            return pairs.Select(t => t.Tile).GetBounds();
        }

        public override bool IsEmpty()
        {
            return pairs.Count <= 0;
        }

        public override void Clear()
        {
            pairs.Clear();
            while (zones.Count > 0)
            {
                RemoveZone(zones[0]);
            }
        }

        public override object Clone()
        {
            var zones = this.zones.Select(z => CloneRefs.Get(z)).Cast<Zone>().ToList();
            var pairs = this.pairs.Select(t => t.Clone()).Cast<TileZonePair>().ToList();

            var clone = new SectorizedTileMapModule(zones, pairs, this.id);
            clone.ZonesProximity = this.ZonesProximity;
            clone.SelectedZones = this.SelectedZones;
            return clone;
        }

        public override void Print()
        {
            string msg = "";
            msg += "Type: " + GetType() + "\n";
            msg += "Hash code: " + GetHashCode() + "\n";
            msg += "ID: " + ID + "\n";
            msg += "\n";
            foreach (var zone in zones)
            {
                msg += zone.ID + "\n";
                foreach (var tile in GetTiles(zone))
                {
                    msg += "  " + tile.Position + "\n";
                }
            }
            Debug.Log(msg);
        }

        public override void Rewrite(LBSModule other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            var other = obj as SectorizedTileMapModule;

            if (other == null) return false;

            var zCount = other.zones.Count;

            if (zCount != this.zones.Count) return false;

            for (int i = 0; i < zCount; i++)
            {
                var z1 = this.zones[i];
                var z2 = other.zones[i];

                if (!z1.ExactlyEquals(z2)/*Equals(z2 as object)*/) return false;
            }

            var pCount = other.pairs.Count;

            if (pCount != this.pairs.Count) return false;

            for (int i = 0; i < pCount; i++)
            {
                var p1 = this.pairs[i];
                var p2 = other.pairs[i];

                if (!p1.Equals(p2)) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }

    [System.Serializable]
    public class TileZonePair : ICloneable
    {
        #region FIELDS
        [SerializeField, JsonRequired, SerializeReference]
        private LBSTile tile;

        [SerializeField, JsonRequired, SerializeReference]
        private Zone zone;
        #endregion

        #region PROEPRTIES
        [JsonIgnore]
        public LBSTile Tile => tile;

        [JsonIgnore]
        public Zone Zone
        {
            get => zone;
            set => zone = value;
        }
        #endregion

        #region CONSTRUCTORS
        public TileZonePair(LBSTile tile, Zone zone)
        {
            this.tile = tile;
            this.zone = zone;
        }
        #endregion

        #region METHODS
        public object Clone()
        {
            var cTile = CloneRefs.Get(tile) as LBSTile;
            var cZone = CloneRefs.Get(zone) as Zone;

            return new TileZonePair(cTile, cZone);
        }

        public override bool Equals(object obj)
        {
            var other = obj as TileZonePair;

            if (other == null) return false;

            if (!this.tile.Equals(other.tile)) return false;

            if (!this.zone.ExactlyEquals(other.zone)/*Equals(other.zone as object)*/) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(zone.GetHashCode());
        }

        public override string ToString()
        {
            return $"({tile}) : ({zone})";
        }
        #endregion

    }
}