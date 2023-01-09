using LBS.ElementView;
using LBS.Representation;
using LBS.Representation.TileMap;
using LBS.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Manipulators
{
    public class AddConnectionManipulator : MouseManipulator
    {
        private TileData first;

        IRepController controller;
        LBSTileMapData data;
        WFCTools.ColorTag ct;

        // Events
        public event Action OnEndAction;

        public AddConnectionManipulator(WFCTools.ColorTag tag,LBSTileMapData data, IRepController controller)
        {
            this.controller = controller;
            this.data = data;
            ct = tag;
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
            var tile = e.target as TileView;
            if (tile == null)
                return;

            var pos = controller.ViewportMousePosition(e.localMousePosition);
            var tpos = ToTileCoords(pos, 100);
            first = data.GetTile(tpos);
        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            //Debug.Log("Move drag");
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            if (first == null)
                return;

            var tile = e.target as TileView;
            if (tile == null)
                return;

            var pos = controller.ViewportMousePosition(e.localMousePosition);
            var tpos = ToTileCoords(pos, 100);
            var second = data.GetTile(tpos);


            var dx = Mathf.Abs(first.Position.x - second.Position.x);
            var dy = Mathf.Abs(first.Position.y - second.Position.y);
            if (dx + dy > 1f)
                return;

            var dir1 = TileMapUtils.CalcDir4Connected(first.Position, second.Position);
            var dir2 = TileMapUtils.CalcDir4Connected(second.Position, first.Position);
            first.SetConection(dir1, ct.tag);
            second.SetConection(dir2, ct.tag);
            OnEndAction?.Invoke();
        }

        public Vector2Int ToTileCoords(Vector2 pos, float size) // (!) esto esta duplicado en otro manipulator, podria heredarse
        {
            int x = (pos.x > 0) ? (int)(pos.x / size) : (int)(pos.x / size) - 1;
            int y = (pos.y > 0) ? (int)(pos.y / size) : (int)(pos.y / size) - 1;

            return new Vector2Int(x, y);
        }
    }

    public class AddDoorManipulator : MouseManipulator
    {
        private LBSTileMapController controller;
        private GenericLBSWindow window;

        private TileData first;

        public AddDoorManipulator(GenericLBSWindow window, LBSTileMapController controller)
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
            
            var schema = controller.GetData() as LBSSchemaData;
            var r1 = schema.GetRoom(first.Position);
            var r2 = schema.GetRoom(second.Position);
            if (r1.Equals(r2))
                return;

            var dx = Mathf.Abs(first.GetPosition().x - second.GetPosition().x);
            var dy = Mathf.Abs(first.GetPosition().y - second.GetPosition().y);
            if (dx + dy > 1f)
                return;

            var door = new DoorData(
                first.GetPosition(),
                second.GetPosition()
                );

            controller.AddDoor(door);
            window.RefreshView();
        }
    }
}
