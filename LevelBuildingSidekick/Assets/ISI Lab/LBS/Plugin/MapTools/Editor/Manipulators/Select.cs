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
        protected override string IconGuid { get => "77f81c1ea560ddf4c99e41c605166e3e" ; }
        public Select():base()
        {
            // Unset feedback
            feedback = null;
            current = LBSInspectorPanel.Instance.data;

            name = "Select";
            description = "Selection";
        }
        
        public override void Init(LBSLayer layer, object provider)
        {
            base.Init(layer, provider);
            // Set provider reference
            current = provider as LBSLocalCurrent;
        }

        protected override void OnMouseUp(VisualElement paramTarget, Vector2Int position, MouseUpEvent e)
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