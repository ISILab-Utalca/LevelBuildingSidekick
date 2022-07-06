using System;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;
using LevelBuildingSidekick.Graph;
using LevelBuildingSidekick.Blueprint;
using System.Linq;
using UnityEditor;

namespace LevelBuildingSidekick.OfficePlan
{
    public class OfficePlanController : LevelRepresentationController
    {
        public GraphController Graph { get; set; }
        public SchemaController Schema { get; set; }
        public GameObject Floor
        {
            get
            {
                return (Data as OfficePlanData).floor;
            }
            set
            {
                (Data as OfficePlanData).floor = value;
            }
        }
        public GameObject Wall
        {
            get
            {
                return (Data as OfficePlanData).wall;
            }
            set
            {
                (Data as OfficePlanData).wall = value;
            }
        }
        public GameObject Door
        {
            get
            {
                return (Data as OfficePlanData).door;
            }
            set
            {
                (Data as OfficePlanData).door = value;
            }
        }
        public ToolkitController toolkit;

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

            var graph = Activator.CreateInstance(data.graph.ControllerType, new object[] { data.graph });
            if (graph is GraphController)
            {
                Graph = graph as GraphController;
            }

            var schema = Activator.CreateInstance(data.schema.ControllerType, new object[] { data.schema });
            if (schema is SchemaController)
            {
                Schema = schema as SchemaController;
            }

            /*if(Graph.Nodes.Count > 0)
            {
                foreach (RoomController r in Schema.Rooms)
                {
                    var node = Graph.Nodes.First((n) => n.ID == r.ID);
                    if (node == null)
                    {
                        Debug.LogError("Incongruent Data: " + r.ID);
                        continue;
                    }

                    r.Room = node.Room;
                }
            }*/
            //Toolkit = Graph.Toolkit;
            //ToolkitOverlay.draw = Graph.Toolkit.View.DrawEditor;
           if(data.toolkitData != null)
           {
                var t = Activator.CreateInstance(data.toolkitData.ControllerType, new object[] { data.toolkitData, this.Graph });
                if (t is ToolkitController)
                {
                    toolkit = t as ToolkitController;
                }
                //ToolkitOverlay.draw = toolkit.View.DrawEditor;
           }
        }

        /*public void GraphToBlueprint()
        {
            float minimumChange = 2 * 1.14f;
            bool[,] adjacency = Graph.AdjacencyMatrix();
            Dictionary<int, Vector2Int> positions = Graph.ToMatrixPositions(Schema.Size);

            foreach (NodeController n in Graph.Nodes)
            {
                if (!Schema.ContainsRoom(n.ID))
                {
                    Schema.AddRoom(n.Room, positions[n.ID]);
                }
            }

            foreach (RoomController r in Schema.Rooms)
            {
                r.ResizeToMin();
            }

            #region PUSH
            bool colliding = false;
            do
            {
                foreach (RoomController r in Schema.Rooms)
                {
                    Vector2Int v = Schema.GetCollisionPush(r);
                    colliding = v.magnitude != 0;
                    v += GetPush(r.ID, adjacency, positions);
                    r.Position += v;
                }
            }
            while (colliding);
            #endregion

            #region PULL
            float maxDistance = 0;
            do
            {
                foreach (RoomController r in Schema.Rooms)
                {
                    float dist = Schema.Translate(r, GetPull(r.ID, adjacency, positions)).magnitude;
                    maxDistance = dist > maxDistance ? dist : maxDistance;
                }
            }
            while (maxDistance < minimumChange);
            #endregion

            //Search empty spaces and fill
            Schema.ToTileMap();
        }*/

        /*public void GraphToSchema()
        {
            //Debug.Log("Step1");
            Schema.ClearRooms();

            List<NodeController> open = new List<NodeController>();
            List<NodeController> closed = new List<NodeController>();

            //Debug.Log("Step1");
            var node = Graph.Nodes.OrderBy((n) => n.Position.magnitude).First();
            var room = Schema.AddRoom(node.Room, Vector2Int.zero);
            //Debug.Log("Step2");
            //Debug.Log("Root: " + room.ID);
            Schema.ResizeRoomToMin(room);
            open.Add(node);

            //Debug.Log("Step3");
            while (open.Count > 0)
            {
                var parent = open.First();
                open.Remove(parent);
                closed.Add(parent);
                var parentRoom = Schema.Rooms.Find((r) => r.ID == parent.ID);
                if(parentRoom == null)
                {
                    Debug.LogError("Parent ID: " + parent.ID);
                    foreach (RoomController r in Schema.Rooms)
                    {
                        Debug.LogError("Room ID" + r.ID);
                    }
                    return;
                }
                foreach (NodeController n in parent.neighbors)
                {
                    if (closed.Find((c) => c.ID == n.ID) != null || open.Find((o) => o.ID == n.ID) != null)
                    {
                        continue;
                    }
                    Vector2 dir = (Vector2)(n.Position - parent.Position) / Vector2.Distance(parent.Position, n.Position);
                    Vector2Int pos = Schema.CloserEmpty(dir, parentRoom.Position, out Vector2Int last);
                    if (pos.x <0 || pos.y < 0)
                    {
                        pos = Schema.CloserEmptyFrom(last);
                    }
                    /*if (pos.x < 0 || pos.y < 0)
                    {
                        pos = Vector2Int.zero;
                    }

        //Debug.LogWarning(pos);

        room = Schema.AddRoom(n.Room, pos);
                    //Debug.Log(room.ID);
                    Schema.ResizeRoomToMin(room);
                    open.Add(n);
                }
            }
            //Debug.Log("Step4");
        }*/

        /*public void GraphToSchema()
        {
            //Debug.Log("Step1");
            Schema.ClearRooms();

            List<NodeController> open = new List<NodeController>();
            List<NodeController> closed = new List<NodeController>();

            //Debug.Log("Step1");
            var node = Graph.Nodes.OrderBy((n) => n.neighbors.Count).First();
            var room = Schema.AddRoom(node.Room, Schema.Size/2);
            //var first = room;
            //Debug.Log("Step2");
            //Debug.Log("Root: " + room.ID);
            Schema.ResizeRoomToMin(room);
            open.Add(node);

            //Debug.Log("Step3");
            while (open.Count > 0)
            {
                var parent = open.First();
                Debug.Log("Node: " + parent.Label);
                open.Remove(parent);
                closed.Add(parent);
                var parentRoom = Schema.Rooms.Find((r) => r.ID == parent.ID);
                if (parentRoom == null)
                {
                    Debug.LogError("Parent ID: " + parent.ID);
                    foreach (RoomController r in Schema.Rooms)
                    {
                        Debug.LogError("Room ID" + r.ID);
                    }
                    return;
                }
                foreach (NodeController n in parent.neighbors)
                {
                    if (closed.Find((c) => c.ID == n.ID) != null || open.Find((o) => o.ID == n.ID) != null)
                    {
                        continue;
                    }
                    Vector2 dir = (Vector2)(n.Position - parent.Position) / Vector2.Distance(parent.Position, n.Position);
                    Vector2Int pos = Schema.CloserEmpty(dir, parentRoom.Position, out Vector2Int last);
                    if (pos.x < 0 || pos.y < 0)
                    {
                        pos = Schema.CloserEmptyFrom(last);
                    }

                    //Debug.LogWarning(pos);

                    room = Schema.AddRoom(n.Room, pos);
                    //Debug.Log(room.ID);
                    Schema.ResizeRoomToMin(room);

                    //Fix PULLS
                    Schema.SolveAdjacencies(room, n.neighbors.Select((i) => i.ID).ToHashSet<int>());

                    if(Schema.GetCollisions(room, out HashSet<Vector2Int> collisions))
                    {
                        Schema.SolveCollisions(room, collisions);
                    }

                    open.Add(n);
                }
            }
            //Debug.Log("Step4");
        }*/

        public void GraphToSchema()
        {
            Schema.ClearRooms();

            if(Graph.Nodes.Count == 0)
            {
                return;
            }

            List<RoomController> rooms = new List<RoomController>();

            foreach (NodeController n in Graph.Nodes)
            {
                var pos = Graph.ToMatrixPosition(n.ID, Schema.Size);
                var room = Schema.AddRoom(n.Room, pos);
                //Debug.Log("Node: " + room.Label);
                Schema.ResizeRoomToMin(room);
                //room.ResizeToMin();
                rooms.Add(room);
            }


            Schema.ClearRooms();
            var node = Graph.Nodes.OrderByDescending((n) => n.MinArea).First();
            var parent = rooms.Find((r) => r.ID == node.ID);

            List<RoomController> open = new List<RoomController>();
            List<RoomController> closed = new List<RoomController>();

            open.Add(parent);
            parent.Position = Graph.ToMatrixPosition(node.ID, Schema.Size);
            Schema.AddRoom(parent);

            //Debug.Log("C: " + open.Count);
            while (open.Count != 0)
            {
                parent = open.First();
                open.Remove(parent);
                closed.Add(parent);

                node = Graph.Nodes.Find((n) => n.ID == parent.ID);

                //Debug.Log("N: " + node.neighbors.Count);
                foreach(NodeController n in node.neighbors.OrderByDescending((n) => n.MinArea))
                {
                    var child = rooms.Find((r) => r.ID == n.ID);
                    if(closed.Find((c) => c.ID == child.ID) != null || open.Find((o) => o.ID == child.ID) != null)
                    {
                        continue;
                    }
                    Vector2 dist = (node.Centroid - n.Centroid);
                    var pos = Schema.CloserEmpty(dist.normalized ,parent.Position, out Vector2Int lastPos);


                    Debug.LogWarning("Node: " + child.Label);
                    if (pos.Equals(-Vector2Int.one))
                    {
                        pos = Schema.CloserEmptyFrom(lastPos);
                    }

                    Debug.Log("P1: " + pos + " - Bounds: " + child.Bounds);

                    if (pos.x < parent.Position.x)
                    {
                        pos.x -= (child.Bounds.x - 1);
                    }
                    if(pos.y > parent.Position.y)
                    {
                        pos.y += (child.Bounds.y - 1);
                    }

                    Debug.Log("P2: " + pos);

                    child.Position = pos;

                    Schema.AddRoom(child);

                    Schema.RegenerateTileMap();

                    /*if (Schema.GetCollisions(child, out HashSet<Vector2Int> collisions))
                    {
                        Schema.SolveCollisions(child, collisions);
                    }*/

                    open.Add(child);
                }
            }
                //open.Add(n);


        }
        //Fix to use centroids
        public Vector2Int GetPull(int ID, bool[,] adjacency, Dictionary<int, Vector2Int> positions)
        {
            int index = Graph.Nodes.FindIndex((n) => n.ID == ID);
            Vector2Int pull = Vector2Int.zero;
            for (int i = 0; i < adjacency.GetLength(1); i++)
            {
                if (adjacency[index, i])
                {
                    pull += (positions[Graph.Nodes[i].ID] - positions[ID]); // - o +? dividir por 2?
                }
            }
            return pull;
        }
        //Fix to Use centroids, should scale with 1/distance
        public Vector2Int GetPush(int ID, bool[,] adjacency, Dictionary<int, Vector2Int> positions)
        {
            int index = Graph.Nodes.FindIndex((n) => n.ID == ID);
            Vector2Int push = Vector2Int.zero;
            for (int i = 0; i < adjacency.GetLength(1); i++)
            {
                if (!adjacency[index, i])
                {
                    push += (positions[ID] - positions[Graph.Nodes[i].ID]); // - o +? dividir por 2? // distance should affect inversed
                }
            }
            return push;
        }
        public Tuple<Vector2Int, Vector2Int>[] GetPulls(bool[,] adjacency, Vector2Int[] positions)
        {
            var pulls = new Tuple<Vector2Int, Vector2Int>[positions.Length];

            //TODO check adjacency is square;

            for (int i = 0; i < positions.Length; i++)
            {
                Vector2Int xPull = Vector2Int.zero;
                Vector2Int yPull = Vector2Int.zero;
                for (int j = 0; j < positions.Length; j++)
                {
                    if (adjacency[i, j])
                    {
                        var pull = positions[i] - positions[j] / 2;

                        if (pull.x < 0)
                        {
                            xPull.x += pull.x;
                        }
                        else
                        {
                            xPull.y += pull.x;
                        }

                        if (pull.y < 0)
                        {
                            yPull.x += pull.y;
                        }
                        else
                        {
                            yPull.y += pull.y;
                        }
                    }
                }
                pulls[i] = new Tuple<Vector2Int, Vector2Int>(xPull, yPull);
            }

            return pulls;
        }

        public override void Update()
        {
            toolkit.Update();

            if(Graph.SelectedNode != null)
            {
                ElementInspector.titleContent = new GUIContent("Node Inspector");
                ElementInspector.draw = Graph.SelectedNode.View.DrawEditor;
            }
            else if(Graph.SelectedEdge != null)
            {
                ElementInspector.titleContent = new GUIContent("Edge Inspector");
                ElementInspector.draw = Graph.SelectedEdge.View.DrawEditor;
            }
            else
            {
                ElementInspector.titleContent = new GUIContent("Graph Inspector");
                ElementInspector.draw = Graph.View.DrawEditor;
            }
        }

        //missing Doors
        public void Generate3D()
        {
            if(Floor == null || Wall == null)
            {
                Debug.LogError("The Tiles Prefabs are NULL");
                return;
            }
            //Debug.Log("B.R: " + Schema.Rooms.Count);
            foreach (RoomController r in Schema.Rooms)
            {
                GameObject parent = new GameObject(r.Label);
                parent.transform.position = new Vector3(r.Position.x, 0, r.Position.y);
                //Debug.Log("R.P: " + r.TilePositions.Count);
                foreach (Vector2Int v in r.TilePositions)
                {
                    var f = GameObject.Instantiate(Floor,new Vector3(v.x + r.Position.x,0,v.y + r.Position.y),Floor.transform.rotation);
                    f.transform.parent = parent.transform;
                    if (v.x == 0)
                    {
                        Vector3 pos = new Vector3(v.x + r.Position.x, 0, v.y + r.Position.y) + Vector3.left / 2;
                        var g = GameObject.Instantiate(Wall, pos, Quaternion.Euler(0,90,0));
                        //g.name = "Left";
                        g.transform.parent = parent.transform;
                    }
                    if(v.y == 0)
                    {
                        Vector3 pos = new Vector3(v.x + r.Position.x, 0, v.y + r.Position.y) + Vector3.back / 2;
                        var g = GameObject.Instantiate(Wall, pos, Quaternion.Euler(0, 0, 0));
                        //g.name = "Back";
                        g.transform.parent = parent.transform;
                    }

                    if(v.x == r.Bounds.x - 1)
                    {
                        Vector3 pos = new Vector3(v.x + r.Position.x, 0, v.y + r.Position.y) + Vector3.right / 2;
                        var g = GameObject.Instantiate(Wall, pos, Quaternion.Euler(0, 270, 0));
                        //g.name = "Right";
                        g.transform.parent = parent.transform;
                    }
                    if(v.y == r.Bounds.y - 1)
                    {
                        //Debug.Log(r.InnerBounds);
                        Vector3 pos = new Vector3(v.x + r.Position.x, 0, v.y + r.Position.y) + Vector3.forward / 2;
                        var g = GameObject.Instantiate(Wall, pos, Quaternion.Euler(0, 180, 0));
                        //g.name = "Front";
                        g.transform.parent = parent.transform;
                    }
                }
            }
        }

        /*[MenuItem("Level Building Sidekick/Open Element Inspector")]
        public static void ShowInspector()
        {
            ElementInspector.Show();
        }*/
    }
}

