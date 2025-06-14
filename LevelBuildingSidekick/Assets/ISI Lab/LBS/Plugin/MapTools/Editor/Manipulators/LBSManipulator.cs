using LBS.Components;
using LBS.VisualElements;
using System;

using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.VisualElements;
using ISILab.Macros;

namespace ISILab.LBS.Manipulators
{
    [Serializable]
    public abstract class LBSManipulator : MouseManipulator
    {
        #region FIELDS

        #region DATA
        protected LBSLayer lbsLayer;

        /// <summary>
        /// The initializer type which can be a module, behaviour or assistant
        /// </summary>
        protected Type objectType;
        
        protected Feedback feedback;
        
        protected VectorImage icon;

        protected string name;
        
        protected string description;
        #endregion
        
        #region STATES
        private bool started;
        private bool ended;
        private bool isRightClick;
        #endregion
        
        #region POSITIONS
        private Vector2Int startClickPosition = Vector2Int.zero;
        private Vector2Int moveClickPosition = Vector2Int.zero;
        private Vector2Int endClickPosition = Vector2Int.zero;
        #endregion
        
        #region MANIPULATOR ADDER AND REMOVER
        /// <summary>
        /// referenced by adders. usable by right click
        /// </summary>
        private LBSManipulator remover;
        
        /// <summary>
        /// referenced by deleters. If activatedByOther, sets adder as manipulator in MainView
        /// </summary>
        private LBSManipulator adder;
        #endregion
        
        #endregion

        #region PROPERTIES

        public Type ObjectType
        {
            get => objectType;
            set => objectType = value;
        }
        public LBSLayer Layer => lbsLayer;
        public string Description => description;
        public string Name => name;
        protected abstract string IconGuid { get; }

        public VectorImage Icon => icon;
        public LBSManipulator Adder => adder;
        public LBSManipulator Remover => remover;

        public Vector2Int StartPosition
        {
            get
            {
                if (started)
                {
                    return startClickPosition;
                }

                Debug.LogWarning("[ISI Lab]: cannot access the variable 'StartPosition' outside the action.");
                return default;
            }
            set => startClickPosition = value;
        }

        public Vector2Int EndPosition
        {
            get
            {
                if (ended)
                {
                    return endClickPosition;
                }

                Debug.LogWarning("[ISI Lab]: cannot assign the variable 'StartPosition' outside the action.");
                return default;
            }
        }

        /// <summary>
        /// Sets the manipulator that removes for the current manipulator's adding function.
        /// Only assign this from an Adder manipulator.
        /// </summary>
        /// <param name="remover"></param>
        public void SetRemover(LBSManipulator remover)
        {
            this.remover = remover;
            remover.adder = this;
        }
        #endregion

        #region EVENTS
        /* meant to call the default description message of a manipulator in case it is overwritten for unique cases */
        public Action OnManipulationNotification;
        public Action OnManipulationUnpressed;
        public Action OnManipulationStart;
        public Action OnManipulationUpdate;
        public Action OnManipulationEnd;
        public Action OnManipulationRightClick;
        public Action OnManipulationRightClickEnd;
        public Action OnManipulationLeftClickCTRL;
        #endregion
        
        #region CONSTRUCTORS
        protected LBSManipulator()
        {
            icon = LBSAssetMacro.LoadAssetByGuid<VectorImage>(IconGuid);
        }    
        #endregion

        #region METHODS
        
        
        /// <summary>
        /// Registers mouse event callbacks on the target element.
        /// </summary>
        protected override void RegisterCallbacksOnTarget()
        {
            target.AddManipulator(new ContextualMenuManipulator(evt => { evt.menu.ClearItems(); }));
            target.RegisterCallback<MouseDownEvent>(OnInternalMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnInternalMouseMove);
            target.RegisterCallback<MouseLeaveEvent>(OnInternalMouseLeave);
            target.RegisterCallback<MouseUpEvent>(OnInternalMouseUp);
            target.RegisterCallback<KeyDownEvent>(OnKeyDown);
            target.RegisterCallback<KeyUpEvent>(OnKeyUp);
            target.RegisterCallback<WheelEvent>(OnWheelEvent);
        }

        /// <summary>
        /// Unregisters mouse event callbacks from the target element.
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnInternalMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnInternalMouseMove);
            target.UnregisterCallback<MouseLeaveEvent>(OnInternalMouseLeave);
            target.UnregisterCallback<MouseUpEvent>(OnInternalMouseUp);
            target.UnregisterCallback<KeyDownEvent>(OnKeyDown);
            target.UnregisterCallback<WheelEvent>(OnWheelEvent);
        }

        
        // if it has an adder ref it means the manipulator's function is to delete
        private bool UpdateFeedbackColor()
        {
            bool deleting = remover is { isRightClick: true } || adder is not null;
            feedback.delete = deleting;
            return deleting;
        }
        
        public void SetFeedback(Feedback feedback)
        {
            if (!started)
                return;

            if (feedback == this.feedback)
                return;

            MainView.Instance.RemoveElement(this.feedback);
            this.feedback = feedback;
            MainView.Instance.AddElement(feedback);
        }

        /// <summary>
        /// Starts the feedback for the manipulation.
        /// </summary>
        private void StartFeedback()
        {
            if (feedback == null)
                return;
            
            UpdateFeedbackColor();
            MainView.Instance.AddElement(feedback);
            feedback.ActualizePositions(startClickPosition, startClickPosition);
        }

        /// <summary>
        /// Updates the feedback during the manipulation.
        /// </summary>
        private void UpdateFeedback()
        {
            if (feedback == null) return;
            if (!started) return;

            UpdateFeedbackColor();
            feedback.ActualizePositions(startClickPosition, moveClickPosition);
        }
        
        /// <summary>
        /// Ends the feedback for the manipulation.
        /// </summary>
        private void EndFeedback()
        {
            if (feedback == null)
                return;

            UpdateFeedbackColor();
            MainView.Instance.RemoveElement(feedback);
        }

        /// <summary>
        /// Handles the internal mouse down event.
        /// </summary>
        /// <param name="_event"></param>
        protected void OnInternalMouseDown(MouseDownEvent _event)
        {
            if (_event.button != 0 && _event.button != 1)
                return;
            
            OnManipulationNotification?.Invoke();
            startClickPosition = MainView.Instance.FixPos(_event.localMousePosition).ToInt();

            // right click tries deleting 
            if (_event.button == 1 && remover != null)
            {
                remover.isRightClick = true;
                
                LBSMainWindow.WarningManipulator("Remover Activated."); // notify remover use
                ToolKit.Instance.SetActive(remover.GetType());
                OnManipulationRightClick?.Invoke();
                
                var ne = MouseDownEvent.GetPooled(_event.localMousePosition, 0, _event.clickCount, _event.mouseDelta, _event.modifiers);
                ne.target = _event.target as VisualElement;

                remover.OnInternalMouseDown(ne);
                _event.StopImmediatePropagation();
                return;
            }
            
            started = true;
             
            StartFeedback();

            OnManipulationStart?.Invoke();
            OnMouseDown(_event.target as VisualElement, startClickPosition, _event);
            
            // check if last called by adder
            if (isRightClick && adder != null)
            {
 
            }
        }

        protected void OnInternalMouseLeave(MouseLeaveEvent e)
        {
            OnMouseLeave(e.target as VisualElement, e);
        }

        /// <summary>
        /// Handles the internal mouse move event.
        /// </summary>
        /// <param name="_event"></param>
        protected void OnInternalMouseMove(MouseMoveEvent _event)
        {
            moveClickPosition = MainView.Instance.FixPos(_event.localMousePosition).ToInt();

            // Display grid position
            if (lbsLayer != null)
            {
                Vector2 pos = lbsLayer.ToFixedPosition(moveClickPosition);
                LBSMainWindow.GridPosition(pos);
            }
   
            // button functionalities
            if (_event.button != 0 && _event.button != 1)
                return;
            
            // right click tries deleting 
            if (_event.button == 1 && remover != null)
            {
                var ne = MouseMoveEvent.GetPooled(_event.localMousePosition, 0, _event.clickCount, _event.mouseDelta, _event.modifiers);
                ne.target = _event.target as VisualElement;
                remover.isRightClick = true;
                _event.StopImmediatePropagation();
                MainView.Instance.RemoveManipulator(adder);
                remover.OnInternalMouseMove(ne);
                return;
            }

            OnMouseMove(_event.target as VisualElement, moveClickPosition, _event);
            UpdateFeedback();

            OnManipulationUpdate?.Invoke();
        }

        /// <summary>
        /// Handles the internal mouse up event.
        /// </summary>
        /// <param name="_event"></param>
        protected void OnInternalMouseUp(MouseUpEvent _event)
        {
            if (_event.button != 0 && _event.button != 1)
                return;
            ended = true;
            endClickPosition = MainView.Instance.FixPos(_event.localMousePosition).ToInt();
            EndFeedback();

            // right click tries deleting 
            if (_event.button == 1 && remover != null)
            {
                remover.isRightClick = true;
                OnManipulationRightClick?.Invoke();
                var ne = MouseUpEvent.GetPooled(_event.localMousePosition, 0, _event.clickCount, _event.mouseDelta, _event.modifiers);
                ne.target = _event.target as VisualElement;
                _event.StopImmediatePropagation();
                remover.OnInternalMouseUp(ne);
                return;
            }
            
            if (_event.button == 1 && remover != null)
            {
                OnManipulationRightClick?.Invoke();
                remover.OnManipulationNotification?.Invoke();
                
                return;
            }
            
            if (!_event.altKey)
            {
                _event.StopPropagation();
                OnMouseUp(_event.target as VisualElement, endClickPosition, _event);
                OnManipulationEnd?.Invoke();
            }

            ended = started = false;

            // if it's a deleter called from an adder
            if (isRightClick)
            {
                isRightClick = false;
                OnManipulationRightClickEnd?.Invoke();
                LBSMainWindow.WarningManipulator(); // finished using a remover
            }
        }
        
        #endregion

        #region VIRTUAL METHODS
        public virtual void Init(LBSLayer layer, object provider = null)
        {
            lbsLayer = layer;
            objectType = provider == null ? typeof(Manipulator) : provider.GetType().BaseType;
        }
        
        protected virtual void OnMouseDown(VisualElement _target, Vector2Int startPosition, MouseDownEvent e) { }
        
        protected virtual void OnMouseLeave(VisualElement _target, MouseLeaveEvent e) { }

        protected virtual void OnMouseMove(VisualElement _target, Vector2Int movePosition, MouseMoveEvent e) { }

        protected virtual void OnMouseUp(VisualElement _target, Vector2Int endPosition, MouseUpEvent e) { }
        
        protected virtual void OnKeyDown(KeyDownEvent e) { }
        
        protected virtual void OnKeyUp(KeyUpEvent e) { }
        
        protected virtual void OnWheelEvent(WheelEvent e) { }
        #endregion
    }

    public interface IToolProvider
    {
        public void SetTools(ToolKit toolkit);
    }
}

