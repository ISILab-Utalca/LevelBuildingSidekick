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
    public class LBSTool
    {
        #region FIELDS
        private VectorImage icon;
        private string name;
        private string description;
        private LBSManipulator manipulator;
        #endregion

        #region PROPERTIES
        public VectorImage Icon => icon;
        public string Name => name;
        public string Description => description;
        public LBSManipulator Manipulator => manipulator;
        
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
            this.manipulator = manipulator;
            name = manipulator.Name;
            description = manipulator.Description;
            icon =  manipulator.Icon;
        }
        #endregion

        #region METHODS
        public virtual void Init(LBSLayer layer, object behaviour)
        {
            // Layer was assigned already - unsubscribe old methods
            if(manipulator.Layer != null)
            {
                manipulator.OnManipulationStart -= () => { OnStart?.Invoke(manipulator.Layer); };
                manipulator.OnManipulationUpdate -= () => { OnPressed?.Invoke(manipulator.Layer); };
                manipulator.OnManipulationEnd -= () => { OnEnd?.Invoke(manipulator.Layer); };
            }
            
            manipulator.OnManipulationStart += () => { OnStart?.Invoke(layer); };
            manipulator.OnManipulationUpdate += () => { OnPressed?.Invoke(layer); };
            manipulator.OnManipulationEnd += () => { OnEnd?.Invoke(layer); };

            manipulator.Init(layer, behaviour);
        }
        

        public void BindButton(ToolButton button)
        {
            var canvas = MainView.Instance;
            
            button.OnFocusEvent += () =>
            {
                canvas.AddManipulator(this.manipulator);
                OnSelect?.Invoke();

            };
            button.OnBlurEvent += () =>
            {
                canvas.RemoveManipulator(this.manipulator);
                OnDeselect?.Invoke();
                LBSMainWindow.MessageManipulator("-");
            };
        }
        
        #endregion;
    }
}