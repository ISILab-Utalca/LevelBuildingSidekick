using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Manipulators;
using ISILab.LBS.VisualElements.Editor;
using ISILab.Macros;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.VisualElements
{
    /// <summary>
    /// Represents a visual element on the quest graph used to indicate a trigger area or region.
    /// 
    /// This element is associated with a <see cref="BaseQuestNodeData"/> and draws a visual box on the graph.
    /// 
    /// Supports interaction such as:
    /// - Dragging to reposition
    /// - Resizing via a handle
    /// - Updating the logical data when moved
    /// - An Icon that represents the node type for easier readability.
    /// 
    /// Also handles custom visual generation through <see cref="MeshGenerationContext"/> to draw lines between this element and its node origin.
    /// </summary>
    public sealed class TriggerElementArea : GraphElement
    {
        private readonly BaseQuestNodeData _data;
        private Color _currentColor;
        
        private string _activeHandle;
        private const string HandleBottomLeft = "bl";
        private const string HandleBottomRight = "br";
        private const string HandleTopLeft = "tl";
        private const string HandleTopRight = "tr";

        private const float GraphGridLength = 100;
        
        private bool _resizing;
        private readonly bool _isCenter;
        private bool _isDragging;
        
        private Vector2 _dragStartMouse;
        private Vector2 _dragStartPosition;
        private Vector2 _resizeStartPosition;

        public TriggerElementArea(BaseQuestNodeData data, Rect area, bool centerTarget = true)
        {
            _isCenter = centerTarget;
            _data = data;

            VisualTreeAsset visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("TriggerElementArea");
            visualTree.CloneTree(this);

            _currentColor = data.Color;

            // Calculate initial visual position
            var position = LBSMainWindow.Instance._selectedLayer.FixedToPosition(
                new Vector2Int((int)area.x, (int)area.y), true);
            Rect drawArea = new Rect(position, new Vector2(area.width * GraphGridLength, area.height * GraphGridLength));

            SetPosition(drawArea);

            VisualElement triggerElementGizmo = this.Q<VisualElement>("TriggerElementSelector");

            // Styling
            Color backgroundColor = _currentColor;
            backgroundColor.a = 0.2f;
            triggerElementGizmo.style.backgroundColor = backgroundColor;
            triggerElementGizmo.style.unityBackgroundImageTintColor = backgroundColor;
            triggerElementGizmo.style.borderBottomColor = _currentColor;
            triggerElementGizmo.style.borderTopColor = _currentColor;
            triggerElementGizmo.style.borderRightColor = _currentColor;
            triggerElementGizmo.style.borderLeftColor = _currentColor;


            var targetIcon = this.Q<VisualElement>("TargetIcon");
            targetIcon.style.backgroundImage = new StyleBackground(data.GetIcon());

            var cornerTargetIcon = this.Q<VisualElement>("CornerTargetIcon");
            cornerTargetIcon.style.backgroundImage = new StyleBackground(data.GetIcon());

            targetIcon.style.display = _isCenter ? DisplayStyle.Flex : DisplayStyle.None;
            cornerTargetIcon.style.display = _isCenter ? DisplayStyle.None : DisplayStyle.Flex;
            
            SetupResizeHandle("Handle_bl", HandleBottomLeft, _isCenter);
            SetupResizeHandle("Handle_br", HandleBottomRight, _isCenter);
            SetupResizeHandle("Handle_tl", HandleTopLeft, _isCenter);
            SetupResizeHandle("Handle_tr", HandleTopRight, _isCenter);
            
            
            // Register mouse callbacks on the whole element
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            RegisterCallback<MouseUpEvent>(OnMouseUp);
            
            
            generateVisualContent -= OnGenerateVisualContent;
            generateVisualContent += OnGenerateVisualContent;
        }

        
        void SetupResizeHandle(string handleName, string handleCode, bool isCenter)
        {
            var handle = this.Q<VisualElement>(handleName);
            handle.style.display = isCenter ? DisplayStyle.Flex : DisplayStyle.None;
            var handleArea = handle.Q<VisualElement>("handleArea");
            
            
            // can only resize the main trigger area of a quest action node
            if (!isCenter) return;
            
            handle.RegisterCallback<MouseLeaveEvent>(_ =>
            {
                _resizing = false;
                _activeHandle = null;
                handleArea.style.display = DisplayStyle.None;
            });
            
            handle.RegisterCallback<MouseEnterEvent>(_ =>
            {
                // only one resizer at a time
                if (_resizing) return;
                
                _resizeStartPosition = GetPosition().position; 
                
                _resizing = true;
                _activeHandle = handleCode;
                handleArea.style.display = DisplayStyle.Flex;
            });

            handle.RegisterCallback<MouseUpEvent>(_ =>
            {
                _resizing = false;
                handleArea.style.display = DisplayStyle.None;

                if (_data.Layer is null) return;

                Rect currentRect = GetPosition();

                float deltaX = currentRect.x - _resizeStartPosition.x;
                float deltaY = currentRect.y - _resizeStartPosition.y;

       
                // Round position and size by 100
                float posX = Mathf.Round(_resizeStartPosition.x / GraphGridLength);
                float posY = -Mathf.Round(_resizeStartPosition.y / GraphGridLength);
                float width = Mathf.Round(currentRect.width / GraphGridLength);
                float height = Mathf.Round(currentRect.height / GraphGridLength);

                int deltaTileX = Mathf.RoundToInt(deltaX / GraphGridLength);
                int deltaTileY = Mathf.RoundToInt(deltaY / GraphGridLength);

                if (_activeHandle == HandleTopLeft)
                {
                    posX += deltaTileX;
                    posY -= deltaTileY;
                }
                else if (_activeHandle == HandleTopRight)
                {
                    posX -= deltaTileX;
                    posY -= deltaTileY;
                }
                else if (_activeHandle == HandleBottomLeft)
                {
                    posX += deltaTileX;
                    posY += deltaTileY;
                }
                // BottomRight does’t change origin
                
                // Update the logical area in tile space
                _data.Area = new Rect(posX, posY, width, height);
                _data.Graph?.DataChanged(_data.OwnerNode);

                _activeHandle = null;
            });

            // Hide the areas by default(show when click on handle, hide on mouse up)
            handleArea.style.display = DisplayStyle.None;
            handle.RegisterCallback<MouseMoveEvent>(OnHandleRectMove);
        }
        
        /// <summary>
        /// Draws a dotted line from the NodeView to the Trigger center
        /// </summary>
        /// <param name="mgc"></param>
        void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            if(!_isCenter) return;
            var painter = mgc.painter2D;
            var lbsLayer = _data.Layer;
            
            var nodeElements = MainView.Instance.GetElementsFromLayerContainer(lbsLayer, _data.ID);

            var node = nodeElements?.FirstOrDefault();
            if (node == null) return;

            Vector2 center = new Vector2(GetPosition().width / 2f, GetPosition().height / 2f);
            Rect nodeRect = node.worldBound;
            Vector2 nodeWorldCenter = nodeRect.position + nodeRect.size / 2f;
            Vector2 to = this.WorldToLocal(nodeWorldCenter); // convert world to local space

            if (_isDragging) _currentColor = new Color(0, 0, 0, 0); // transparent if moving
            painter.DrawDottedLine(center, to, _currentColor, 4f, 10f);
        }


        private void OnMouseDown(MouseDownEvent e)
        {
            // If resizing do NOT MOVE
            if(_resizing) return;
            
            if (e.button != 0) return;
            _isDragging = true;
            _dragStartMouse = e.mousePosition;
            
            
            var tilePosition = new Vector2Int((int)_data.Area.x, (int)_data.Area.y);
            _dragStartPosition = LBSMainWindow.Instance._selectedLayer.FixedToPosition(tilePosition, true);

            e.StopPropagation();
        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            // If resizing do NOT MOVE
            if(_resizing) return;
            
            if (!_isDragging || e.pressedButtons != 1) return;
            if (!MainView.Instance.HasManipulator<SelectManipulator>()) return;

            var scale = MainView.Instance.viewTransform.scale;

            Vector2 delta = (e.mousePosition - _dragStartMouse) / scale;
            Vector2 newPos = _dragStartPosition + delta;

            Rect newRect = new Rect(newPos, GetPosition().size);
            SetPosition(newRect);
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            // If resizing do NOT MOVE
            if(_resizing) return;
            
            if (!_isDragging) return;
            _isDragging = false;

            _data.Area = new Rect(Mathf.Round(GetPosition().x/GraphGridLength), -Mathf.Round(GetPosition().y/GraphGridLength), _data.Area.width, _data.Area.height);
            _data.Graph?.DataChanged(_data.OwnerNode);
        }

        void OnHandleRectMove(MouseMoveEvent e)
        {
            if (!_resizing || string.IsNullOrEmpty(_activeHandle)) return;
            if (e.pressedButtons != 1 || e.button != 0) return;

            var scale = MainView.Instance.viewTransform.scale;
            Vector2 delta = e.mouseDelta / scale;
            Rect currentRect = GetPosition();

            float newX = currentRect.x;
            float newY = currentRect.y;
            float newWidth = currentRect.width;
            float newHeight = currentRect.height;

            if (_activeHandle.Contains("l"))
            {
                newX += delta.x;
                newWidth -= delta.x;
            }
            if (_activeHandle.Contains("r"))
            {
                newWidth += delta.x;
            }
            if (_activeHandle.Contains("t"))
            {
                newY += delta.y;
                newHeight -= delta.y;
            }
            if (_activeHandle.Contains("b"))
            {
                newHeight += delta.y;
            }

            // Clamp minimum size
            newWidth = Mathf.Max(newWidth, 20);
            newHeight = Mathf.Max(newHeight, 20);

            SetPosition(new Rect(newX, newY, newWidth, newHeight));
            e.StopPropagation();
        }

    }
}
