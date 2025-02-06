using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization;
using ISILab.AI.Optimization.Populations;
using ISILab.AI.Optimization.Selections;
using ISILab.AI.Optimization.Terminations;
using ISILab.AI.Wrappers;
using ISILab.Commons;
using ISILab.Extensions;
using ISILab.LBS.Assistants;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using Optimization.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;
using Debug = UnityEngine.Debug;

public class BlueprintAssistant : LBSAssistant, IStep1
{
    private List<Vector2Int> Dirs => Directions.Bidimencional.Edges;

    #region META-FIELDS
    [SerializeField, JsonRequired]
    public bool visibleConstraints = false;
    [SerializeField, JsonRequired]
    public bool printClocks = false;
    #endregion

    #region PROPERTIES
    [JsonIgnore]
    public List<Zone> ZonesWhitTiles => Owner.GetModule<SectorizedTileMapModule>().ZonesWithTiles;
    [JsonIgnore]
    public TileMapModule TileMapMod => Owner.GetModule<TileMapModule>();
    [JsonIgnore]
    public SectorizedTileMapModule AreasMod => Owner.GetModule<SectorizedTileMapModule>();
    [JsonIgnore]
    public ConnectedZonesModule GraphMod => Owner.GetModule<ConnectedZonesModule>();
    [JsonIgnore]
    public ConstrainsZonesModule ConstrainsZonesMod => Owner.GetModule<ConstrainsZonesModule>();
    #endregion

    #region CONSTRUCTORS
    public BlueprintAssistant(Texture2D icon, string name) : base(icon, name)
    {
    }
    #endregion

    #region FIELDS
    [JsonIgnore, NonSerialized]
    private HillClimbing hillClimbing;
    #endregion

    public void Execute()
    {
        var clock = new Stopwatch();

        UnityEngine.Debug.Log("HillClimbing start!");
        OnStart?.Invoke();


        clock.Start();
        hillClimbing.Start();
        clock.Stop();

        var modules = (hillClimbing.BestCandidate as OptimizableModules).Modules; // FIX
        var zones = modules.GetModule<SectorizedTileMapModule>();
        RecalculateWalls(modules);

        SetDoors(modules);

        foreach (var module in modules)
        {
            var old = this.Owner.GetModule(module.ID);
            this.Owner.ReplaceModule(old, module);
        }

        Owner.Reload();

        OnTermination?.Invoke();

        UnityEngine.Debug.Log("HillClimbing finish!");
        Debug.Log(
            "Execute \n" +
            "Time: " + clock.ElapsedMilliseconds / 1000f + " s. \n" +
            "Ticks: " + clock.ElapsedTicks);

    }

    public void RecalculateWalls(List<LBSModule> layer)
    {
        var tileModule = layer.GetModule<TileMapModule>();
        var zonesMod = layer.GetModule<SectorizedTileMapModule>();
        var connection = layer.GetModule<ConnectedTileMapModule>();

        foreach (var tile in tileModule.Tiles)
        {
            var currZone = zonesMod.GetZone(tile);

            var currConnects = connection.GetConnections(tile);
            var neigs = tileModule.GetTileNeighbors(tile, Dirs);

            var edt = connection.GetPair(tile).EditedByIA;

            for (int i = 0; i < Dirs.Count; i++)
            {
                if (!edt[i])
                    continue;

                if (neigs[i] == null)
                {
                    if (currConnects[i] != "Door")
                    {
                        connection.SetConnection(tile, i, "Wall", true);
                    }
                    continue;
                }

                var otherZone = zonesMod.GetZone(neigs[i]);
                if (otherZone == currZone)
                {
                    connection.SetConnection(tile, i, "Empty", true);
                }
                else
                {
                    if (currConnects[i] != "Door")
                    {
                        connection.SetConnection(tile, i, "Wall", true);
                    }
                }
            }
        }
    }


    private void SetDoors(List<LBSModule> layer)
    {
        // Get Modules
        var tilesMod = layer.GetModule<TileMapModule>();
        var zonesMod = layer.GetModule<SectorizedTileMapModule>();
        var connectedZones = layer.GetModule<ConnectedZonesModule>();
        var connectedTiles = layer.GetModule<ConnectedTileMapModule>();

        foreach (var tile in tilesMod.Tiles)
        {
            // Get connection for each tile
            var connection = connectedTiles.GetConnections(tile);
            for (int i = 0; i < connection.Count; i++)
            {
                // set all DOORS to WALLS
                if (connection[i] == "Door")
                    connection[i] = "Wall";
            }
        }

        foreach (var edge in connectedZones.Edges)
        {
            // Get zones
            var zone1 = edge.First;
            var zone2 = edge.Second;

            // Get tiles form both zones
            var tilesZ1 = zonesMod.GetTiles(zone1);
            var tilesZ2 = zonesMod.GetTiles(zone2);

            // Cheack if both zones contain tiles
            if (tilesZ1.Count <= 0 || tilesZ2.Count <= 0)
                continue;

            // Create list of posibles pair tiles
            var pairs = new List<(LBSTile, LBSTile)>();
            foreach (var t1 in tilesZ1)
            {
                foreach (var t2 in tilesZ2)
                {
                    // Calculate dist between tiles
                    var dist = (t1.Position - t2.Position).magnitude;

                    // If tiles are neightbors
                    if (dist == 1)
                    {
                        pairs.Add((t1, t2));
                    }
                }
            }

            // Cheack if contians posibles pair
            if (pairs.Count <= 0)
                continue;

            // Select a random pair
            var selc = pairs.Random();

            // Get direction indexs
            var dir = selc.Item1.Position - selc.Item2.Position;
            var indx1 = Directions.Bidimencional.Edges.IndexOf(-dir);
            var indx2 = Directions.Bidimencional.Edges.IndexOf(dir);

            // Get direction pairs
            var p1 = connectedTiles.GetPair(selc.Item1);
            var p2 = connectedTiles.GetPair(selc.Item2);

            // Set connections
            p1.SetConnection(indx1, "Door", true);
            p2.SetConnection(indx2, "Door", true);
        }
    }

    public void RecalculateConstraint()
    {
        var zoneModule = Owner.GetModule<SectorizedTileMapModule>();
        var zones = zoneModule.Zones;

        ConstrainsZonesMod.Clear();

        foreach (var zone in zones)
        {
            var bounds = zoneModule.GetBounds(zone);

            var min = new Vector2(bounds.width - 2, bounds.height - 2);

            if (min.x < 1)
                min.x = 1;
            if (min.y < 1)
                min.y = 1;

            var max = new Vector2(bounds.width + 2, bounds.height + 2);

            ConstrainsZonesMod.AddPair(zone, min, max);

        }
    }


    public override void OnAttachLayer(LBSLayer layer)
    {
        // Call base method
        base.OnAttachLayer(layer);

        // Get modules
        var zonesMod = layer.GetModule<SectorizedTileMapModule>();

        // Set event
        zonesMod.OnAddZone += (module, zone) =>
        {
            ConstrainsZonesMod.RecalculateConstraint(zonesMod.Zones);
        };
        zonesMod.OnRemoveZone += (module, zone) =>
        {
            ConstrainsZonesMod.RecalculateConstraint(zonesMod.Zones);
            GraphMod.RemoveEdges(zone);

        };

        zonesMod.OnRemovePair += (module, tile) =>
        {
            if (tile == null) return;

            if (!module.ZonesWithTiles.Contains(tile.Zone))
            {
                GraphMod.RemoveEdges(tile.Zone);
            }

        };

        // Set first population
        var adam = new OptimizableModules(layer.Modules);


        var selection = new EliteSelection();
        var termination = new FitnessStagnationTermination(1);
        var evaluator = new WeightedEvaluator(new System.Tuple<IEvaluator, float>[]
        {
            new System.Tuple<IEvaluator, float> (new AdjacenciesEvaluator(layer), 4f),
            new System.Tuple<IEvaluator, float> (new AreasEvaluator(layer), 0.15f),
            new System.Tuple<IEvaluator, float> (new EmptySpaceEvaluator(layer), 0.35f),
            new System.Tuple<IEvaluator, float> (new RoomCutEvaluator(layer), 1f),
        });

        var population = new Population(1, 100, adam); // agregar parametros

        hillClimbing = new HillClimbing(population, evaluator, selection, GetNeighbors, termination);

    }

    public override object Clone()
    {
        throw new System.NotImplementedException();
    }

    public void ConnectZones(Zone first, Zone second)
    {
        throw new System.NotImplementedException();
    }

    public Zone GetZone(LBSTile tile)
    {
        throw new System.NotImplementedException();
    }

    public Zone GetZone(Vector2Int position)
    {
        throw new System.NotImplementedException();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="adam"></param>
    /// <returns></returns>
    public List<IOptimizable> GetNeighbors(IOptimizable adam)//, Graph graph)
    {
        return GetNeigPARALLEL(adam);//, graph); 
    }


    private List<IOptimizable> GetNeigPARALLEL(IOptimizable adam)// Graph graph)
    {
        var map = adam as Map;

        // Init empty neighbors group
        var neighbors = new List<IOptimizable>();

        // Parallel.ForEach to iterate over zones in parallel
        var tempZ = new List<IOptimizable>[map.rooms.Count];
        var rooms = map.rooms.ToList();
        Parallel.For(0, map.rooms.Count(), i =>
        {
            var (key, room) = rooms[i];

            // Get Schema walls for zones
            var walls = room.GetWalls();

            // Create a new Optimizable for each wall
            var temp = new IOptimizable[walls.Count];
            Parallel.For(0, walls.Count, w =>
            {
                var wall = walls[w];

                temp[w] = GetNeightByAdditive(adam, key, wall.Item1.ToList(), wall.Item2);

            });

            // Create a new Optimizable for each wall with negative DIR
            var temp2 = new IOptimizable[walls.Count];
            Parallel.For(0, walls.Count, w =>
            {
                var wall = walls[w];

                temp2[w] = GetNeightByAdditive(adam, key, wall.Item1.ToList(), -wall.Item2);
            });

            // Create optimizable for each wall removing this wall
            var temp3 = new IOptimizable[walls.Count];
            Parallel.For(0, walls.Count, w =>
            {
                var wall = walls[w];

                temp3[w] = GetNeightByExclusion(adam, wall.Item1.ToList(), wall.Item2);
            });

            // Create optimizable moving zones
            var temp4 = new IOptimizable[Dirs.Count];
            Parallel.For(0, Dirs.Count, d =>
            {
                var dir = Dirs[d];

                temp4[d] = GetNeightByMove(adam, key, dir);
            });

            tempZ[i] = temp.Concat(temp2).Concat(temp3).Concat(temp4).ToList();
        });

        neighbors = tempZ.SelectMany(x => x).ToList();

        return neighbors;
    }


    private IOptimizable GetNeightByAdditive(IOptimizable original, string zoneName, List<Vector2Int> walls, Vector2Int dir)
    {
        // Generate clone
        var map = (original as Map).Clone() as Map;

        // add wall positions
        var toAdd = walls.Select(x => x + dir).ToList();
        map.SetRoomTiles(toAdd, zoneName);

        // return neigthbour
        return map;
    }

    private IOptimizable GetNeightByExclusion(IOptimizable original, List<Vector2Int> tiles, Vector2Int dir)
    {
        // Generate clone
        var map = (original as Map).Clone() as Map;

        var room = map.rooms.First().Value;

        if (room.Bounds.width <= 1 && (dir == Vector2.up || dir == Vector2.down))
            return map;
        
        if (room.Bounds.height <= 1 && (dir == Vector2.right || dir == Vector2.left))
            return map;

        foreach (var pos in tiles)
        {
            room.tiles.Remove(pos);
        }

        return map;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="original"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    private IOptimizable GetNeightByMove(IOptimizable original, string zoneName, Vector2Int dir)
    {
        // Generate clone
        var map = (original as Map).Clone() as Map;

        map.rooms.TryGetValue(zoneName, out var room);

        foreach (var (pos, tile) in room.tiles)
        {
            var nPos = pos + dir;

            foreach (var (key, otherRoom) in map.rooms)
            {
                if (otherRoom == room)
                    continue;

                otherRoom.tiles.TryGetValue(nPos, out var otherTile);
                if (otherTile != null)
                {
                    otherRoom.tiles.Remove(nPos);
                }
            }
        }

        map.MoveArea(room, dir);
        zonesMod.MoveArea(room, dir);

        // return neigthbour
        return map;
    }

}