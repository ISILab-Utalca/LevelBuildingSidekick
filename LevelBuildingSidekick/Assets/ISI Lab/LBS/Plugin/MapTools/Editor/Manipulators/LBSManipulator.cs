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

                Debug.LogWarning("[ISI Lab]: no puedes acceder a la variable 'StartPosition' fuera de la accion.");
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

                Debug.LogWarning("[ISI Lab]: no puedes axeder a la variable 'StartPosition' fuera de la accion.");
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
        /// <param name="e"></param>
        protected void OnInternalMouseDown(MouseDownEvent e)
        {
            if (e.button != 0 && e.button != 1)
                return;
            
            OnManipulationNotification?.Invoke();
            startClickPosition = MainView.Instance.FixPos(e.localMousePosition).ToInt();

            // right click tries deleting 
            if (e.button == 1 && remover != null)
            {
                remover.isRightClick = true;
                
                LBSMainWindow.WarningManipulator("Remover Activated."); // notify remover use
                ToolKit.Instance.SetActive(remover.GetType());
                OnManipulationRightClick?.Invoke();
                
                var ne = MouseDownEvent.GetPooled(e.localMousePosition, 0, e.clickCount, e.mouseDelta, e.modifiers);
                ne.target = e.target as VisualElement;

                remover.OnInternalMouseDown(ne);
                e.StopImmediatePropagation();
                return;
            }
            
            started = true;
             
            StartFeedback();

            OnManipulationStart?.Invoke();
            OnMouseDown(e.target as VisualElement, startClickPosition, e);
            
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
        /// <param name="e"></param>
        protected void OnInternalMouseMove(MouseMoveEvent e)
        {
            moveClickPosition = MainView.Instance.FixPos(e.localMousePosition).ToInt();

            // Display grid position
            if (lbsLayer != null)
            {
                Vector2 pos = lbsLayer.ToFixedPosition(moveClickPosition);
                LBSMainWindow.GridPosition(pos);
            }
   
            // button functionalities
            if (e.button != 0 && e.button != 1)
                return;
            
            // right click tries deleting 
            if (e.button == 1 && remover != null)
            {
                var ne = MouseMoveEvent.GetPooled(e.localMousePosition, 0, e.clickCount, e.mouseDelta, e.modifiers);
                ne.target = e.target as VisualElement;
                remover.isRightClick = true;
                e.StopImmediatePropagation();
                MainView.Instance.RemoveManipulator(adder);
                remover.OnInternalMouseMove(ne);
                return;
            }

            OnMouseMove(e.target as VisualElement, moveClickPosition, e);
            UpdateFeedback();

            OnManipulationUpdate?.Invoke();
        }

        /// <summary>
        /// Handles the internal mouse up event.
        /// </summary>
        /// <param name="e"></param>
        protected void OnInternalMouseUp(MouseUpEvent e)
        {
            if (e.button != 0 && e.button != 1)
                return;
            ended = true;
            endClickPosition = MainView.Instance.FixPos(e.localMousePosition).ToInt();
            EndFeedback();

            // right click tries deleting 
            if (e.button == 1 && remover != null)
            {
                remover.isRightClick = true;
                OnManipulationRightClick?.Invoke();
                var ne = MouseUpEvent.GetPooled(e.localMousePosition, 0, e.clickCount, e.mouseDelta, e.modifiers);
                ne.target = e.target as VisualElement;
                e.StopImmediatePropagation();
                remover.OnInternalMouseUp(ne);
                return;
            }
            
            if (e.button == 1 && remover != null)
            {
                OnManipulationRightClick?.Invoke();
                remover.OnManipulationNotification?.Invoke();
                
                return;
            }
            
            if (!e.altKey)
            {
                e.StopPropagation();
                OnMouseUp(e.target as VisualElement, endClickPosition, e);
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
        
        protected virtual void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e) { }
        
        protected virtual void OnMouseLeave(VisualElement target, MouseLeaveEvent e) { }

        protected virtual void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e) { }

        protected virtual void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e) { }
        
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

