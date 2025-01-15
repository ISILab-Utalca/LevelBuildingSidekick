using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class Select : LBSManipulator
    {
        private LBSLayer layer;
        private LBSLocalCurrent current;

        public Select()
        {
            // Unset feedback
            feedback = null;

            current = LBSInspectorPanel.Instance.data;
        }

        public override void Init(LBSLayer layer, object provider)
        {
            // Set layer reference
            this.layer = layer;

            // Set provider reference
            current = provider as LBSLocalCurrent;

        }

        protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
        {
           
            current = LBSInspectorPanel.Instance.data;

            // Get fixed position

            // Get selectable elements
            var selected = new List<object>();
            foreach (var module in layer.Modules)
            {
                if (module is ISelectable)
                {
                    selected.AddRange((module as ISelectable).GetSelected(position));
                }
            }

            current.SetSelectedVE(selected);
        }
    }
}