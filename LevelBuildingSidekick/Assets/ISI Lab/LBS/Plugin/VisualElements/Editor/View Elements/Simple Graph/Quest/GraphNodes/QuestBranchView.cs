using System;
using ISILab.Commons.Utility.Editor;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Components;
using ISILab.LBS.Manipulators;
using LBS.VisualElements;
using UnityEditor;

namespace ISILab.LBS.VisualElements
{
    public class QuestBranchView : QuestGraphNodeView
    {
        #region Static Assets
        private static VisualTreeAsset _rootAsset;
        #endregion

        #region UI Elements
        private readonly VisualElement _root;
        private VisualElement _or;
        private VisualElement _and;
        #endregion

        public QuestBranchView(GraphNode graphNode)
        {
            if (_rootAsset == null)
                _rootAsset = DirectoryTools.GetAssetByName<VisualTreeAsset>("QuestBranchView");

            _rootAsset.CloneTree(this);

            _root = this.Q<VisualElement>("Root");
            InvalidConnectionIcon = this.Q<VisualElement>("InvalidConnectionIcon");

            InvalidConnectionIcon.style.unityBackgroundImageTintColor = InvalidGrammarColor;
            
            VisualElement coloredVe = this.Q<VisualElement>("Root");
            coloredVe.style.backgroundColor = DefaultBackgroundColor;
            
            Node = graphNode ?? throw new ArgumentNullException(nameof(graphNode));
            
            _or = this.Q<VisualElement>("OrVe");
            _and = this.Q<VisualElement>("AndVe");
            
            if(graphNode is OrNode) _or.style.display = DisplayStyle.Flex;
            if(graphNode is AndNode) _and.style.display = DisplayStyle.Flex;

            SetPosition(new Rect(Node.NodeViewPosition.position, Vector2.one));

            RegisterCallbacks();
            Update();
        }

        #region Callbacks
        private void RegisterCallbacks()
        {
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            RegisterCallback<GeometryChangedEvent>(_ => Update());

            OnMoving += rect => Node.NodeViewPosition = rect;
        }
        #endregion

        #region Update
        private void Update()
        {
            DisplayGrammarState(Node);
            SetPosition(new Rect(GetPosition().position, new Vector2(_root.resolvedStyle.width, _root.resolvedStyle.height)));
            OnMoving?.Invoke(GetPosition());
        }

        public override void DisplayGrammarState(GraphNode node)
        {
            base.DisplayGrammarState(node);
            _root.SetBorder(!node.ValidConnections ? InvalidGrammarColor : ValidGrammarColor, 1f);
            _or.SetBorder(!node.ValidConnections ? InvalidGrammarColor : ValidGrammarColor, 1f);
            _and.SetBorder(!node.ValidConnections ? InvalidGrammarColor : ValidGrammarColor, 1f);
        }
        
        protected override void OnMouseDown(MouseDownEvent evt)
        {
            base.OnMouseDown(evt);

            if (evt.button == 1)
            {
                MakeMenu(evt);
            }
        }

        private void MakeMenu(MouseDownEvent evt)
        {
            // Create the menu
            var menu = new GenericMenu();

            if (Node.GetType() == typeof(AndNode))
            {
                menu.AddItem(new GUIContent("Set as 'Or branch' "), false, () =>
                {
                    Debug.Log("Set as Or branch");
                });
            }
    
            if (Node.GetType() == typeof(OrNode))
            {
                menu.AddItem(new GUIContent("Set as 'And branch' "), false, () =>
                {
                    Debug.Log("Set as And branch");
                });
            }

            menu.AddItem(new GUIContent("Delete node"), false, () =>
            {
                Debug.Log("Delete node");
            });
            
            menu.ShowAsContext();
            evt.StopPropagation();
        }

        #endregion
    }
}
