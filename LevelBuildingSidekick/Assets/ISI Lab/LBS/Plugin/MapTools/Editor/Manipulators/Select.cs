using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using LBS.Components;
   
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class Select : LBSManipulator
    {
        private LBSLocalCurrent current;

        public Select()
        {
            // Unset feedback
            feedback = null;

            current = LBSInspectorPanel.Instance.data;
        }

        public override void Init(LBSLayer layer, object provider)
        {
            base.Init(layer, provider);

            // Set provider reference
            current = provider as LBSLocalCurrent;
            
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
        {
           
            current = LBSInspectorPanel.Instance.data;
            
            // Get selectable elements
            var selected = new List<object>();
            foreach (var module in lbsLayer.Modules)
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