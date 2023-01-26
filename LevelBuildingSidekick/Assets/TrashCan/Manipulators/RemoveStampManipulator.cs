using LBS.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Manipulators
{
    public class RemoveStampManipulator : MouseManipulator
    {
        private LBSStampTileMapController controller;
        private GenericLBSWindow window;

        public RemoveStampManipulator(GenericLBSWindow window, LBSStampTileMapController controller)
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
            var t = e.target as LBSStampView;
            if (t == null)
                return;

            //var stamps = LBSController.CurrentLevel.data.GetRepresentation<LBSStampGroupData>();
            //stamps.RemoveStamp(t.Data);
            window.RefreshView();
        }
    }
}