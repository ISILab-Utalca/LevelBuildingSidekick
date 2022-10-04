using LBS.ElementView;
using LBS.Representation.TileMap;
using LBS.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Manipulators
{
    public class DeleteTileManipulator : MouseManipulator
    {
        private LBSTileMapController controller;
        private GenericGraphWindow window;

        public DeleteTileManipulator(GenericGraphWindow window, LBSTileMapController controller)
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