
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Modules;
using UnityEditor;

namespace ISILab.LBS.VisualElements
{
    // Class that draws the quest edges.
    public class LBSQuestEdgeView : GraphElement
    {
        private Vector2Int _pos1, _pos2;

        private const int PosOffset = 16;

        private readonly QuestEdge _edge;

        private readonly VisualElement connectionTypeView;
        
        public LBSQuestEdgeView(QuestEdge edge, QuestNodeView node1, QuestNodeView node2, int l, int stroke)
        {
            // Set Data
            this._edge = edge;

            var visualTree = Resources.Load<VisualTreeAsset>("EdgeConnectionView");
            if (visualTree != null)
            {
                var rootFromUxml = visualTree.CloneTree();
                Add(rootFromUxml);
            }
            
            connectionTypeView = this.Q<VisualElement>("View");
            if (connectionTypeView != null)
            {
                // Subscribe to draw arrow inside the "View"
                connectionTypeView.generateVisualContent += DrawArrowInView;
            }
            
            // Set first node
            node1.OnMoving += (rect) =>
            {
                SetPosition(new Rect(_pos1, new Vector2(10, 10)));
                ActualizePositions(rect.center.ToInt(), _pos2);
            };

            // Set second node
            node2.OnMoving += (rect) =>
            {
                ActualizePositions(_pos1, rect.center.ToInt());
            };

            var sPos1 = new Vector2Int((int)node1.GetPosition().xMax - PosOffset, (int)node1.GetPosition().center.y);
            var sPos2 = new Vector2Int((int)node2.GetPosition().xMin + PosOffset, (int)node2.GetPosition().center.y);
            ActualizePositions(sPos1, sPos2);

            SetPosition(new Rect(_pos1, new Vector2(10, 10)));
            
            generateVisualContent -= OnGenerateVisualContent;
            generateVisualContent += OnGenerateVisualContent;
            
            RegisterCallback<MouseUpEvent>(OnMouseUp);
        }
        
        private void OnMouseUp(MouseUpEvent evt)
        {
            // only on Right-click
            if (evt.button != 1) return; 

            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Set to Single"), _edge.EdgeType == ConnectionType.Single, () =>
            {
                _edge.SetConnectionType(ConnectionType.Single);
                MarkDirtyRepaint();
            });
            menu.AddItem(new GUIContent("Set to OR"), _edge.EdgeType == ConnectionType.Or, () =>
            {
                _edge.SetConnectionType(ConnectionType.Or);
                MarkDirtyRepaint();
            });
            menu.AddItem(new GUIContent("Set to AND"), _edge.EdgeType == ConnectionType.And, () =>
            {
                _edge.SetConnectionType(ConnectionType.And);
                MarkDirtyRepaint();
            });
            menu.ShowAsContext();
        }

        void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            var painter = mgc.painter2D;
            // line
            painter.DrawDottedLine(Vector2.zero , _pos2 - _pos1, Color.white, 3f, 5f);
            
            // arrow
            Vector2Int pos = _pos2 - _pos1;
            Vector2 midpoint = (_pos2 - _pos1) / 2;
            Vector2 direction = new Vector2(pos.x, pos.y).normalized;

            painter.DrawEquilateralArrow(midpoint, direction, 15f, Color.white);
            
        }

        private void ActualizePositions(Vector2Int pos1, Vector2Int pos2)
        {
            this._pos1 = pos1;
            this._pos2 = pos2;
            MarkDirtyRepaint();
        }
    }
}