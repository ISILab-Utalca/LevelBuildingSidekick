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
        public BlueprintController Blueprint { get; set; }
        public TeselationType Teseleation
        {
            get
            {
                return (Data as OfficePlanData).type;
            }
            set
            {
                (Data as OfficePlanData).type = value;
            }
        }
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

            var blueprint = Activator.CreateInstance(data.blueprint.ControllerType, new object[] { data.blueprint });
            if (blueprint is BlueprintController)
            {
                Blueprint = blueprint as BlueprintController;
            }

            foreach (RoomController r in Blueprint.Rooms)
            {
                var node = Graph.Nodes.First((n) => n.ID == r.ID);
                if (node == null)
                {
                    Debug.LogError("Incongruent Data: " + r.ID);
                    continue;
                }

                r.Room = node.Room;
            }
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
            Dictionary<int, Vector2Int> positions = Graph.ToMatrixPositions(Blueprint.Size);

            foreach (NodeController n in Graph.Nodes)
            {
                if (!Blueprint.ContainsRoom(n.ID))
                {
                    Blueprint.AddRoom(n.Room, positions[n.ID]);
                }
            }

            foreach (RoomController r in Blueprint.Rooms)
            {
                r.ResizeToMin();
            }

            #region PUSH
            bool colliding = false;
            do
            {
                foreach (RoomController r in Blueprint.Rooms)
                {
                    Vector2Int v = Blueprint.GetCollisionPush(r);
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
                foreach (RoomController r in Blueprint.Rooms)
                {
                    float dist = Blueprint.Translate(r, GetPull(r.ID, adjacency, positions)).magnitude;
                    maxDistance = dist > maxDistance ? dist : maxDistance;
                }
            }
            while (maxDistance < minimumChange);
            #endregion

            //Search empty spaces and fill
            Blueprint.ToTileMap();
        }*/

        public void SimpleGraphToBlueprint()
        {
            //Debug.Log("Step1");
            Blueprint.ClearRooms();

            List<NodeController> open = new List<NodeController>();
            List<NodeController> closed = new List<NodeController>();

            //Debug.Log("Step1");
            var node = Graph.Nodes.OrderBy((n) => n.Position.magnitude).First();
            var room = Blueprint.AddRoom(node.Room, Vector2Int.zero);
            //Debug.Log("Step2");
            //Debug.Log("Root: " + room.ID);
            Blueprint.ResizeRoomToMin(room);
            open.Add(node);

            //Debug.Log("Step3");
            while (open.Count > 0)
            {
                var parent = open.First();
                open.Remove(parent);
                closed.Add(parent);
                var parentRoom = Blueprint.Rooms.Find((r) => r.ID == parent.ID);
                if(parentRoom == null)
                {
                    Debug.LogError("Parent ID: " + parent.ID);
                    foreach (RoomController r in Blueprint.Rooms)
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
                    Vector2Int pos = Blueprint.CloserEmpty(dir, parentRoom.Position, out Vector2Int last);
                    if (pos.x <0 || pos.y < 0)
                    {
                        pos = Blueprint.CloserEmptyFrom(last);
                    }
                    /*if (pos.x < 0 || pos.y < 0)
                    {
                        pos = Vector2Int.zero;
                    }*/

                    //Debug.LogWarning(pos);

                    room = Blueprint.AddRoom(n.Room, pos);
                    //Debug.Log(room.ID);
                    Blueprint.ResizeRoomToMin(room);
                    open.Add(n);
                }
            }
            //Debug.Log("Step4");
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
            //Debug.Log("B.R: " + Blueprint.Rooms.Count);
            foreach (RoomController r in Blueprint.Rooms)
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
                    if(v.x == r.InnerBounds.x - 1)
                    {
                        Vector3 pos = new Vector3(v.x + r.Position.x, 0, v.y + r.Position.y) + Vector3.right / 2;
                        var g = GameObject.Instantiate(Wall, pos, Quaternion.Euler(0, 270, 0));
                        //g.name = "Right";
                        g.transform.parent = parent.transform;
                    }
                    if(v.y == r.InnerBounds.y - 1)
                    {
                        Vector3 pos = new Vector3(v.x + r.Position.x, 0, v.y + r.Position.y) + Vector3.forward / 2;
                        var g = GameObject.Instantiate(Wall, pos, Quaternion.Euler(0, 180, 0));
                        //g.name = "Front";
                        g.transform.parent = parent.transform;
                    }
                }
            }
        }

        [MenuItem("Level Building Sidekick/Open Element Inspector")]
        public static void ShowInspector()
        {
            ElementInspector.Show();
        }
    }
}

