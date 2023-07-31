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
    public class AreaToGraph : Transformer
    {
        GraphModule<RoomNode> graph;
        AreaTileMap<TiledArea> schema;

        public AreaToGraph(Type from, Type to) : base(from, to) { }

        public AreaToGraph(AreaTileMap<TiledArea> schema, GraphModule<RoomNode> graph) : base(schema.GetType(), graph.GetType())
        {
            this.graph = graph;
            this.schema = schema;
        }

        public override void ReCalculate(ref LBSLayer layer)
        {
            throw new NotImplementedException();
        }

        public override void Switch(ref LBSLayer layer)
        {
            schema = layer.GetModule(From) as AreaTileMap<TiledArea>;
            graph = layer.GetModule(To) as GraphModule<RoomNode>;


            if (schema == null)
            {
                Debug.LogError("Area Module is NULL");
                return;
            }

            if (schema.IsEmpty())
            {
                CreateDataFrom();
            }
            else
            {
                Debug.LogWarning("[ISI Lab]: Implementar bien 'AreaToGraph' cuando el objetivo no esta vacio");
                //EditDataFrom();
            }
        }

        private void CreateDataFrom()
        {
            foreach (var area in schema.Areas)
            {
                var pos = area.Centroid;
                var rData = new RoomData(area.Width, area.Height, new List<string>(), area.Color ); // (!) le faltan las tags pero no se de donde sacarlas
                new RoomNode(area.ID, pos, rData);
            }

            // (!!!) le faltan las conexiones
        }

        private void EditDataFrom()
        {
            List<string> ids = new List<string>();

            for (int i = 0; i < schema.AreaCount; i++)
            {
                var area = schema.GetArea(i);
                ids.Add(area.ID);
                var node = graph.GetNode(area.ID);
                if (node != null)
                {
                    node.Position = area.Centroid.ToInt();
                    continue;
                }
                node = new RoomNode(area.ID, area.Centroid, new RoomData());
                graph.AddNode(node);
            }

            for (int i = 0; i < graph.NodeCount; i++)
            {
                var n = graph.GetNode(i);
                if (!ids.Contains(n.ID))
                {
                    Debug.LogWarning("Node Removed: No Tiles present in Room");
                    graph.RemoveNode(n.ID);
                }
            }
        }

    }
}
