using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components.Graph;
using LBS.Components.Teselation;
using LBS.Components.Specifics;
using LBS.Components.TileMap;
using LBS.Components;
using System;

namespace LBS.Tools.Transformer
{
    public class GraphToArea : Transformer
    {
        private GraphModule<RoomNode> graph;
        private AreaTileMap<TiledArea<ConnectedTile>, ConnectedTile> schema;

        private bool keepShape;

        public bool KeepShape => false;

        public GraphToArea(Type from, Type to) : base(from, to){ }

        public override void Switch(ref LBSLayer layer)
        {
            graph = layer.GetModule(From) as GraphModule<RoomNode>;
            schema = layer.GetModule(To) as AreaTileMap<TiledArea<ConnectedTile>, ConnectedTile>;


            if (graph == null)
            {
                Debug.LogError("Graph is NULL");
                return;
            }

            if(schema.IsEmpty())
            {
                CreateDataFrom();
            }
            else
            {
                EditDataFrom();
            }
        }

        private void CreateDataFrom()
        {
            for (int i = 0; i < graph.NodeCount; i++)
            {
                var node = graph.GetNode(i);
                var area = ConstructArea(node);
                schema.AddArea(area);
            }
        }

        private void EditDataFrom()
        {
            for (int i = 0; i < graph.NodeCount; i++)
            {
                var node = graph.GetNode(i);
                var room = schema.GetArea(node.ID);
                if (room != null)
                {
                    if (!KeepShape)
                    {
                        // (!) Puede que este creando las areas al reves por eje Y inverso
                        var cArea = ConstructArea(node);
                        room = new TiledArea<LBSTile>(cArea.Tiles, cArea.ID, cArea.Key, cArea.Color);
                    }
                    else
                    {
                        room.Origin += room.Centroid - node.Position;
                    }
                    continue;
                }
            }
        }

        private TiledArea<ConnectedTile> ConstructArea(RoomNode node)
        {
            var tiles = new List<ConnectedTile>();

            for(int j = 0; j < node.Height; j++)
            {
                for(int i = 0; i < node.Width; i++)
                {
                    tiles.Add(new ConnectedTile(new Vector2(i,j), node.ID));
                }
            }
            var color = new Color().RandomColor();
            var area = new TiledArea<ConnectedTile>(tiles, node.ID, "Room: " + node.ID,color);
            return area;
        }

    }
}

