using LBS.Windows;
using LBS.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Representation.TileMap;
using LBS.Representation;
using System;
using LBS.ElementView;
using static UnityEngine.ParticleSystem;
using static UnityEditor.PlayerSettings;
using Unity.VisualScripting;
using MouseButton = UnityEngine.UIElements.MouseButton;

namespace LBS.Manipulators
{
    public class AddTileWithBrushManipulator : MouseManipulator
    {
        IRepController controller;
        LBSTileMapData data;

        // Events
        public event Action OnEndAction;

        public AddTileWithBrushManipulator(LBSTileMapData data, IRepController controller)
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
            var pos = controller.ViewportMousePosition(e.localMousePosition);
            var tPos = ToTileCoords(pos, 100); // (!) pasar mejor los paramtros no enduro
            data.AddTile(new TileData(tPos, 0, new string[4] { "", "", "", "" }));
            OnEndAction?.Invoke();
        }

        public Vector2Int ToTileCoords(Vector2 pos, float size)
        {
            int x = (pos.x > 0) ? (int)(pos.x / size) : (int)(pos.x / size) - 1;
            int y = (pos.y > 0) ? (int)(pos.y / size) : (int)(pos.y / size) - 1;

            return new Vector2Int(x, y);
        }
    }

    public class AddTileWithBrushManipulatorSchema : MouseManipulator
    {
        private LBSTileMapController controller;
        private GenericLBSWindow window;
        private RoomData cRoom;
        private bool activeDragging = false;

        private Vector2 pos1 = new Vector2();
        private Vector2 pos2 = new Vector2();

        public AddTileWithBrushManipulatorSchema(GenericLBSWindow window, LBSTileMapController controller, RoomData cRoom)
        {
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            this.controller = controller;
            this.window = window;
            this.cRoom = cRoom;
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
            activeDragging = true;
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            activeDragging = false;
        }


        //Dragging draw brush
        private void OnMouseMove(MouseMoveEvent e)
        {
            Debug.LogWarning("Dragging");

            if (cRoom == null)
            {
                Debug.LogWarning("No room selected");
                return;
            }

            var pos = controller.ViewportMousePosition(e.localMousePosition);
            var tPos = controller.ToTileCoords(pos);
            var schema = LBSController.CurrentLevel.data.GetRepresentation<LBSSchemaData>();
            if (activeDragging)
            {
                var tile = new TileData(tPos, 0, new string[4]); // (!) esto solo esta para 4 conectados 
                schema.AddTile(tile, cRoom.ID);
                window.RefreshView();
            }
        }
    }
}
