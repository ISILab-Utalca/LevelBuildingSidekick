using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using LBS;
using System;
using System.Linq;
using Utility;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using LBS.Graph;
using LBS.Schema;
using LBS.Transformers;
using LBS.ElementView;
using LBS.VisualElements;

namespace LBS.Graph
{
    // Este no deberia ser un controlador general de grafos
    // sino que deberia exitir un controlador para cada uno
    // de los diferentes grafos que podamso crear, asi podemos
    // tener una "GraphView" generica, ademas de una data generica
    // pero podemos guardar los datos especificos y controlarlos
    // de manera especifica (!!!)
    public class LBSGraphRCController : LBSGraphController
    {

        public LBSGraphRCController(LBSGraphView view,GraphicsModule data) : base(view, data)
        {

        }

        public override void OnContextualBuid(MainViewOld view, ContextualMenuPopulateEvent cmpe)
        {
            var pos = (cmpe.localMousePosition - new Vector2(view.transform.position.x, view.transform.position.y)) / view.scale;

            cmpe.menu.AppendAction("GraphRC/Add Node", (dma) => {
                var n = NewNode(pos);
                AddNodeView(n);
                });
        }

        public override string GetName()
        {
            return "Node Layer";
        }

        public void GenerateSchema() 
        {
            var tm = new GraphToSchema().Transform(data);
            LBSController.CurrentLevel.data.AddRepresentation(tm);
        }

        public override void PopulateView(MainViewOld view)
        {
            this.view = view;
            view.DeleteElements(elements);
            data.GetNodes().ForEach(n => AddNodeView(n));
            data.GetEdges().ForEach(e => AddEdgeView(e));
        }

        private void AddEdgeView(LBSEdgeDataOld edge)
        {
            var nodeViews = view.graphElements.ToList().Where(e => e is LBSNodeViewOld).Select(e => e as LBSNodeViewOld).ToList();
            var nv1 = nodeViews.Find((n) => {
                return n.Data.Label == edge.FirstNodeLabel;
            });
            var nv2 = nodeViews.Find((n) =>
            {
                return n.Data.Label == edge.SecondNodeLabel;
            });

            if (nv1 == null || nv2 == null)
            {
                Debug.LogWarning("There is no 'NodeView' to which to link this 'EdgeView'.");
                return;
            }

            var edgeView = new LBSDotedEdgeView(nv1, nv2, view);
            nv1.OnMoving += edgeView.ActualizeView;
            nv2.OnMoving += edgeView.ActualizeView;
            elements.Add(edgeView);
            view.AddElement(edgeView);
        }

        internal override LBSNodeDataOld NewNode(Vector2 position)
        {
            var graph = data;
            LBSNodeDataOld node = new RoomCharacteristicsData("Node: " + graph.NodeCount(), position, CellSize);
            AddNode(node);
            return node;
        }
    }

}
