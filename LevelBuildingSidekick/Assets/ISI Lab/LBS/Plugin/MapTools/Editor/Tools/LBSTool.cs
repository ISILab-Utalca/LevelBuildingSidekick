using ISILab.LBS.Manipulators;
using ISILab.LBS.VisualElements;
using ISILab.LBS.VisualElements.Editor;
using LBS.Components;
using System;
using System.Collections.Generic;
using ISILab.LBS.Editor.Windows;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS
{
    public sealed class LBSTool
    {
        #region PROPERTIES
        public VectorImage Icon { get; }

        public string Name { get; }

        public string Description { get; }

        public LBSManipulator Manipulator { get; }

        #endregion

        #region EVENTS
        public event Action OnSelect;
        public event Action OnDeselect;
        public event Action<LBSLayer> OnStart;
        public event Action<LBSLayer> OnPressed;
        public event Action<LBSLayer> OnEnd;
        #endregion

        #region CONSTRUCTORS
        public LBSTool(LBSManipulator manipulator)
        {
            this.Manipulator = manipulator;
            Name = manipulator.Name;
            Description = manipulator.Description;
            Icon =  manipulator.Icon;
        }
        #endregion

        #region METHODS
        public void Init(LBSLayer layer, object behaviour)
        {
            // Layer was assigned already - unsubscribe old methods
            if(Manipulator.Layer != null)
            {
                Manipulator.OnManipulationStart -= () => { OnStart?.Invoke(Manipulator.Layer); };
                Manipulator.OnManipulationUpdate -= () => { OnPressed?.Invoke(Manipulator.Layer); };
                Manipulator.OnManipulationEnd -= () => { OnEnd?.Invoke(Manipulator.Layer); };
            }
            
            Manipulator.OnManipulationStart += () => { OnStart?.Invoke(layer); };
            Manipulator.OnManipulationUpdate += () => { OnPressed?.Invoke(layer); };
            Manipulator.OnManipulationEnd += () => { OnEnd?.Invoke(layer); };

            Manipulator.Init(layer, behaviour);
        }
        

        public void BindButton(ToolButton button)
        {
            var canvas = MainView.Instance;
            
            button.OnFocusEvent += () =>
            {
                canvas.AddManipulator(this.Manipulator);
                OnSelect?.Invoke();

            };
            button.OnBlurEvent += () =>
            {
                canvas.RemoveManipulator(this.Manipulator);
                OnDeselect?.Invoke();
                LBSMainWindow.MessageManipulator("-");
            };
        }
        
        #endregion;
    }
}