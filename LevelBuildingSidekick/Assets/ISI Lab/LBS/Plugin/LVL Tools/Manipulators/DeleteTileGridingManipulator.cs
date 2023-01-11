using LBS.ElementView;
using LBS.Representation.TileMap;
using LBS.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using MouseButton = UnityEngine.UIElements.MouseButton;

namespace LBS.Manipulators
{
    public class DeleteTileGridingManipulator : MouseManipulator
    {
        IRepController controller;
        LBSTileMapData data;

        // Events
        public event Action OnEndAction;

        public DeleteTileGridingManipulator(LBSTileMapData data, IRepController controller)
        {
            this.controller = controller;
            this.data = data;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            var t = e.target as LBSGraphElement;
            if (t == null)
                return;

            var tile = e.target as TileView;
            if (tile != null)
            {
                var pos = controller.ViewportMousePosition(e.localMousePosition);
                Vector2Int tpos = ToTileCoords(pos, 100);
                data.RemoveTile(tpos);
                OnEndAction?.Invoke();
                return;
            }
        }

        public Vector2Int ToTileCoords(Vector2 pos, float size) // (!) esto esta duplicado en otro manipulator, podria heredarse
        {
            int x = (pos.x > 0) ? (int)(pos.x / size) : (int)(pos.x / size) - 1;
            int y = (pos.y > 0) ? (int)(pos.y / size) : (int)(pos.y / size) - 1;

            return new Vector2Int(x, y);
        }
    }

    public class DeleteTileGridingManipulator_Scheme : MouseManipulator
    {
        private LBSTileMapController controller;
        private GenericLBSWindow window;

        private bool activeDragging = false;

        //pos1 -> first tile clicked
        //pos2 -> last tile hovered
        private Vector2 pos1 = new Vector2();
        private Vector2 pos2 = new Vector2();

        public DeleteTileGridingManipulator_Scheme(GenericLBSWindow window, LBSTileMapController controller)
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            this.controller = controller;
            this.window = window;
        }
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            pos1 = controller.ViewportMousePosition(e.localMousePosition);
            activeDragging = true;
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            pos2 = controller.ViewportMousePosition(e.localMousePosition);
            activeDragging = false;
        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            if (!activeDragging) return;

            var tPos1 = controller.ToTileCoords(pos1);
            var tPos2 = controller.ToTileCoords(pos2);
            var schema = LBSController.CurrentLevel.data.GetRepresentation<LBSSchemaData>();

            for (int i = tPos1.y; i <= tPos2.y; i++)
            {
                for (int j = tPos1.x; j <= tPos2.x; j++)
                {
                    var t = e.target as LBSGraphElement;
                    if (t == null)
                        return;
                    var tile = e.target as LBSTileView;
                    if (tile.GetPosition().y == tPos1.y && tile.GetPosition().x == tPos1.x && tile != null)
                    {
                        controller.RemoveTile(tile.Data);
                        window.RefreshView();
                        return;
                    }
                }
            }

            

            
        }
    }
}