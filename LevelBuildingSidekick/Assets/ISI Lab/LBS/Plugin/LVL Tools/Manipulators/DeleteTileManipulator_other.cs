using LBS.ElementView;
using LBS.Representation.TileMap;
using LBS.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Manipulators
{
    public class DeleteTileManipulator : MouseManipulator
    {
        IRepController controller;
        LBSTileMapData data;

        // Events
        public event Action OnEndAction;

        public DeleteTileManipulator(LBSTileMapData data, IRepController controller)
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

    public class DeleteTileManipulator_other : MouseManipulator
    {
        private LBSTileMapController controller;
        private GenericLBSWindow window;

        public DeleteTileManipulator_other(GenericLBSWindow window, LBSTileMapController controller)
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            this.controller = controller;
            this.window = window;
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

            var tile = e.target as LBSTileView;
            if (tile != null)
            {
                controller.RemoveTile(tile.Data);
                window.RefreshView();
                return;
            }
        }
    }
}