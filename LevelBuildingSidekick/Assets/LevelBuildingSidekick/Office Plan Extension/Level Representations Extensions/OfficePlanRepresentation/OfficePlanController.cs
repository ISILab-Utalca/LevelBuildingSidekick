using System;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using LevelBuildingSidekick.Graph;
using LevelBuildingSidekick.Schema;
using System.Linq;
using UnityEditor;
using static LevelBuildingSidekick.Schema.SchemaData;

namespace LevelBuildingSidekick.OfficePlan
{
    public class OfficePlanController : LevelRepresentationController
    {
        private SchemaData schema;

        public GraphController Graph { get; set; }
        //public SchemaController Schema { get; set; }

        private static GenericWindow _ElementInspector;

        public static GenericWindow ElementInspector
        {
            get
            {
                if (_ElementInspector == null)
                {
                    _ElementInspector = GenericWindow.CreateInstance<GenericWindow>();
                }
                return _ElementInspector;
            }
        }

        public OfficePlanController(Data data) : base(data)
        {
            View = new OfficePlanView(this);
        }

        public override void LoadData()
        {
            base.LoadData();

            var data = Data as OfficePlanData;

            if (data.graph == null)
            {
                data.graph = new GraphData();
            }

            var graph = Activator.CreateInstance(data.graph.ControllerType, new object[] { data.graph });
            if (graph is GraphController)
            {
                Graph = graph as GraphController;
            }

            if (data.schema == null)
            {
                data.schema = new SchemaData();
            }

            //var schema = Activator.CreateInstance(data.schema.ControllerType, new object[] { data.schema });
            //if (schema is SchemaController)
            {
              //  Schema = schema as SchemaController;
            }
        }

        public void GenerateShcema()
        {
            schema = GraphToSchema(Graph.Data as GraphData);
        }

        public void PrintSchema()
        {
            string msg = "";

            var sch = schema;
            msg += "<b>Schema</b>\n";
            msg += "Room count:" + sch.rooms.Count + "\n";

            foreach (var room in sch.rooms)
            {
                msg += "-------------------\n";
                msg += "id: " + room.ID + "\n";
                msg += "tile count: " + room.tiles.Count + "\n";
            }

            Debug.Log(msg);
        }

        public void Optimize()
        {
            var graph = Graph.Data as GraphData;
            var optimized = Utility.HillClimbing.Run(schema,graph,
                            () => { return Utility.HillClimbing.NonSignificantEpochs >= 100; },
                            GetNeighbors,
                            EvaluateMap);
            Debug.Log(optimized);
        }

        private SchemaData GraphToSchema(GraphData graphData) // ¿esta funcionando?
        {
            if (Graph.Nodes.Count <= 0)
            {
                Debug.LogWarning("[Error]: Graph node have 0 nodes.");
                return null;
            }

            Queue<NodeController> open = new Queue<NodeController>();
            HashSet<NodeController> closed = new HashSet<NodeController>();

            var parent = Graph.Nodes.OrderByDescending((n) => n.neighbors.Count).First();
            open.Enqueue(parent);

            var schema = new SchemaData();
            int h = (int)((parent.Room.maxHeight + parent.Room.minHeight) / 2f);
            int w = (int)((parent.Room.maxWidth + parent.Room.minWidth) / 2f);
            schema.AddRoom(Vector2Int.zero, h, w, parent.Room.label);

            while (open.Count > 0)
            {
                parent = open.Dequeue();

                var childs = parent.neighbors.OrderBy(n => Utility.MathTools.GetAngleD15(parent.Centroid, n.Centroid));
                foreach (NodeController child in parent.neighbors)
                {
                    if (closed.Contains(child) || open.ToHashSet().Contains(child))
                        continue;

                    open.Enqueue(child);
                    h = (int)((child.Room.maxHeight + child.Room.minHeight) / 2f);
                    w = (int)((child.Room.maxWidth + child.Room.minWidth) / 2f);
                    var pos = (child.Centroid - parent.Centroid);
                    schema.AddRoom(pos, w, h, child.Room.label);
                }

                closed.Add(parent);
            }

            return schema;
        }

        public List<SchemaData> GetNeighbors(SchemaData rootSchema)
        {
            var neightbours = new List<SchemaData>(); 
            foreach (var room in rootSchema.rooms)
            {
                var vWalls = room.GetVerticalWalls();
                var hWalls = room.GetHorizontalWalls();
                var walls = vWalls.Concat(hWalls);

                foreach (var wall in walls)
                {
                    var neighbor = rootSchema.Clone() as SchemaData;
                    neighbor.SetTiles(wall.allTiles, ""); // setea los tiles a nulo o default
                    neightbours.Add(neighbor);
                }
                foreach (var wall in walls)
                {
                    var neighbor = rootSchema.Clone() as SchemaData;
                    var tiles = new List<Vector2Int>();
                    wall.allTiles.ForEach(t => tiles.Add(t + wall.dir));
                    neighbor.SetTiles(tiles,room.ID);
                    neightbours.Add(neighbor);
                }
            }
            return neightbours;
        }

        public float EvaluateMap(SchemaData schemaData, GraphData graphData)
        {
            float alfa = 0.84f;
            float beta = 1 - alfa;
            var adjacenceValue = EvaluateAdjacencies(graphData, schemaData) * alfa;
            var areaValue = EvaluateAreas(graphData, schemaData) * beta;
            Debug.Log("Ad: "+adjacenceValue +" ,Ar: "+ areaValue);
            return adjacenceValue + areaValue ;
        }

        private float EvaluateAreas(GraphData graphData, SchemaData schema)
        {
            var value = 0f;
            for (int i = 0; i < graphData.nodes.Count; i++)
            {
                var node = graphData.nodes[i];
                //var n = schema.GetRoomByID(node.id);
                switch (node.room.proportionType)
                {
                    case ProportionType.RATIO:
                        value = EvaluateBtyRatio(node, schema.rooms[i]); // usar n en vez de la referencia en lista
                        break;
                    case ProportionType.SIZE:
                        value = EvaluateBtySize(node, schema.rooms[i]);
                        break;
                }
            }
            return value / schema.rooms.Count;
        }

        private float EvaluateBtyRatio(NodeData node, SchemaRoomData room)
        {
            float current = room.GetRatio();
            float objetive = node.room.xAspectRatio / (float)node.room.yAspectRatio;

            return 1 - (Mathf.Abs(objetive - current) / objetive);
        }

        private float EvaluateBtySize(NodeData node,SchemaRoomData room)
        {
            var vw = 1f;
            if (room.GetWidth() < node.room.minWidth || room.GetWidth() > node.room.maxWidth)
            {
                var objetive = (node.room.minWidth + node.room.maxWidth) / 2f;
                var current = room.GetWidth();
                vw -= (Mathf.Abs(objetive - current) / objetive);
            }

            var vh = 1f;
            if (room.GetHeight() < node.room.minHeight || room.GetHeight() > node.room.maxHeight)
            {
                var objetive = (node.room.minHeight + node.room.maxHeight) / 2f;
                var current = room.GetHeight();
                vh -= (Mathf.Abs(objetive - current) / objetive);
            }

            return (vw + vh)/2f;
        }

        private float EvaluateAdjacencies(GraphData graphData, SchemaData schema) // esto podria recivir una funcion de calculo de distancia (?)
        {
            var distValue = 0f;
            foreach (var edge in graphData.edges)
            {
                var r1 = schema.GetRoomByID(edge.firstNode.room.label);
                var r2 = schema.GetRoomByID(edge.secondNode.room.label);

                var roomDist = GetRoomDistance(r1, r2);
                if (roomDist <= 1)
                {
                    distValue++;
                }
                else
                {
                    var c = r1.tiles.Count();
                    var max1 = (r1.GetHeight() + r1.GetWidth()) / 2f;
                    var max2 = (r2.GetHeight() + r2.GetWidth()) / 2f;

                    Debug.Log("dist: " + roomDist + " ,c: " + c + " ,max1: " + max1 + " ,max2: " + max2);
                    distValue += 1 - (roomDist/(max1 + max2));
                }
            }

            return distValue / (float)graphData.edges.Count;
        }

        private int GetRoomDistance(SchemaRoomData r1, SchemaRoomData r2) // O2 - manhattan
        {
            var lessDist = int.MaxValue;
            var ts1 = r1.tiles.ToList();
            var ts2 = r2.tiles.ToList();
            for (int i = 0; i < ts1.Count; i++)
            {
                for (int j = 0; j < ts2.Count; j++)
                {
                    var t1 = ts1[i];
                    var t2 = ts2[j];

                    var dist = Mathf.Abs(t1.x - t2.x) + Mathf.Abs(t1.y - t2.y); // manhattan

                    if (dist <= lessDist)
                    {
                        lessDist = dist;
                    }
                }
            }
            return lessDist;
        }

        public override void Update()
        {
            Toolkit.Update();
            
            if(Graph.SelectedNode != null)
            {
                ElementInspector.titleContent = new GUIContent("Node Inspector");
                ElementInspector.draw = Graph.SelectedNode.View.DrawEditor; // (!!!)
            }
            else if(Graph.SelectedEdge != null)
            {
                ElementInspector.titleContent = new GUIContent("Edge Inspector");
                ElementInspector.draw = Graph.SelectedEdge.View.DrawEditor; // (!!!)
            }
            else
            {
                ElementInspector.titleContent = new GUIContent("Graph Inspector");
                ElementInspector.draw = Graph.View.DrawEditor; // (!!!)
            }
        }
    }
}

