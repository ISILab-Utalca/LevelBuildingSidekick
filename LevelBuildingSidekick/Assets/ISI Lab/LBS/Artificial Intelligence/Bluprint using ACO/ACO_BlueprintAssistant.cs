using ISILab.Extensions;
using ISILab.LBS;
using ISILab.LBS.Assistants;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using Optimization.ACO;
using Optimization.Data;
using Optimization.Evaluators;
using Optimization.Restrictions;
using Optimization.Terminators;
using Problem.Evaluators;
using Problem.Restrictions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;


[System.Serializable]
[RequieredModule(typeof(TileMapModule),
    typeof(ConnectedTileMapModule),
    typeof(SectorizedTileMapModule),
    typeof(ConnectedZonesModule),
    typeof(ConstrainsZonesModule))]
public class ACO_BlueprintAssistant : LBSAssistant, IStep1
{
    #region META-FIELDS
    [SerializeField, JsonRequired]
    public bool visibleConstraints = false;
    [SerializeField, JsonRequired]
    public bool printClocks = false;
    #endregion

    [JsonIgnore, NonSerialized]
    private ACO aco = new ACO();

    [Range(1, 100)]
    public int iterations = 10;
    [Range(1, 1000)]
    public int antsPerIteration = 5;
    [Range(0.01f, 10)]
    public float pheromoneIntensity = 1f;
    [Range(0.01f, 099f)]
    public float evaporationRate = 0.9f;

    public string seed = "";

    public float[] evaluatorWeight = new float[] { 0.3f, 0.3f, 0.4f };

    public bool enforceGraphStructure = true;

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
    public ACO_BlueprintAssistant(Texture2D icon, string name) : base(icon, name)
    {
    }
    #endregion

    public override object Clone()
    {
        ACO_BlueprintAssistant clone = new ACO_BlueprintAssistant(this.Icon, this.Name);
        return clone;
    }

    public void Execute()
    {
        var clock = new Stopwatch();

        UnityEngine.Debug.Log("ACO start!");
        OnStart?.Invoke();

        // init Seed
        if (seed != "")
        {
            UnityEngine.Random.InitState(seed.GetHashCode());
        }
        else
        {
            var randomSeed = System.DateTime.Now.Ticks.GetHashCode();
            UnityEngine.Random.InitState(randomSeed);
        }

        //Create Graph
        var graph = GenerateGraph();

        // Terminator
        var terminator = new AgregateTermination()
        {
            terminators = new ITerminator[]
            {
                new IterationTerminator() { maxIterations = iterations} // numero de generaciones
            },
        };

        // Evaluator
        var evaluator = new WeightedAgregateEvaluator()
        {
            evs = new (IEvaluator, float)[]
            {
                (new VoidEvaluator(), evaluatorWeight[0]),
                (new ExteriorWallEvaluator(), evaluatorWeight[1]),
                (new CornerEvaluator(), evaluatorWeight[2])
            }
        };

        // Restricion
        var restrcition = new AgregateRestriction()
        {
            res = new IRestriction[]
            {
                new ConectivityGraphRestriction(),
                new SplitingRoomRestriction(),
                new AmountRoomRestriction(),
                new MinMaxAreaRestriction()
            }
        };

        clock.Start();
        aco = new ACO();
        var (path, data) = aco.Execute(
            graph,
            antsPerIteration,
            pheromoneIntensity,
            evaporationRate,
            evaluator,
            terminator,
            restrcition);
        clock.Stop();

        // add ACO data to mdules
        var acoData = path.Last();
        DataACOtoModules(acoData);

        Owner.Reload();
        UnityEngine.Debug.Log("ACO End!");
        OnTermination?.Invoke();
    }

    private void DataACOtoModules(Map map)
    {

        var tmm = Owner.GetModule<TileMapModule>();
        tmm.Clear();
        var stmm = Owner.GetModule<SectorizedTileMapModule>();
        stmm.Clear();
        var ctmm = Owner.GetModule<ConnectedTileMapModule>();
        ctmm.Clear();

        var rooms = map.rooms.ToArray();
        for (int i = 0; i < rooms.Length; i++)
        {
            var room = rooms[i].Value.ToArray();

            var zone = new Zone("Room " + i, new Color().RandomColor()); // FIX: mantener color de lo anterior.
            stmm.AddZone(zone);

            for (int j = 0; j < room.Length; j++)
            {
                var tile = new LBSTile(room[j].Key);
                tmm.AddTile(tile);

                stmm.AddPair(new TileZonePair(tile, zone));

                ctmm.AddPair(tile, new List<string>() { "", "", "", "" }, new List<bool> { true, true, true, true });
            }
        }

        // TODO: recalcular las murallas y puertas

        var czm = Owner.GetModule<ConnectedZonesModule>();
        czm.Clear();
        // TODO: volver a conectar las zonas.


        var cszm = Owner.GetModule<ConstrainsZonesModule>();
        cszm.Clear();
        RecalculateConstraint();

    }

    public List<LBSTile> GetTiles(Zone zone)
    {
        return Owner.GetModule<SectorizedTileMapModule>().GetTiles(zone);
    }

    public List<ZoneEdge> GetEdges()
    {
        return Owner.GetModule<ConnectedZonesModule>().Edges;
    }

    public Zone GetZone(LBSTile tile)
    {
        var STMM = Owner.GetModule<SectorizedTileMapModule>();

        // Check if tile is valid
        if (tile == null)
            return null;

        var pair = STMM.GetPairTile(tile);

        if (pair == null || pair.Zone == null)
        {
            var msg = "";
            STMM.PairTiles.ForEach(p => msg += p.Tile.GetHashCode() + "\n");
            msg += "=====\n";
            msg += tile.GetHashCode();
            UnityEngine.Debug.Log(msg);
            return null;
        }

        return pair.Zone;
    }

    public Zone GetZone(Vector2Int position)
    {
        var TMM = Owner.GetModule<TileMapModule>();
        var tile = TMM.GetTile(position);
        return GetZone(tile);
    }

    public void ConnectZones(Zone z1, Zone z2)
    {
        var CZM = Owner.GetModule<ConnectedZonesModule>();
        CZM.AddEdge(z1, z2);
    }

    private int _Parse(string v)
    {
        int r = 0;
        for (int i = 0; i < v.Length; i++)
        {
            r += (int)v[i] * (10^i);
        }
        return r;
    }    

    private Graph GenerateGraph()
    {
        var graph = new Graph("ACO-Graph");

        var STMM = Owner.GetModule<SectorizedTileMapModule>();

        STMM.Zones.ForEach(z =>
        {
            var node = new Graph.Node();
            node.name = z.ID;
            node.id = _Parse(z.ID);
            node.pos = new Vector2(z.Pivot.x, z.Pivot.y);
            node.color = new Color(z.Color.r, z.Color.g, z.Color.b);
            graph.nodes.Add(node);
        });

        var CTMM = Owner.GetModule<ConnectedZonesModule>();

        CTMM.Edges.ForEach(c =>
        {
            var edge = new Graph.Edge();
            edge.n1 = graph.nodes.Find(n => n.id == _Parse(c.First.ID));
            edge.n2 = graph.nodes.Find(n => n.id == _Parse(c.Second.ID));

            graph.edges.Add(edge);
        });

        var CZM = Owner.GetModule<ConstrainsZonesModule>();

        CZM.Constraints.ForEach(c =>
        {
            var id = _Parse(c.Zone.ID);
            var nodes = graph.nodes;
            var node = graph.nodes.Find(n => n.id == id);

            var cnt = c.Constraint;
            node.maxArea = new Vector2Int(
                (int)cnt.maxWidth,
                (int)cnt.maxHeight);

            node.minArea = new Vector2Int(
                (int)cnt.minWidth,
                (int)cnt.minHeight);
        });

        return graph;
    }

    public void RecalculateConstraint()
    {
        var zoneModule = Owner.GetModule<SectorizedTileMapModule>();
        var zones = zoneModule.Zones;

        var CZM = Owner.GetModule<ConstrainsZonesModule>();
        CZM.Clear();

        foreach (var zone in zones)
        {
            var bounds = zoneModule.GetBounds(zone);

            var min = new Vector2(bounds.width - 2, bounds.height - 2);

            if (min.x < 1)
                min.x = 1;
            if (min.y < 1)
                min.y = 1;

            var max = new Vector2(bounds.width + 2, bounds.height + 2);

            CZM.AddPair(zone, min, max);

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

        // TODO:
        // aqui van las configuraciones necesarias para el funcionamineot de ACO
        // para referencias de implementacion ver la clase [HillClimbingAssistant]
        // en la funcion [OnAttachLayer]
    }

}
