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

            Controller.Nodes.ForEach(n =>
            {
                AddNodeView(n);
            });

            controller.Edges.ForEach(e =>
            {
                var nv1 = GetNodeViewBylabel(e.FirstNodeLabel);
                var nv2 = GetNodeViewBylabel(e.SecondNodeLabel);
                AddEdgeView(nv1, nv2);
            });
        }

        public LBSNodeView GetNodeViewBylabel(string label)
        {
            foreach (var element in graphElements)
            {
                if (element is LBSNodeView)
                {
                    var n = (LBSNodeView)element;
                    if (n != null && n.Data.label == label)
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

            /*
            evt.menu.AppendAction("New Conection", (a) =>
            {
                var n1 = controller.Nodes[0]; // temp
                var n2 = controller.Nodes[1]; // temp
                var edge = Controller.NewEdge(n1, n2);
                var nv1 = GetNodeViewBylabel(n1.label);
                var nv2 = GetNodeViewBylabel(n2.label);
                AddEdgeView(nv1, nv2);
            });
            */

            evt.menu.AppendAction("Clean", (Action<DropdownMenuAction>)((a) =>
            {
                this.Controller.Clear();
                DeleteElements(graphElements);
            }));

            evt.menu.AppendAction("Save", a =>
            {
                Utility.JSONDataManager.SaveData("LBSLevels", "test1", LBSController.CurrentLevel);
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


        public void AddEdgeView(LBSNodeView nv1, LBSNodeView nv2)
        {
            var view = new LBSEdgeView(nv1, nv2, this);
            var l1 = nv1.Data.label;
            var l2 = nv2.Data.label;

            if (l1 == l2) // si son el mismo nodo no hago nada
                return;

            foreach (var e in controller.Edges) // recorro las conexiones
            {
                if (e.Contains(l1) && e.Contains(l2)) // si exite una conexion igual no hago nada
                    return;
            }

            nv1.OnMoving += view.UpdateDots;
            nv2.OnMoving += view.UpdateDots;
            AddElement(view);
            Debug.Log("C: " + graphElements.Count());
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
            first = GetNodeViewBylabel(data.label);
            //proxyEdge = new LBSProxyEdge(first.GetPosition().position,new Vector2(0,0));
            //AddElement(proxyEdge);
            isDragEdge = true;
        }

        public void EndDragEdge(LBSNodeData data)
        {
            if (first != null)
            {
                var second = GetNodeViewBylabel(data.label);
                AddEdgeView(first, second);
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


