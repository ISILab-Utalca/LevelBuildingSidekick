using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Components.Graph;
using LBS.Components.Teselation;
using LBS.Components.Specifics;

namespace LBS.Tools.Transformer
{
    public class GraphToArea : Transformer
    {
        GraphModule<RoomNode> graph;
        AreaModule<Area> areaModule;

        bool keepShape;

        public bool KeepShape => false;

        public GraphToArea(GraphModule<RoomNode> graph, AreaModule<Area> area)
        {
            this.graph = graph;
            this.areaModule = area;
        }

        public override void Switch()
        {
            if(graph == null)
            {
                Debug.LogError("Graph is NULL");
                return;
            }

            if(areaModule == null)
            {
                areaModule = new AreaModule<Area>();
            }

            for(int i = 0; i < graph.NodeCount; i++)
            {
                var node = graph.GetNode(i);
                var area = areaModule.GetArea(node.ID);
                if(area != null)
                {
                    if (!KeepShape)
                    {
                        //Puede que este creando las areas al reves por eje Y inverso
                        area = ConstructArea(node);
                    }
                    else
                    {
                        area.Centroid = node.Position;
                    }
                    continue;
                }
                areaModule.AddArea(ConstructArea(node));
            }
        }

        public Area ConstructArea(RoomNode node)
        {
            var area = new Area(node.ID,
                        new List<Vector2>() { node.Position,
                        new Vector2(node.Position.x + node.Room.Width, node.Position.y),
                        node.Position + node.Room.Size,
                        new Vector2(node.Position.x, node.Position.y + node.Room.Height)
                        });
            return area;
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

