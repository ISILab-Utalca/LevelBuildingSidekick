using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using LevelBuildingSidekick;
using System;
using System.Linq;
using UnityEngine.UIElements;

namespace LevelBuildingSidekick.Graph
{
    public class LBSGraphView : GraphView
    {
        public static bool isDragEdge = false;
        private static LBSNodeView first;

        public new class UxmlFactory : UxmlFactory<LBSGraphView, GraphView.UxmlTraits> { }

        //Vector2 scrollPos;
        Vector2 scrollPosition;
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
            //Controller = new LBSGraphController();
            var gb = new GridBackground();
            gb.StretchToParentSize();
            Insert(0, gb);

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            //createEdge = new CreateEdge(controller, this);
            //this.AddManipulator(createEdge);

            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("GraphWindow");
            //style.backgroundColor = new Color(79, 79, 79);;
            styleSheets.Add(styleSheet);
            LBSController.ShowLevelInspector();
        }

        public void PopulateView()
        {
            DeleteElements(graphElements);

            var graph = LBSController.CurrentLevel.data.GetRepresentation<LBSGraphData>();

            graph.GetNodes().ForEach(n => AddNodeView(n));
            graph.GetEdges().ForEach(e => AddEdgeView(e));
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

            evt.menu.AppendAction("Clean", (Action<DropdownMenuAction>)((a) =>
            {
                this.Controller.Clear();
                DeleteElements(graphElements);
            }));

            evt.menu.AppendAction("Save", a =>
            {
                Utility.JSONDataManager.SaveData("LBSLevels", "test1", LBSController.CurrentLevel);
            });

            evt.menu.AppendAction("Debug/print info",(a) => {
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
                if(e.button == 1)
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
            var nv1 = nodeViews.Find(n => n.Data.Label == edge.FirstNodeLabel);
            var nv2 = nodeViews.Find(n => n.Data.Label == edge.SecondNodeLabel);

            if (nv1 == null || nv2 == null)
            {
                Debug.LogWarning("There is no 'NodeView' to which to link this 'EdgeView'.");
                return;
            }

            var edgeView = new LBSEdgeView(nv1, nv2, this);
            nv1.OnMoving += edgeView.UpdateDots;
            nv2.OnMoving += edgeView.UpdateDots;
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
                if(e is LBSEdgeView)
                {
                    var edge = (LBSEdgeView)e;
                    //edge.UpdateDraw();
                }
            }
        }

        public void StartDragEdge(LBSNodeData data)
        {
            first = GetNodeViewBylabel(data.Label);
            //proxyEdge = new LBSProxyEdge(first.GetPosition().position,new Vector2(0,0));
            //AddElement(proxyEdge);
            isDragEdge = true;
        }

        public void EndDragEdge(LBSNodeData data)
        {
            if (first != null)
            {
                var second = GetNodeViewBylabel(data.Label);
                var edge = new LBSEdgeData(first.Data, second.Data);
                AddEdgeView(edge);
            }

            //RemoveElement(proxyEdge);
            //proxyEdge = null;
            first = null;
            isDragEdge = false;
        }

        public override void ClearSelection()
        {
            base.ClearSelection();
            if(selection.Count == 0)
            {
                LBSController.ShowLevelInspector();
            }
        }
    }
}


