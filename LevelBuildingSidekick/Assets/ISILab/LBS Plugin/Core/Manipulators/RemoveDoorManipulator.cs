using LBS.ElementView;
using LBS.Representation;
using LBS.Representation.TileMap;
using LBS.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Manipulators
{
    public class RemoveDoorManipulator : MouseManipulator
    {
        private LBSTileMapController controller;
        private GenericGraphWindow window;

        private TileData first;

        public RemoveDoorManipulator(GenericGraphWindow window, LBSTileMapController controller)
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.RightMouse });
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
            var tile = e.target as TileSchema;
            if (tile == null)
                return;

            first = tile.Data;

        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            //Debug.Log("Move drag");
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            if (first == null)
                return;

            var tile = e.target as TileSchema;
            if (tile == null)
                return;

            var door = controller.GetDoor(first,tile.Data);
            if(door != null)
            {
                controller.RemoveDoor(door);
            }
            window.RefreshView();
        }
    }
}
