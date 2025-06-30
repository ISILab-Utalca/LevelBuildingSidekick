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
        protected LBSLayer LBSLayer;

        /// <summary>
        /// The initializer type which can be a module, behaviour or assistant
        /// </summary>
        private Type _objectType;
        
        protected Feedback Feedback;
        
        private VectorImage _icon;

        #endregion
        
        #region STATES
        private bool _started;
        private bool _ended;
        private bool _isRightClick;
        private bool _hasProcessedMouseUp;
        #endregion
        
        #region POSITIONS
        private Vector2Int _startClickPosition = Vector2Int.zero;
        private Vector2Int _moveClickPosition = Vector2Int.zero;
        private Vector2Int _endClickPosition = Vector2Int.zero;
        #endregion
        
        #region MANIPULATOR ADDER AND REMOVER
        /// <summary>
        /// referenced by deleters. If activatedByOther, sets adder as manipulator in MainView
        /// </summary>
        public LBSManipulator Adder { get; private set; }

        /// <summary>
        /// referenced by adders. usable by right click
        /// </summary>
        public LBSManipulator Remover { get; private set; }
        #endregion
        
        #endregion

        #region PROPERTIES

        public Type ObjectType
        {
            get => _objectType;
            set => _objectType = value;
        }
        public LBSLayer Layer => LBSLayer;
        public string Description { get; set; }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        protected abstract string IconGuid { get; }

        public VectorImage Icon => _icon;

     

        public Vector2Int StartPosition
        {
            get
            {
                if (_started)
                {
                    return _startClickPosition;
                }

                Debug.LogWarning("[ISI Lab]: cannot access the variable 'StartPosition' outside the action.");
                return default;
            }
            set => _startClickPosition = value;
        }

        public Vector2Int EndPosition
        {
            get
            {
                if (_ended)
                {
                    return _endClickPosition;
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
            Remover = remover;
            remover.Adder = this;
        }
        #endregion

        #region EVENTS
        /* meant to call the default description message of a manipulator in case it is overwritten for unique cases */
        public Action OnManipulationNotification;
        public Action OnManipulationStart;
        public Action OnManipulationUpdate;
        public Action OnManipulationEnd;
        public Action OnManipulationRightClick;
        public Action OnManipulationRightClickEnd;
        public Action OnManipulationLeftClickCtrl;
        private string _name;

        #endregion
        
        #region CONSTRUCTORS

        protected LBSManipulator()
        {
            if (IconGuid is "") return;
            _icon = LBSAssetMacro.LoadAssetByGuid<VectorImage>(IconGuid);
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
            bool deleting = Remover is { _isRightClick: true } || Adder is not null;
            Feedback.delete = deleting;
            return deleting;
        }
        
        public void SetFeedback(Feedback feedback)
        {
            if (!_started)
                return;

            if (feedback == this.Feedback)
                return;

            MainView.Instance.RemoveElement(this.Feedback);
            this.Feedback = feedback;
            MainView.Instance.AddElement(feedback);
        }

        /// <summary>
        /// Starts the feedback for the manipulation.
        /// </summary>
        private void StartFeedback()
        {
            if (Feedback == null)
                return;
            
            UpdateFeedbackColor();
            MainView.Instance.AddElement(Feedback);
            Feedback.ActualizePositions(_startClickPosition, _startClickPosition);
        }

        /// <summary>
        /// Updates the feedback during the manipulation.
        /// </summary>
        private void UpdateFeedback()
        {
            if (Feedback == null) return;
            if (!_started) return;

            UpdateFeedbackColor();
            Feedback.ActualizePositions(_startClickPosition, _moveClickPosition);
        }
        
        /// <summary>
        /// Ends the feedback for the manipulation.
        /// </summary>
        private void EndFeedback()
        {
            if (Feedback == null)
                return;

            UpdateFeedbackColor();
            MainView.Instance.RemoveElement(Feedback);
        }

        /// <summary>
        /// Handles the internal mouse down event.
        /// </summary>
        /// <param name="event"></param>
        protected void OnInternalMouseDown(MouseDownEvent @event)
        {
            if (@event.button != 0 && @event.button != 1)
                return;
            
            _hasProcessedMouseUp = false;
            OnManipulationNotification?.Invoke();
            _startClickPosition = MainView.Instance.FixPos(@event.localMousePosition).ToInt();

            // right click tries deleting 
            if (@event.button == 1 && Remover != null)
            {
                Remover._isRightClick = true;
                
                LBSMainWindow.WarningManipulator("Remover Activated."); // notify remover use
                ToolKit.Instance.SetActive(Remover.GetType());
                OnManipulationRightClick?.Invoke();
                
                var ne = MouseDownEvent.GetPooled(@event.localMousePosition, 0, @event.clickCount, @event.mouseDelta, @event.modifiers);
                ne.target = @event.target as VisualElement;

                Remover.OnInternalMouseDown(ne);
                @event.StopImmediatePropagation();
                return;
            }
            
            _started = true;
             
            StartFeedback();

            OnManipulationStart?.Invoke();
            OnMouseDown(@event.target as VisualElement, _startClickPosition, @event);
        }

        protected void OnInternalMouseLeave(MouseLeaveEvent e)
        {
            OnMouseLeave(e.target as VisualElement, e);
        }

        /// <summary>
        /// Handles the internal mouse move event.
        /// </summary>
        /// <param name="event"></param>
        protected void OnInternalMouseMove(MouseMoveEvent @event)
        {
            _moveClickPosition = MainView.Instance.FixPos(@event.localMousePosition).ToInt();

            // Display grid position
            if (LBSLayer != null)
            {
                Vector2 pos = LBSLayer.ToFixedPosition(_moveClickPosition);
                LBSMainWindow.GridPosition(pos);
            }
   
            // button functionalities
            if (@event.button != 0 && @event.button != 1)
                return;
            
            // right click tries deleting 
            if (@event.button == 1 && Remover != null)
            {
                var ne = MouseMoveEvent.GetPooled(@event.localMousePosition, 0, @event.clickCount, @event.mouseDelta, @event.modifiers);
                ne.target = @event.target as VisualElement;
                Remover._isRightClick = true;
                @event.StopImmediatePropagation();
                MainView.Instance.RemoveManipulator(Adder);
                Remover.OnInternalMouseMove(ne);
                return;
            }

            OnMouseMove(@event.target as VisualElement, _moveClickPosition, @event);
            UpdateFeedback();

            OnManipulationUpdate?.Invoke();
        }

        /// <summary>
        /// Handles the internal mouse up event.
        /// </summary>
        /// <param name="event"></param>
        protected void OnInternalMouseUp(MouseUpEvent @event)
        {
            Debug.Log($"OnInternalMouseUp called by: {GetType().Name}");

            
            if (@event.button != 0 && @event.button != 1 || _hasProcessedMouseUp)
            {
                @event.StopImmediatePropagation();
                return;
            }


            _hasProcessedMouseUp = true;
            
            _ended = true;
            _endClickPosition = MainView.Instance.FixPos(@event.localMousePosition).ToInt();
            EndFeedback();

            // right click tries deleting 
            if (@event.button == 1 && Remover != null)
            {
                Remover._isRightClick = true;
                OnManipulationRightClick?.Invoke();
                var ne = MouseUpEvent.GetPooled(@event.localMousePosition, 0, @event.clickCount, @event.mouseDelta, @event.modifiers);
                ne.target = @event.target as VisualElement;
                @event.StopImmediatePropagation();
                Remover.OnInternalMouseUp(ne);
                return;
            }
            
            if (@event.button == 1 && Remover != null)
            {
                OnManipulationRightClick?.Invoke();
                Remover.OnManipulationNotification?.Invoke();
                @event.StopImmediatePropagation();
                return;
            }
            
            if (!@event.altKey)
            {
                OnMouseUp(@event.target as VisualElement, _endClickPosition, @event);
                @event.StopImmediatePropagation();
            }

            _ended = _started = false;

            // if it's a deleter called from an adder
            if (_isRightClick)
            {
                _isRightClick = false;
                LBSMainWindow.WarningManipulator(); // finished using a remover
            }
            
            OnManipulationEnd?.Invoke();
            @event.StopImmediatePropagation();
        }
        
        #endregion

        #region VIRTUAL METHODS
        public virtual void Init(LBSLayer layer, object provider = null)
        {
            LBSLayer = layer;
            _objectType = provider == null ? typeof(Manipulator) : provider.GetType().BaseType;
        }
        
        protected virtual void OnMouseDown(VisualElement element, Vector2Int startPosition, MouseDownEvent e) { }
        
        protected virtual void OnMouseLeave(VisualElement element, MouseLeaveEvent e) { }

        protected virtual void OnMouseMove(VisualElement element, Vector2Int movePosition, MouseMoveEvent e) { }

        protected virtual void OnMouseUp(VisualElement element, Vector2Int endPosition, MouseUpEvent e) { }
        
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

