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
using Unity.IO.LowLevel.Unsafe;

namespace LBS.Manipulators
{
    public class CreateTileDragingManipulator : MouseManipulator
    {
        IRepController controller;
        LBSTileMapData data;

        // Events
        public event Action OnEndAction;

        public CreateTileDragingManipulator(LBSTileMapData data, IRepController controller)
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

    public class CreateTileDragingManipulatorSchema : MouseManipulator
    {
        private LBSTileMapController controller;
        private GenericLBSWindow window;
        private RoomData cRoom;
        private bool activeDragging = false;

        //pos1 -> first tile clicked
        //pos2 -> last tile hovered
        private Vector2 pos1 = new Vector2();
        private Vector2 pos2 = new Vector2();

        public CreateTileDragingManipulatorSchema(GenericLBSWindow window, LBSTileMapController controller, RoomData cRoom)
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
            pos1 = controller.ViewportMousePosition(e.localMousePosition);
            activeDragging = true;
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            pos2 = controller.ViewportMousePosition(e.localMousePosition);
            activeDragging = false;
        }

        //Draging draw
        //for()
        //{
        //  for()
        //  {
        //      going through the first tile with "OnMouseDown" to where it is dropped with "OnMouseUp"
        //  }
        //}
        private void OnMouseMove(MouseMoveEvent e)
        {
            if (cRoom == null)
            {
                Debug.LogWarning("No room selected");
                return;
            }

            if (!activeDragging) return;

            var tPos1 = controller.ToTileCoords(pos1);
            var tPos2 = controller.ToTileCoords(pos2);
            //var schema = LBSController.CurrentLevel.data.GetRepresentation<LBSSchemaData>();
                    
            for (int i = tPos1.y; i <= tPos2.y; i++)
            {
                for (int j = tPos1.x; j <= tPos2.x; j++)
                {
                    var tile = new TileData(new Vector2Int(j, i), 0, new string[4]); // (!) esto solo esta para 4 conectados
                    //schema.AddTile(tile, cRoom.ID);
                    window.RefreshView();
                }
            }
        }
    }


}
