using ISILab.Commons;
using ISILab.Extensions;
using ISILab.LBS.Components;
using LBS.Components.TileMap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ISILab.LBS.Modules
{
    public static class ModuleBuilder
    {
        public static void BuildFromExterior(this SectorizedTileMapModule sectorTM, ConnectedTileMapModule connectedTM, ConnectedTileMapModule zoneConnected, List<string> floorTags)
        {
            if (connectedTM is null)
            {
                Debug.LogError("Could not interpret zones. Connected Tile Map was null.");
                return;
            }

            if (floorTags is null || floorTags.Count == 0)
            {
                Debug.LogError("Cannot build zones. Navigable tags were null or empty.");
                return;
            }

            switch (connectedTM.GridType)
            {
                case ConnectedTileMapModule.ConnectedTileType.EdgeBased:
                    sectorTM.BuildFromEdgeBasedExterior(connectedTM, floorTags);
                    break;
                case ConnectedTileMapModule.ConnectedTileType.VertexBased:
                    sectorTM.BuildFromVertexBasedExterior(connectedTM, floorTags, zoneConnected);
                    break;
            }
        }

        private static void BuildFromEdgeBasedExterior(this SectorizedTileMapModule sectorTM, ConnectedTileMapModule connectedTM, List<string> floorTags)
        {
            throw new NotImplementedException("It's currently not possible to interpret zones from Edge-based Exterior Layers");
        }

        private static void BuildFromVertexBasedExterior(this SectorizedTileMapModule sectorTM, ConnectedTileMapModule connectedTM, List<string> floorTags, ConnectedTileMapModule zoneConnected)
        {
            sectorTM.Clear();

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
            foreach (TileConnectionsPair tile in toRemove)
                zoneConnected.RemoveTile(tile);

            var allAnnexed = new List<TileGroup.AnnexedTile>();
            var allPaths = new List<TileGroup.PathTile>();

            // Por cada grupo
            foreach (TileGroup tileGroup in tileGroups)
            {
                // Se crea una zona temporal
                Color zoneColor = new Color().RandomColorHSV();
                Zone zone = new Zone(zoneColor.ToString(), zoneColor);
                sectorTM.AddZone(zone);
                tileGroup.zone = zone;

                List<Vector2Int> positions = tileGroup.originalTiles.Select(t => t.Position).ToList();

                // (tile, direccion, conexiones)
                var annexed = new List<TileGroup.AnnexedTile>();
                var paths = new List<TileGroup.PathTile>();

                // Por cada tile
                foreach (LBSTile tile in tileGroup.originalTiles)
                {
                    // Se agrega el tile a la zona
                    sectorTM.AddPair(new TileZonePair(tile, zone));

                    for (int i = 0; i < dirs.Count; i++)
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
                            if (t is not null)
                            {
                                List<string> connections = connectedTM.GetConnections(t);
                                var extra = new TileGroup.ExtraTile(t, i, connections);
                                var anx = new TileGroup.AnnexedTile(extra);
                                var path = new TileGroup.PathTile(extra);
                                int count = CountConnectionFloorTags(extra);
                                if (count == 0) toSet = "Wall";
                                if (count == 1 && !allAnnexed.Contains(anx))
                                {
                                    int? d = paths.Find(p => p.Equals(path))?.direction;
                                    if (!paths.Remove(path))
                                        paths.Add(path); // Un tile anexado nunca es camino  &&  Si hay dos sospechas de camino, es una esquina y no un camino.
                                    else
                                    {
                                        toSet = "Wall";
                                        LBSTile other = tilemap.GetTileNeighbor(t, dirs[(d.Value + 2) % 4]);
                                        zoneConnected.SetConnection(other, d.Value, "Wall", false);
                                    }
                                }
                                if (count == 2)
                                {
                                    annexed.Add(anx);
                                    paths.RemoveAll(p => p.Equals(anx));
                                    zoneConnected.AddPair(t, new List<string>() { "Empty", "Empty", "Empty", "Empty", }, new List<bool>() { false, false, false, false, });
                                }
                            }
                            else
                            {
                                toSet = "Door";
                            }
                        }
                        zoneConnected.SetConnection(tile, i, toSet, false);
                    }
                }
                tileGroup.annexedTiles.AddRange(annexed);
                allAnnexed.AddRange(annexed);

                var annexedTiles = annexed.Select(anx => anx.tile);
                paths.RemoveAll(path =>
                {
                    TileConnectionsPair origin = zoneConnected.GetPair(path.tile.Position - dirs[path.direction]);
                    origin.SetConnection(path.direction, "Wall", false);
                    return annexedTiles.Contains(path.tile);
                });
                tileGroup.pathTiles.AddRange(paths);
                allPaths.AddRange(paths);
                string log = "Sospechas de caminos:\n";
                foreach (var path in paths)
                {
                    log += $"Tile: {path.tile} | Direccion: {path.direction}\n";
                }
                //Debug.Log(log);
            }

            List<LBSTile> duplicatedPaths = new();
            foreach (TileGroup tileGroup in tileGroups)
            {
                // Agregar tiles anexados
                foreach (TileGroup.AnnexedTile annexedTile in tileGroup.annexedTiles)
                {
                    sectorTM.AddPair(new TileZonePair(annexedTile.tile, tileGroup.zone));
                    for (int i = 0; i < dirs.Count; i++)
                    {
                        // Siempre habra muro hacia adelante desde el origen del tile anexado
                        if (i == annexedTile.direction)
                        {
                            zoneConnected.SetConnection(annexedTile.tile, i, "Wall", false);
                            continue;
                        }
                        // Siempre dejar vacio en la direccion de origen
                        if ((i + 2) % 4 == annexedTile.direction) continue;

                        // Lo demas es para comprobar los costados

                        // Si el tile vecino no existe en zoneConnected, es que no pertenece a ninguna zona. Colocar muro
                        LBSTile neighbour = zoneConnected.GetPair(tilemap.GetTileNeighbor(annexedTile.tile, dirs[i]))?.Tile;
                        if (neighbour == null)
                        {
                            zoneConnected.SetConnection(annexedTile.tile, i, "Wall", false);
                            continue;
                        }
                        // Si el tile vecino es de la zona de origen, dejar vacio
                        if (tileGroup.ExtendedTiles.Contains(neighbour))
                        {
                            zoneConnected.SetConnection(neighbour, (i + 2) % 4, "Empty", false);
                            continue;
                        }
                        // Revisar si el tile vecino corresponde a un tile no anexado de otra zona (Creo que a este punto es la unica posibilidad)
                        zoneConnected.SetConnection(annexedTile.tile, i, "Door", false);
                        zoneConnected.SetConnection(neighbour, (i + 2) % 4, "Door", false);
                    }
                }

                // Comprobar caminos
                List<TileGroup.PathTile> pathTiles = tileGroup.pathTiles;
                //for(int i = 0; i < pathTiles.Count; i++)
                foreach (TileGroup.PathTile pathTile in tileGroup.pathTiles)
                {
                    if (duplicatedPaths.Contains(pathTile.tile)) continue;

                    (string, string) tags = pathTile.tags;
                    List<LBSTile> newPath = new List<LBSTile>();// { pathTiles[i].tile };

                    LBSTile current = pathTile.tile;
                    int direction = pathTile.direction;
                    bool newZoneReached = false;
                    bool validPath = !IsDoubleCorner(current, out List<string> conns);
                    while (validPath && !newZoneReached)
                    {
                        newPath.Add(current);
                        current = tilemap.GetTileNeighbor(current, dirs[direction]);
                        TileConnectionsPair existentTile = zoneConnected.GetPair(current);
                        if (existentTile is not null && !tileGroup.ExtendedTiles.Contains(existentTile.Tile))
                        {
                            newZoneReached = true;
                        }
                        validPath = current is not null && tags.Equals(GetOriginTags(current, direction));
                    }

                    if (validPath)
                    {
                        Zone newZone = new Zone("", new Color().RandomColorHSV());
                        sectorTM.AddZone(newZone);
                        for (int i = 0; i < newPath.Count; i++)
                        {
                            sectorTM.AddPair(new TileZonePair(newPath[i], newZone));

                            List<string> zoneConns = new() { "Empty", "Empty", "Empty", "Empty", };
                            if (i == newPath.Count - 1)
                            {
                                duplicatedPaths.Add(newPath[i]); // The path won't be checked again from the other end.
                                zoneConns[0] = "Door";
                                zoneConnected.SetConnection(current, (direction + 2) % 4, "Door", false);
                            }
                            if (i == 0)
                            {
                                zoneConns[2] = "Door";
                                TileConnectionsPair origin = zoneConnected.GetPair(newPath[i].Position - dirs[direction]);
                                origin.SetConnection(direction, "Door", false);
                            }
                            //zoneConns[floorTags.Contains(conns[direction]) ? 3 : 1] = "Wall";
                            zoneConns[1] = zoneConns[3] = "Wall";
                            zoneConnected.AddPair(newPath[i], zoneConns.Rotate(direction), new List<bool>() { false, false, false, false, });
                        }
                    }
                    else
                    {
                        // Clean up. Colocar muros 
                        TileConnectionsPair origin = zoneConnected.GetPair(pathTile.tile.Position - dirs[direction]);
                        origin.SetConnection(direction, "Wall", false);
                    }
                }
            }

            //sectorTM.Print();
            //zoneConnected.Print();

            return; /// END OF METHOD ///

            // Local functions

            int CountConnectionFloorTags(TileGroup.ExtraTile extraTile)
            {
                List<string> conns = connectedTM.GetConnections(extraTile.tile);
                int dir = extraTile.direction;
                //(int, int) inds = (dir == 0 ? 3 : dir - 1, dir);
                (int, int) inds = ((dir + 1) % 4, (dir + 2) % 4);
                int count = 0;
                if (floorTags.Contains(conns[inds.Item1])) count++;
                if (floorTags.Contains(conns[inds.Item2])) count++;

                return count;
            }

            (string, string) GetOriginTags(LBSTile tile, int dir, List<string> conns = null)
            {
                conns ??= connectedTM.GetConnections(tile);
                (int, int) inds = ((dir + 1) % 4, (dir + 2) % 4);
                return (conns[inds.Item1], conns[inds.Item2]);
            }

            bool IsDoubleCorner(LBSTile tile, out List<string> conns)
            {
                conns = connectedTM.GetConnections(tile);
                bool firstIsFloor = floorTags.Contains(conns[0]);
                bool secondIsFloor = floorTags.Contains(conns[1]);
                return (firstIsFloor != secondIsFloor) && conns[0].Equals(conns[2]) && conns[1].Equals(conns[3]);
            }
        }
    }

    class TileGroup
    {
        public class ExtraTile
        {
            public LBSTile tile;
            public int direction;
            public List<string> connections;

            public ExtraTile(LBSTile tile, int direction, List<string> connections)
            {
                this.tile = tile;
                this.direction = direction;
                this.connections = connections;
            }

            public ExtraTile(ExtraTile original) : this(original.tile, original.direction, original.connections) { }

            public override bool Equals(object obj)
            {
                if (obj is not ExtraTile other) return false;
                return Equals(tile, other.tile);
            }

            public override int GetHashCode() => tile.GetHashCode();

            public override string ToString() => $"{tile} | Direction: {direction} | {connections}";
        }

        public class AnnexedTile : ExtraTile
        {
            public AnnexedTile(LBSTile tile, int direction, List<string> connections) : base(tile, direction, connections) { }

            public AnnexedTile(ExtraTile original) : base(original) { }
        }

        public class PathTile : ExtraTile
        {
            public (string, string) tags;

            public PathTile(LBSTile tile, int direction, List<string> connections) : base(tile, direction, connections)
            {
                tags = (connections[(direction + 1) % 4], connections[(direction + 2) % 4]);
            }

            public PathTile(ExtraTile original) : base(original)
            {
                tags = (original.connections[(original.direction + 1) % 4], original.connections[(original.direction + 2) % 4]);
            }
        }

        public List<LBSTile> originalTiles = new();
        public Zone zone;
        public List<AnnexedTile> annexedTiles = new();
        public List<PathTile> pathTiles = new();

        public List<LBSTile> ExtendedTiles
        {
            get
            {
                var ret = new List<LBSTile>(originalTiles);
                ret.AddRange(new List<LBSTile>(annexedTiles.Select(t => t.tile)));
                return ret;
            }
        }

        public TileGroup(LBSTile first)
        {
            originalTiles.Add(first);
        }

        public override bool Equals(object obj)
        {
            if (obj is not TileGroup other) return false;
            return Equals(zone, other.zone);
        }

        public override int GetHashCode() => zone.GetHashCode();
    }
}
