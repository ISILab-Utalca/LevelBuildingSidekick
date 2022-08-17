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
        private CreateEdge createEdge;

        public new class UxmlFactory : UxmlFactory<LBSGraphView, GraphView.UxmlTraits> { }

        //Vector2 scrollPos;
        Vector2 scrollPosition;
        private LBSGraphController controller;
        public LBSGraphController Controller
        {
            get
            {
                if(controller == null)
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
        }

        public void PopulateView()
        {
            DeleteElements(graphElements);

            Controller.Nodes.ForEach(n => {
                AddNodeView(n);
            });

            controller.Edges.ForEach(e => {
                AddEdgeView(e);
            });
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var pos = (evt.localMousePosition - new Vector2(viewTransform.position.x, viewTransform.position.y)) / scale;

            evt.menu.AppendAction("New Node", (a) =>
            {
                var data = Controller.NewNode(pos);
                AddNodeView(data);
            });

            evt.menu.AppendAction("New Conection", (a) =>
            {
                var n1 = controller.Nodes[0]; // temp
                var n2 = controller.Nodes[1]; // temp
                var edge = Controller.NewEdge(n1,n2);
                AddEdgeView(edge);
            });


            evt.menu.AppendAction("Clean", (Action<DropdownMenuAction>)((a) =>
            {
                this.Controller.Clear();
                DeleteElements(graphElements);
            }));

        }

 
        public void AddEdgeView(LBSEdgeData data)
        {
            var view = new LBSEdgeView();//new LBSEdgeView(data);
            AddElement(view);
            Debug.Log("C: "+ graphElements.Count());
        }

        public void AddNodeView(LBSNodeData data)
        {
            var view = new LBSNodeView(data);
            AddElement(view);
            //view.AddManipulator(createEdge);
        }
    
    }
}


