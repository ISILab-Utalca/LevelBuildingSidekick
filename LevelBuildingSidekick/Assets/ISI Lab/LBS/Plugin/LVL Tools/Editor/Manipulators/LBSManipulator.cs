using LBS.Components;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.VisualElements;

namespace ISILab.LBS.Manipulators
{
    [Serializable]
    public abstract class LBSManipulator : MouseManipulator
    {
        #region FIELDS
        protected Feedback feedback;

        private bool started = false;
        private bool ended = false;

        private Vector2Int startClickPosition = Vector2Int.zero;
        private Vector2Int moveClickPosition = Vector2Int.zero;
        private Vector2Int endClickPosition = Vector2Int.zero;
        #endregion

        #region PROPERTIES
        public Vector2Int StartPosition
        {
            get
            {
                if (started)
                {
                    return startClickPosition;
                }
                else
                {
                    Debug.LogWarning("[ISI Lab]: no puedes axeder a la variable 'StartPosition' fuera de la accion.");
                    return default;
                }
            }
        }

        public Vector2Int CurrentPosition
        {
            get
            {
                if (started)
                {
                    return moveClickPosition;
                }
                else
                {
                    Debug.LogWarning("[ISI Lab]: no puedes axeder a la variable 'StartPosition' fuera de la accion.");
                    return default;
                }
            }
        }

        public Vector2Int EndPosition
        {
            get
            {
                if (ended)
                {
                    return endClickPosition;
                }
                else
                {
                    Debug.LogWarning("[ISI Lab]: no puedes axeder a la variable 'StartPosition' fuera de la accion.");
                    return default;
                }
            }
        }
        #endregion

        #region EVENTS
        public Action OnManipulationUnpressed;
        public Action OnManipulationStart;
        public Action OnManipulationUpdate;
        public Action OnManipulationEnd;
        #endregion

        #region METHODS
        /// <summary>
        /// Registers mouse event callbacks on the target element.
        /// </summary>
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnInternalMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnInternalMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnInternalMouseUp);
        }

        /// <summary>
        /// Unregisters mouse event callbacks from the target element.
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnInternalMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnInternalMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnInternalMouseUp);
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

            MainView.Instance.AddElement(feedback);
            feedback.ActualizePositions(startClickPosition, startClickPosition);
        }

        /// <summary>
        /// Updates the feedback during the manipulation.
        /// </summary>
        private void UpdateFeedback()
        {
            if (feedback == null)
                return;

            if (!started)
                return;

            feedback.ActualizePositions(startClickPosition, moveClickPosition);
        }

        /// <summary>
        /// Ends the feedback for the manipulation.
        /// </summary>
        private void EndFeedback()
        {
            if (feedback == null)
                return;

            MainView.Instance.RemoveElement(feedback);
        }

        /// <summary>
        /// Handles the internal mouse down event.
        /// </summary>
        /// <param name="e"></param>
        protected void OnInternalMouseDown(MouseDownEvent e)
        {
            if (e.button != 0)
                return;

            started = true;
            startClickPosition = MainView.Instance.FixPos(e.localMousePosition).ToInt();
            StartFeedback();

            OnManipulationStart?.Invoke();
            OnMouseDown(e.target as VisualElement, startClickPosition, e);
        }

        /// <summary>
        /// Handles the internal mouse move event.
        /// </summary>
        /// <param name="e"></param>
        protected void OnInternalMouseMove(MouseMoveEvent e)
        {
            if (e.button != 0)
                return;

            moveClickPosition = MainView.Instance.FixPos(e.localMousePosition).ToInt();

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
            if (e.button != 0)
                return;

            ended = true;
            endClickPosition = MainView.Instance.FixPos(e.localMousePosition).ToInt();
            EndFeedback();

            if (!e.altKey)
            {
                OnMouseUp(e.target as VisualElement, endClickPosition, e);
                OnManipulationEnd?.Invoke();
            }

            ended = started = false;
        }
        #endregion

        #region VIRTUAL METHODS
        public abstract void Init(LBSLayer layer, object provider);

        protected virtual void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e) { }

        protected virtual void OnMouseMove(VisualElement target, Vector2Int movePosition, MouseMoveEvent e) { }

        protected virtual void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e) { }
        #endregion
    }

    public interface IToolProvider
    {
        public void SetTools(ToolKit toolkit);
    }
}

