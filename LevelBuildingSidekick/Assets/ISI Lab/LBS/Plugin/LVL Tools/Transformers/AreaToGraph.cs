using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components.Graph;
using LBS.Components.Teselation;
using LBS.Components.Specifics;
using LBS.Components.TileMap;

namespace LBS.Tools.Transformer
{
    public class AreaToGraph : Transformer
    {
        GraphModule<RoomNode> graph;
        AreaTileMap<TiledArea<ConnectedTile>, ConnectedTile> schema;

        public AreaToGraph(AreaTileMap<TiledArea<ConnectedTile>, ConnectedTile> schema, GraphModule<RoomNode> graph)
        {
            this.graph = graph;
            this.schema = schema;
        }

        public override void Switch()
        {
            if(schema == null)
            {
                Debug.LogError("Area Module is NULL");
                return;
            }

            if (graph == null)
            {
                Debug.LogWarning("Graph is NULL, new Graph created");
                graph = new GraphModule<RoomNode>();
            }

            List<string> ids = new List<string>();

            for(int i = 0; i < schema.RoomCount; i++)
            {
                var area = schema.GetRoom(i);
                ids.Add(area.Key);
                var node = graph.GetNode(area.Key);
                if(node != null)
                {
                    node.Position = area.Centroid.ToInt();
                    continue;
                }
                node = new RoomNode(area.Key, area.Centroid, new RoomData());
                graph.AddNode(node);
            }

            for(int i = 0; i < graph.NodeCount; i++)
            {
                var n = graph.GetNode(i);
                if(!ids.Contains(n.ID))
                {
                    Debug.LogWarning("Node Removed: No Tiles present in Room");
                    graph.RemoveNode(n.ID);
                }
            }
        }

        public override void OnAdd()
        {
            throw new System.NotImplementedException();
        }

        public override void OnRemove()
        {
            throw new System.NotImplementedException();
        }
    }
}
