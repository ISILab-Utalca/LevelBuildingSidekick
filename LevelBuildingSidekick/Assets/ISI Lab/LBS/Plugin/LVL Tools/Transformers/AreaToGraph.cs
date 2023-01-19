using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components.Graph;
using LBS.Components.Teselation;
using LBS.Components.Specifics;

namespace LBS.Tools.Transformer
{
    public class AreaToGraph : Transformer
    {
        GraphModule<RoomNode> graph;
        AreaModule<Area> areaModule;

        public AreaToGraph(AreaModule<Area> area, GraphModule<RoomNode> graph)
        {
            this.graph = graph;
            this.areaModule = area;
        }

        public override void Switch()
        {
            if(areaModule == null)
            {
                Debug.LogError("Area Module is NULL");
                return;
            }

            if (graph == null)
            {
                Debug.LogWarning("Graph is NULL, new Graph created");
                graph = new GraphModule<RoomNode>();
            }

            for(int i = 0; i < areaModule.AreaCount; i++)
            {
                var area = areaModule.GetArea(i);
                var node = graph.GetNode(area.ID);
                if(node != null)
                {
                    node.Position = area.Centroid.ToInt();
                    continue;
                }
                node = new RoomNode(area.ID, area.Centroid, new RoomData());
                graph.AddNode(node);
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
