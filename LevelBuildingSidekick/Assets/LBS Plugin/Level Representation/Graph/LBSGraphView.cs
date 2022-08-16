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
        public new class UxmlFactory : UxmlFactory<LBSGraphView, GraphView.UxmlTraits> { }

        //Vector2 scrollPos;
        Vector2 scrollPosition;
        public LBSGraphController controller;

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
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("GraphWindow"); 
            //style.backgroundColor = new Color(79, 79, 79);
            styleSheets.Add(styleSheet);

        }

        public void PopulateView()
        {

            controller.Nodes.ToList().ForEach(n => AddElement(new LBSNodeView(n)));
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var pos = (evt.localMousePosition - new Vector2(viewTransform.position.x, viewTransform.position.y)) / scale;
            {
                evt.menu.AppendAction("New Node", (a) => AddNode(pos));
            }        
        }

        public void AddNode(Vector2 pos)
        {
            var n = controller.NewNode(pos);
            AddElement(new LBSNodeView(n));
        }


        public void Draw2D()
        {
            /*
            var controller = Controller as LBSGraphController;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            var offset = controller.CellSize * Vector2Int.one;

            var lastElement = controller.FartherPosition();
            var size = lastElement + offset;

            var r = GUILayoutUtility.GetRect(10*offset.x,
                10*offset.y,
                size.x,
                size.y);

            GUI.DrawTexture(r, new Texture2D(1, 1));

            foreach (LBSNodeController n in controller.Nodes)
            {
                (n.View as View).Draw2D();
            }
            foreach (EdgeController e in controller.Edges)
            {
                (e.View as View).Draw2D();
            }
            GUILayout.EndScrollView();
            */
        }
    }
}


