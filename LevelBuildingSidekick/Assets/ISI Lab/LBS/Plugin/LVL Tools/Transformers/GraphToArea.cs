using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components.Graph;
using LBS.Components.Teselation;
using LBS.Components.Specifics;
using LBS.Components.TileMap;

namespace LBS.Tools.Transformer
{
    public class GraphToArea : Transformer
    {
        GraphModule<RoomNode> graph;
        AreaTileMap<TiledArea<ConnectedTile>, ConnectedTile> schema;

        bool keepShape;

        public bool KeepShape => false;

        public GraphToArea(GraphModule<RoomNode> graph, AreaTileMap<TiledArea<ConnectedTile>, ConnectedTile> schema)
        {
            this.graph = graph;
            this.schema = schema;
        }

        public override void Switch()
        {
            if(graph == null)
            {
                Debug.LogError("Graph is NULL");
                return;
            }

            if(schema == null)
            {
                schema = new AreaTileMap<TiledArea<ConnectedTile>, ConnectedTile>();
            }

            for(int i = 0; i < graph.NodeCount; i++)
            {
                var node = graph.GetNode(i);
                var room = schema.GetArea(node.ID);
                if(room != null)
                {
                    if (!KeepShape)
                    {
                        // (!) Puede que este creando las areas al reves por eje Y inverso
                        var ca = ConstructArea(node);
                        room = new TiledArea<LBSTile>(ca.Tiles, ca.ID, ca.Key);
                    }
                    else
                    {
                        room.Origin += room.Centroid - node.Position;
                    }
                    continue;
                }
            }
        }

        public TiledArea<ConnectedTile> ConstructArea(RoomNode node)
        {
            var tiles = new List<ConnectedTile>();

            for(int j = 0; j < node.Height; j++)
            {
                for(int i = 0; i < node.Width; i++)
                {
                    tiles.Add(new ConnectedTile(new Vector2(i,j), node.ID));
                }
            }


            return new TiledArea<ConnectedTile>(tiles, node.ID, "Room: " + node.ID);
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

