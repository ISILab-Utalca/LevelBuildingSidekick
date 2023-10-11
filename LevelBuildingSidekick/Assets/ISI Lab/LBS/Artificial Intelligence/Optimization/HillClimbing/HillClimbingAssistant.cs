using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components;
using LBS.Components.TileMap;
using System.Linq;
using ISILab.AI.Optimization.Selections;
using ISILab.AI.Optimization.Populations;
using Commons.Optimization.Evaluator;
using ISILab.AI.Optimization.Terminations;
using ISILab.AI.Optimization;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Newtonsoft.Json;
using LBS.Assisstants;


[System.Serializable]
[RequieredModule(typeof(TileMapModule),
    typeof(ConnectedTileMapModule),
    typeof(SectorizedTileMapModule),
    typeof(ConnectedZonesModule),
    typeof(ConstrainsZonesModule))]
//[RequieredModule(typeof(LBSRoomGraph), typeof(LBSSchema))]
public class HillClimbingAssistant : LBSAssistant
{
    private List<Vector2Int> Dirs => Directions.Bidimencional.Edges;

    #region META-FIELDS
    [SerializeField, JsonRequired]
    public bool visibleConstraints = false;
    #endregion

    #region FIELDS
    [JsonIgnore, NonSerialized]
    private HillClimbing hillClimbing;

    [JsonIgnore, NonSerialized]
    private Stopwatch clock = new Stopwatch();

    /*
    [JsonIgnore]
    private ConnectedZonesModule graph;
    [JsonIgnore]
    private SectorizedTileMapModule areas;
    [JsonIgnore]
    private TileMapModule tileMap;
    [JsonIgnore]
    private ConstrainsZonesModule constrainsZones;
    */
    [JsonIgnore, NonSerialized]
    private LBSLayer layer;
    #endregion

    #region PROPERTIES
    [JsonIgnore]
    public List<Zone> ZonesWhitTiles => Owner.GetModule<SectorizedTileMapModule>().ZonesWithTiles;
    public TileMapModule TileMapMod => Owner.GetModule<TileMapModule>();
    public SectorizedTileMapModule AreasMod => Owner.GetModule<SectorizedTileMapModule>();
    public ConnectedZonesModule GraphMod => Owner.GetModule<ConnectedZonesModule>();
    public ConstrainsZonesModule ConstrainsZonesMod => Owner.GetModule<ConstrainsZonesModule>();
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

        foreach (var zone in ZonesWhitTiles)
        {
            if(ConstrainsZonesMod.GetLimits(zone) == null)
            {
                var bounds = AreasMod.GetBounds(zone);
                ConstrainsZonesMod.AddPair(zone, bounds.size - Vector2.one, bounds.size + Vector2.one);
            }
        }


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

        Owner.Reload();

        OnTermination?.Invoke();
        UnityEngine.Debug.Log("HillClimbing finish!");
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
                    if(dist == 1)
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

    public List<ZoneEdge> GetEdges()
    {
        return GraphMod.Edges;
    }

    public List<LBSTile> GetTiles(Zone zone)
    {
        return AreasMod.GetTiles(zone);
    }

    public void RecalculateConstraint()
    { 
        var zoneModule = Owner.GetModule<SectorizedTileMapModule>();
        var zones = zoneModule.Zones;

        ConstrainsZonesMod.Clear();

        foreach( var zone in zones ) 
        {
            var bounds = zoneModule.GetBounds(zone);

            var min = new Vector2(bounds.width - 2, bounds.height - 2);

            if(min.x < 1)
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
        zonesMod.OnAddZone += (module, zone) => {
            ConstrainsZonesMod.RecalculateConstraint(zonesMod.Zones);
        };
        zonesMod.OnRemoveZone += (module, zone) => { 
            ConstrainsZonesMod.RecalculateConstraint(zonesMod.Zones);
            GraphMod.RemoveEdges(zone);

        };

        // Set constraint
        // ConstrainsZonesMod.RecalculateConstraint(zonesMod.Zones);

        // Set first population
        var adam = new OptimizableModules(layer.Modules);


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
        //Debug.Log("z: " + zones.ZonesWithTiles.Count);
        foreach (var zone in zones.ZonesWithTiles)
        {
            // Get Schema walls for zones
            var vWalls = zones.GetVerticalWalls(zone);
            var hWalls = zones.GetHorizontalWalls(zone);
            var walls = vWalls.Concat(hWalls).ToList();

            //Debug.Log("w: " + walls.Count);

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
        var connectMod = modules.GetModule<ConnectedTileMapModule>();


        foreach (var pos in walls)
        {
            // create new tile
            var nTile = new LBSTile(pos + dir);
            tilesMod.AddTile(nTile);
            zonesMod.AddTile(nTile, zone);
            connectMod.AddPair(nTile, new List<string>() { "", "", "", "" }, new List<bool>() { true, true, true, true });
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
        var connectMod = modules.GetModule<ConnectedTileMapModule>();

        foreach (var pos in walls)
        {
            // Remove tile
            var tile = tilesMod.GetTile(pos);
            tilesMod.RemoveTile(tile);
            zonesMod.RemovePair(tile);
            connectMod.RemoveTile(tile);
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
        var connectionMod = modules.GetModule<ConnectedTileMapModule>();


        //make sure tiles don't overlapg
        zonesMod.MoveArea(zone, dir);
        var tiles = zonesMod.GetTiles(zone);

        var zones = zonesMod.Zones;

        foreach (var z in zones)
        {
            if (z == zone)
                continue;
            var zTiles = zonesMod.GetTiles(z);
            foreach (var tile in zTiles)
            {
                if(tiles.Any(t => t.Position == tile.Position))
                {
                    tilesMod.RemoveTile(tile);
                    zonesMod.RemovePair(tile);
                    connectionMod.RemoveTile(tile);
                }
            }
        }

        // return neigthbour
        return new OptimizableModules(modules);
    }

    public void RemoveZoneConnection(Vector2 position, float delta)
    {
        ZoneEdge edge = GraphMod.GetEdge(position, delta);
        GraphMod.RemoveEdge(edge);
    }

    public Zone GetZone(LBSTile tile)
    {
        // Check if tile is valid
        if(tile == null)
            return null;

        var pair = AreasMod.GetPairTile(tile);


        if (pair == null || pair.Zone == null)
        {
            var msg = "";
            AreasMod.PairTiles.ForEach(p => msg += p.Tile.GetHashCode() + "\n");
            msg += "=====\n";
            msg += tile.GetHashCode();
            Debug.Log(msg);
            return null;
        }

        return pair.Zone;
    }

    public Zone GetZone(Vector2 position)
    {
        var tile = GetTile(position.ToInt());
        return GetZone(tile);
    }

    public LBSTile GetTile(Vector2Int position)
    {
        return TileMapMod.GetTile(position);
    }

    public void ConnectZones(Zone first, Zone second)
    {
        GraphMod.AddEdge(first, second);
    }

    public override object Clone()
    {
        return new HillClimbingAssistant(this.Icon, this.Name);
    }

    public override bool Equals(object obj)
    {
        var other = obj as HillClimbingAssistant;

        if (other == null) return false;

        if(!this.Name.Equals(other.Name)) return false;

        return true;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
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
