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
    public class AddDoorManipulator : MouseManipulator
    {
        private LBSTileMapController controller;
        private GenericGraphWindow window;

        private TileData first;

        public AddDoorManipulator(GenericGraphWindow window, LBSTileMapController controller)
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
            var tile = e.target as LBSTileView;
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

            var tile = e.target as LBSTileView;
            if (tile == null)
                return;

            var second = tile.Data;

            if (first.GetRoomID().Equals(second.GetRoomID()))
                return;

            var dx = Mathf.Abs(first.GetPosition().x - second.GetPosition().x);
            var dy = Mathf.Abs(first.GetPosition().y - second.GetPosition().y);
            if (dx + dy > 1f)
                return;

            var door = new DoorData(
                first.GetRoomID(),
                second.GetRoomID(),
                first.GetPosition(),
                second.GetPosition()
                );

            controller.AddDoor(door);
            window.RefreshView();
        }
    }
}