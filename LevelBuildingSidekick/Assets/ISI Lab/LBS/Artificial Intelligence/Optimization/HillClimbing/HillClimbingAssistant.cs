using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.AI;
using UnityEngine.UIElements;
using LBS.Components;
using LBS.Components.TileMap;
using System.Linq;
using ISILab.AI.Optimization.Selections;
using ISILab.AI.Optimization.Populations;
using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization.Terminations;
using ISILab.AI.Optimization;
using LBS.Components.Graph;
using LBS.Tools.Transformer;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Newtonsoft.Json;
using LBS.Assisstants;
using UnityEditor.Graphs;
using UnityEngine.Tilemaps;
using System.Reflection;

[System.Serializable]
[RequieredModule(typeof(TileMapModule),
    typeof(ConnectedTileMapModule),
    typeof(SectorizedTileMapModule),
    typeof(ConnectedTileMapModule),
    typeof(ConstrainsZonesModule))]
//[RequieredModule(typeof(LBSRoomGraph), typeof(LBSSchema))]
public class HillClimbingAssistant : LBSAssistant
{
    private List<Vector2Int> Dirs => Directions.Bidimencional.Edges;

    #region FIELDS

    [JsonIgnore, NonSerialized]
    private HillClimbing hillClimbing;

    [JsonIgnore, NonSerialized]
    private Stopwatch clock = new Stopwatch();

    [JsonIgnore, NonSerialized]
    private LBSLayer layer;

    [JsonIgnore]
    private ConnectedZonesModule graph;
    [JsonIgnore]
    private SectorizedTileMapModule areas;
    [JsonIgnore]
    private TileMapModule tileMap;
    [JsonIgnore]
    private ConstrainsZonesModule constrainsZones;
    #endregion

    #region PROPERTIES
    public object Constraints => constrainsZones.Constraints;
    #endregion

    #region CONSTRUCTORS
    public HillClimbingAssistant(Texture2D icon, string name) : base(icon, name)
    {
    }
    #endregion

    #region METHODS
    public override void Execute()
    {
        clock = new Stopwatch();

        UnityEngine.Debug.Log("HillClimbing start!");
        OnStart?.Invoke();

        //OnAdd(Owner);

        clock.Start();
        hillClimbing.Start();
        clock.Stop();

        var modules = (hillClimbing.BestCandidate as OptimizableModules).Modules;
        var zones = modules.GetModule<SectorizedTileMapModule>();
        RecalculateWalls(modules);

        SetDoors(modules);

        foreach(var module in modules)
        {
            var old = this.Owner.GetModule(module.ID);
            this.Owner.ReplaceModule(old, module);
        }

        OnTermination?.Invoke();
        UnityEngine.Debug.Log("HillClimbing finish!");
    }

    public void RecalculateWalls(List<LBSModule> layer)
    {
        var tileModule = layer.GetModule<TileMapModule>();
        var zones = layer.GetModule<SectorizedTileMapModule>();
        var connection = layer.GetModule<ConnectedTileMapModule>();

        foreach (var tile in tileModule.Tiles)
        {
            var currZone = GetZone(tile);

            var currConnects = connection.GetConnections(tile);
            var neigs = tileModule.GetTileNeighbors(tile, Dirs);// GetTileNeighbors(tile, Directions);

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

                var otherZone = GetZone(neigs[i]);
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
        var zones = layer.GetModule<SectorizedTileMapModule>();
        var connectedZones = layer.GetModule<ConnectedZonesModule>();
        var connectedTiles = layer.GetModule<ConnectedTileMapModule>();

        foreach (var area in zones.Zones)
        {
            foreach (var tile in zones.GetTiles(area))
            {
                var cTile = tile as ConnectedTile;
                for (int i = 0; i < cTile.Connections.Length; i++)
                {
                    if (cTile.Connections[i].Contains("Door"))
                    {
                        cTile.SetConnection("Wall", i);
                    }
                }
            }
        }

        var edges = connectedZones.Edges;
        for (int i = 0; i < edges.Count; i++)
        {
            var edge = edges[i];

            var zone1 = edge.First;
            var zone2 = edge.Second;

            var tilesZ1 = zones.GetTiles(zone1);
            var tilesZ2 = zones.GetTiles(zone2);
            if (tilesZ1.Count <= 0 || tilesZ2.Count <= 0) // signiofica que una de las dos areas desaparecio y no deberia aporta, de hecho podria ser negativo (!)
                continue;

            var pairs = new List<Tuple<ConnectedTile, ConnectedTile>>();
            foreach (var t1 in tilesZ1)
            {
                foreach (var t2 in tilesZ2)
                {
                    var dist = Vector2Int.Distance(t1.Position, t2.Position);
                    if (dist <= 1.1f)
                    {
                        pairs.Add(new Tuple<ConnectedTile, ConnectedTile>(t1 as ConnectedTile, t2 as ConnectedTile));
                    }
                }
            }

            if (pairs.Count <= 0)
                continue;

            var selc = pairs.Random();
            // var selc = pairs[UnityEngine.Random.Range(0, pairs.Count() - 1)];

            var dir = selc.Item1.Position - selc.Item2.Position;

            selc.Item1.SetConnection("Door", -dir);
            selc.Item2.SetConnection("Door", dir);

        }


    }



    public override void OnAdd(LBSLayer layer)
    {
        // Set Owner
        Owner = layer;

        // Modules
        var modules = layer.Modules;

        // Set Module references
        tileMap = Owner.GetModule<TileMapModule>();
        graph = Owner.GetModule<ConnectedZonesModule>();
        constrainsZones = Owner.GetModule<ConstrainsZonesModule>();


        // Set constraint
        var zoneModule = layer.GetModule<SectorizedTileMapModule>();
        constrainsZones.RecalculateConstraint(zoneModule.Zones);


        var adam = new OptimizableModules(modules);

        var selection = new EliteSelection();
        var termination = new FitnessStagnationTermination(1); // agregar termination de maximo local
        var evaluator = new WeightedEvaluator(new System.Tuple<IEvaluator, float>[] //agregar parametros necesarios a las clases de evaluaci�n
        {
            new System.Tuple<IEvaluator, float> (new AdjacenciesEvaluator(layer), 0.4f),
            new System.Tuple<IEvaluator, float> (new AreasEvaluator(layer), 0.15f),
            new System.Tuple<IEvaluator, float> (new EmptySpaceEvaluator(layer), 0.35f),
            new System.Tuple<IEvaluator, float> (new RoomCutEvaluator(layer), 1f),
            //new System.Tuple<IEvaluator, float> (new StretchEvaluator(), 0.1f),
        });
        var population = new Population(1, 100, adam); // agregar parametros

        hillClimbing = new HillClimbing(population, evaluator, selection, GetNeighbors, termination); // asignar Adam

    }

    private StochasticHillClimbing InitStochastic(LBSLayer layer)
    {
        // Set Owner
        Owner = layer;

        // Set Module references
        tileMap = Owner.GetModule<TileMapModule>();
        graph = Owner.GetModule<ConnectedZonesModule>();
        constrainsZones = Owner.GetModule<ConstrainsZonesModule>();

        var adam = new OptimizableModules(new List<LBSModule>(layer.Modules));//(layer.Clone() as LBSLayer);

        var selection = new EliteSelection();
        var termination = new FitnessStagnationTermination(1); // agregar termination de maximo local
        var evaluator = new WeightedEvaluator(new System.Tuple<IEvaluator, float>[] //agregar parametros necesarios a las clases de evaluaci�n
        {
            new System.Tuple<IEvaluator, float> (new AdjacenciesEvaluator(layer), 0.4f),
            new System.Tuple<IEvaluator, float> (new AreasEvaluator(layer), 0.15f),
            new System.Tuple<IEvaluator, float> (new EmptySpaceEvaluator(layer), 0.35f),
            new System.Tuple<IEvaluator, float> (new RoomCutEvaluator(layer), 1f),
            //new System.Tuple<IEvaluator, float> (new StretchEvaluator(), 0.1f),
        });
        var population = new Population(1, 100, adam); // agregar parametros

        return new StochasticHillClimbing(population, evaluator, selection, GetNeighbors, termination); // asignar Adam
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="adam"></param>
    /// <returns></returns>
    public List<IOptimizable> GetNeighbors(IOptimizable adam)
    {
        var modules = (adam as OptimizableModules).Modules;
        var zones = modules.GetModule<SectorizedTileMapModule>();

        // Init empty neigh group
        var neighbours = new List<IOptimizable>();
        foreach (var zone in zones.ZonesWithTiles)
        {
            // Get Schema walls for zones
            var vWalls = zones.GetVerticalWalls(zone);
            var hWalls = zones.GetHorizontalWalls(zone);
            var walls = vWalls.Concat(hWalls).ToList();

            // Create a new Optimizable for each wall
            foreach (var wall in walls)
            {
                var neigth = GetNeigthByAdditive(adam, zone.ID, wall.Tiles, wall.Dir);
                neighbours.Add(neigth);
            }

            // Create a new Optimizable for each wall with negative DIR
            foreach (var wall in walls)
            {
                var neigth = GetNeigthByAdditive(adam, zone.ID, wall.Tiles, - wall.Dir);
                neighbours.Add(neigth);
            }

            // Create optimizable for each wall removing this wall
            foreach(var wall in walls)
            {
                var neigth = GetNeigthByExclusion(adam,wall.Tiles);
                neighbours.Add(neigth);
            }

            // Create optimizalbe moving zones
            foreach( var dir in Dirs)
            {
                var neigth = GetNeigthByMove(adam,zone.ID, dir);
                neighbours.Add(neigth);
            }
        }

        return neighbours;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="original"></param>
    /// <param name="zoneName"></param>
    /// <param name="walls"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    private IOptimizable GetNeigthByAdditive(IOptimizable original, string zoneName, List<Vector2Int> walls, Vector2Int dir)
    {
        // Generate clone
        var modules = (original as OptimizableModules).Modules.Clone();

        // Get zone
        var zone = modules.GetModule<SectorizedTileMapModule>().GetZone(zoneName);

        // Get relative modules
        var zonesMod = modules.GetModule<SectorizedTileMapModule>();
        var tilesMod = modules.GetModule<TileMapModule>();

        foreach (var pos in walls)
        {
            // create new tile
            var nTile = new LBSTile(pos + dir);
            tilesMod.AddTile(nTile);
            zonesMod.AddTile(nTile, zone);
        }

        // return neigthbour
        return new OptimizableModules(modules);
    }

    /// <summary>
    /// XXXXXX
    /// </summary>
    /// <param name="original"></param>
    /// <param name="walls"></param>
    /// <returns></returns>
    private IOptimizable GetNeigthByExclusion(IOptimizable original, List<Vector2Int> walls)
    {
        // Generate clone
        var modules = (original as OptimizableModules).Modules.Clone();

        // Get relative modules
        var zonesMod = modules.GetModule<SectorizedTileMapModule>();
        var tilesMod = modules.GetModule<TileMapModule>();

        foreach (var pos in walls)
        {
            // Remove tile
            var tile = tilesMod.GetTile(pos);
            tilesMod.RemoveTile(tile);
            zonesMod.RemoveTile(tile);
        }

        // return neigthbour
        return new OptimizableModules(modules);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="original"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    private IOptimizable GetNeigthByMove(IOptimizable original, string zoneName, Vector2Int dir)
    {
        // Generate clone
        var modules = (original as OptimizableModules).Modules.Clone();

        // Get zone
        var zone = modules.GetModule<SectorizedTileMapModule>().GetZone(zoneName);

        // Get relative modules
        var zonesMod = modules.GetModule<SectorizedTileMapModule>();
        var tilesMod = modules.GetModule<TileMapModule>();

        zonesMod.MoveArea(zone, dir);

        // return neigthbour
        return new OptimizableModules(modules);
    }

    public void RemoveZoneConnection(Vector2Int position, float delta)
    {
        ZoneEdge edge = graph.GetEdge(position, delta);
        graph.RemoveEdge(edge);
        throw new System.NotImplementedException();
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

    public LBSTile GetTile(Vector2Int position)
    {
        return tileMap.GetTile(position);
    }

    public void ConnectZones(Zone first, Zone second)
    {
        graph.AddEdge(first, second);
    }

    public override object Clone()
    {
        return new HillClimbingAssistant(this.Icon, this.Name);
    }

    /*
    public void RunExperiment()
    { 
        var elitistLog = SchemaHCLog.CreateInstance("SchemaHCLog") as SchemaHCLog;
        elitistLog.name = Owner.Name + "Elitist Log";
        clock = new Stopwatch();

        //Init(Owner);
        hillClimbing.OnGenerationRan += () => {
            var log = new HCLog();
            log.time = clock.ElapsedMilliseconds;
            log.evaluationTime = hillClimbing.Elog;
            log.neighborTime = hillClimbing.Nlog;
            log.neighborCount = hillClimbing.NNlog;
            log.bestFitness = hillClimbing.BestCandidate.Fitness;
            elitistLog.log.Add(log);
        };
        hillClimbing.Selection = new EliteSelection();

        clock.Restart();
        hillClimbing.Start();
        clock.Stop();
        
        AssetDatabase.CreateAsset(elitistLog, "Assets/Experiments/" + Owner.Name + "ElitistLog.asset" );
        AssetDatabase.SaveAssets();

        var x = (hillClimbing.BestCandidate as OptimizableSchema).Schema;
        CalculateConnections.Operate(x);

        SetDoors(x, Owner.GetModule<LBSRoomGraph>());

        Owner.SetModule(x, x.Key);
        OnTermination?.Invoke();

    }
    */

    /*
    public void RunStochasticExperiment()
    {
        var sch = Owner.GetModule<LBSSchema>().Clone() as LBSSchema;
        for(int i = 0; i < 10; i++)
        {
            Owner.SetModule<LBSSchema>(sch);
            var hc = InitStochastic(Owner);
            var Log = SchemaHCLog.CreateInstance("SchemaHCLog") as SchemaHCLog;
            Log.name = Owner.Name + "S" + (i + 1) + "Stochastic Log";
            clock = new Stopwatch();

            //Init(Owner);
            hc.OnGenerationRan += () => {
                var log = new HCLog();
                log.time = clock.ElapsedMilliseconds;
                log.evaluationTime = hc.Elog;
                log.neighborTime = hc.Nlog;
                log.neighborCount = hc.NNlog;
                log.bestFitness = hc.BestCandidate.Fitness;
                Log.log.Add(log);
            };
            //hc.Selection = new StochasticElitistSelection();

            clock.Restart();
            hc.Start();
            clock.Stop();

            AssetDatabase.CreateAsset(Log, "Assets/Experiments/" + Owner.Name + "S" + (i + 1) +  "StochasticLog.asset");
            AssetDatabase.SaveAssets();

            var x = (hc.BestCandidate as OptimizableSchema).Schema;
            CalculateConnections.Operate(x);

            SetDoors(x, Owner.GetModule<LBSRoomGraph>());

            Owner.SetModule(x, x.Key);
            OnTermination?.Invoke();
        }
        

    }
    */
    #endregion
}
