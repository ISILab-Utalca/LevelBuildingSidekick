using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using LevelBuildingSidekick;
using System;
using System.Linq;
using UnityEngine.UIElements;
using LBS.Representation.TileMap;

namespace LevelBuildingSidekick.Graph
{
    public class LBSGraphView : LBSBaseView
    {
        public static bool isDragEdge = false;
        private static LBSNodeView first;

        public new class UxmlFactory : UxmlFactory<LBSGraphView, GraphView.UxmlTraits> { }

        private LBSGraphController controller;
        public LBSGraphController Controller
        {
            get
            {
                if (controller == null)
                {
                    controller = new LBSGraphController(this);
                }
                return controller;
            }
        }

        public List<LBSNodeView> SelectedNodes
        {
            get
            {
                return selection.Where((s) => s is LBSNodeView) as List<LBSNodeView>;
            }
        }

        public List<LBSEdgeView> SelectedEdges
        {
            get
            {
                return selection.Where(s => s is LBSEdgeView) as List<LBSEdgeView>;
            }
        }

        public LBSGraphView()
        {
            var gb = new GridBackground();
            gb.StretchToParentSize();
            Insert(0, gb);

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("GraphWindow");
            styleSheets.Add(styleSheet);
            LBSController.ShowLevelInspector();
        }

        public override void Populate<T>(T value)
        {
            var data = value as LBSGraphData;
            if(data == null)
                Debug.LogWarning("[Error]: The information you are trying to upload cannot be displayed in this view.");


            DeleteElements(graphElements);
            data.GetNodes().ForEach(n => AddNodeView(n));
            data.GetEdges().ForEach(e => AddEdgeView(e));
        }

        public LBSNodeView GetNodeViewBylabel(string label)
        {
            foreach (var element in graphElements)
            {
                if (element is LBSNodeView)
                {
                    var n = (LBSNodeView)element;
                    if (n != null && n.Data.Label == label)
                        return n;
                }
            }
            return null;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var pos = (evt.localMousePosition - new Vector2(viewTransform.position.x, viewTransform.position.y)) / scale;

            evt.menu.AppendAction("New Node", (a) =>
            {
                var data = Controller.NewNode(pos);
                AddNodeView(data);
            });

            evt.menu.AppendAction("Clean", ((a) =>
            {
                this.Controller.Clear();
                DeleteElements(graphElements);
            }));

            evt.menu.AppendAction("Debug/print info", (a) => {
                var graph = (controller.Data as LBSGraphData);
                graph.Print();

            });
        }

        protected override void ExecuteDefaultAction(EventBase evt)
        {
            base.ExecuteDefaultAction(evt);
            if (evt is MouseUpEvent)
            {
                var e = (MouseUpEvent)evt;
                if (e.button == 1)
                {
                    isDragEdge = false;
                    first = null;
                    //proxyEdge = null;
                }
            }
        }


        private void AddEdgeView(LBSEdgeData edge)
        {
            var graph = LBSController.CurrentLevel.data.GetRepresentation<LBSGraphData>();
            var nodeViews = graphElements.ToList().Where(e => e is LBSNodeView).Select(e => e as LBSNodeView).ToList();
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

            var edgeView = new LBSDotedEdgeView(nv1, nv2, this);
            nv1.OnMoving += edgeView.ActualizeView;
            nv2.OnMoving += edgeView.ActualizeView;
            AddElement(edgeView);
        }

        public void AddNodeView(LBSNodeData data)
        {
            var view = new LBSNodeView(data);
            view.OnStartDragEdge += StartDragEdge;
            view.OnEndDragEdge += EndDragEdge;
            AddElement(view);
            //view.AddManipulator(createEdge);
        }

        public void OnGUI()
        {
            if (isDragEdge)
            {
                //var pos1 = first.GetPosition().center;
                //var pos2 = mouse
                //proxyEdge.UpdateDraw(pos1,);
            }

            foreach (var e in graphElements)
            {
                if (e is LBSEdgeView)
                {
                    var edge = (LBSEdgeView)e;
                    //edge.UpdateDraw();
                }
            }
        }

        public void StartDragEdge(LBSNodeData data)
        {
            Debug.Log("Start edge");
            first = GetNodeViewBylabel(data.Label);
            //proxyEdge = new LBSProxyEdge(first.GetPosition().position,new Vector2(0,0));
            //AddElement(proxyEdge);
            isDragEdge = true;
        }

        public void EndDragEdge(LBSNodeData data)
        {
            if (first != null)
            {
                Debug.Log("End Edge");
                var second = GetNodeViewBylabel(data.Label);
                var edge = new LBSEdgeData(first.Data, second.Data);
                var current = LBSController.CurrentLevel.data.GetRepresentation<LBSGraphData>();
                current.AddEdge(edge);
                AddEdgeView(edge);
            }

            first = null;
            isDragEdge = false;
        }

        public override void ClearSelection()
        {
            base.ClearSelection();
            if (selection.Count == 0)
            {
                LBSController.ShowLevelInspector();
            }
        }


    }
}


