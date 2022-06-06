using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelBuildingSidekick;

namespace LevelBuildingSidekick.Graph
{
    public class NodeController : Controller
    {
        public List<NodeController> neighbors;
        public Vector2Int Position => (Data as NodeData).Position;
        public NodeController(Data data) : base(data)
        {
            View = new NodeView(this);
            neighbors = new List<NodeController>();
        }

        public override void LoadData()
        {
            //throw new System.NotImplementedException();
        }

        public override void Update()
        {
        }

        public Rect GetRect()
        {
            NodeData d = Data as NodeData;
            return new Rect(d.Position, d.Radius * 2 * Vector2.one);
        }

        public void Translate(Vector2 delta)
        {
            NodeData d = Data as NodeData;
            d.Position += new Vector2Int((int)delta.x,(int)delta.y);
        }
        public void Translate(Vector2Int delta)
        {
            NodeData d = Data as NodeData;
            d.Position += delta;
        }
    }
}

