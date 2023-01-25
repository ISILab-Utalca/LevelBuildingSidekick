using LBS.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

namespace LBS.Manipulators
{
    public class AddStampManipulator : MouseManipulator
    {
        private LBSStampTileMapController controller;
        private GenericLBSWindow window;

        public AddStampManipulator(GenericLBSWindow window, LBSStampTileMapController controller)
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
            var selected = BrushWindow.SelectedStamp;
            if (selected == null)
                return;

            var pos = controller.ViewportMousePosition(e.localMousePosition);
            //Ugly Fix(!)
            int dy = pos.y < 0 ? -1 : 0;
            int dx = pos.x < 0 ? -1 : 0;
            var dp = new Vector2Int(dx,dy);

            var tPos = controller.ToTileCoords(pos);
            //var stamps = LBSController.CurrentLevel.data.GetRepresentation<LBSStampGroupData>();
            var stamp = new StampData(selected.Label, tPos + dp);
            //stamps.AddStamp(stamp);
            window.RefreshView();
            //controller.CreateStamp(e.localMousePosition, window.MainView, selected);
        }
    }
}
