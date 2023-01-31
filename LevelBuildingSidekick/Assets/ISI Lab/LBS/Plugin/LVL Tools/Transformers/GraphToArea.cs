using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components.Graph;
using LBS.Components.Teselation;
using LBS.Components.Specifics;
using LBS.Components.TileMap;
using LBS.Components;
using System;
using System.Linq;

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

            /*
            if (graph == null)
            {
                Debug.LogError("Graph is NULL");
                return;
            }
            */

            if(schema.IsEmpty())
            {
                CreateDataFrom();
                var parche = new AreaToTileMap(layer);
                parche.Switch(ref layer);
            }
            else
            {
                Debug.LogWarning("[ISI Lab]: Implementar bien 'GraphToArea' cuando el objetivo no esta vacio");
                //EditDataFrom();
            }
        }

        

        private void CreateDataFrom()
        {
            Queue<RoomNode> open = new Queue<RoomNode>();
            HashSet<RoomNode> closed = new HashSet<RoomNode>();

            var parent = graph.GetNodes().OrderByDescending((n) => graph.GetNeighbors(n).Count).First();
            open.Enqueue(parent);

            var node = parent;
            var area = ConstructArea(node, new Vector2Int());
            schema.AddArea(area);

            int exit = 0;

            while (open.Count > 0 && exit <= 50) 
            {
                exit++;

                parent = open.Dequeue();

                var childs = graph.GetNeighbors(parent);

                foreach (var child in childs)
                {
                    if (closed.Contains(child) || open.ToHashSet().Contains(child))
                        continue;

                    open.Enqueue(child);

                    var childW = child.Room.Width;
                    var childH = child.Room.Height;
                    var parentW = parent.Room.Width;
                    var parentH = parent.Room.Height;

                    var pSchema = schema.GetArea(parent.ID);
                    var pPos = (pSchema != null) ? pSchema.Centroid : Vector2Int.zero; // parent

                    var dir = -((Vector2)(child.Position - parent.Position)).normalized;

                    var posX = (dir.x * ((childW + parentW) / 2f) * Mathf.Sqrt(2)) + pPos.x;
                    var posY = (dir.y * ((childH + parentH) / 2f) * Mathf.Sqrt(2)) + pPos.y;

                    if ((childW == 1 || childW == 2) && ((childH == 1 || childH == 2)))
                    {
                        posX = Mathf.Round(posX);
                        posY = Mathf.Round(posY);
                    }

                    area = ConstructArea(child, new Vector2Int((int)posX,(int) posY));
                    schema.AddArea(area);

                    //if (schema.CheckTilesRooms()) 
                    //    schema.RepositionRooms(); 
                }

                closed.Add(parent);

            }

            schema.RecalculateTilePos();
            /*
            for (int i = 0; i < graph.NodeCount; i++)
            {
                var node = graph.GetNode(i);
                var area = ConstructArea(node);
                schema.AddArea(area);
            }
            */
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
                        var cArea = ConstructArea(node, new Vector2Int());
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

        private TiledArea<ConnectedTile> ConstructArea(RoomNode node, Vector2Int offset)
        {
            var tiles = new List<ConnectedTile>();

            for(int j = 0; j < node.Room.Height; j++)
            {
                for(int i = 0; i < node.Room.Width; i++)
                {
                    tiles.Add(new ConnectedTile(new Vector2Int(i,j) + offset, node.ID));
                }
            }
            var area = new TiledArea<ConnectedTile>(tiles, node.ID, node.ID, node.Room.color);
            return area;
        }

    }
}

