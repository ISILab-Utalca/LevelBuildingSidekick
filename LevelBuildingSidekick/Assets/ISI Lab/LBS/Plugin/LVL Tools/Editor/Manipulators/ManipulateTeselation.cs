using LBS.Components;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ManipulateTeselation<T> : LBSManipulator where T : LBSTile
{
    protected Feedback feedback = new AreaFeedback();

    protected TileMapModule<T> module;
    private MainView mainView;

    private bool started = false;
    private bool ended = false;

    private Vector2Int startClickPosition = Vector2Int.zero;
    private Vector2Int moveClickPosition = Vector2Int.zero;
    private Vector2Int endClickPosition = Vector2Int.zero;

    protected MainView MainView => mainView;

    public Vector2Int StartPosition // estos nombres podrian ser mas descriptivos por que "movePos" es como poco claro (!) 
    {
        get
        {
            if(started)
            {
                return startClickPosition;
            }
            else
            {
                Debug.LogWarning("[ISI Lab]: no puedes axeder a la variable 'StartPosition' fuera de la accion.");
                return default(Vector2Int);
            }
        }
    }

    public Vector2Int MovePosition // estos nombres podrian ser mas descriptivos por que "movePos" es como poco claro (!) 
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
                return default(Vector2Int);
            }
        }
    }

    public Vector2Int EndPosition // estos nombres podrian ser mas descriptivos por que "movePos" es como poco claro (!) 
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
                return default(Vector2Int);
            }
        }
    }

    public ManipulateTeselation() : base() 
    {
        feedback.fixToTeselation = true;
    }

    public override void Init(ref MainView view, ref LBSLevelData level, ref LBSLayer layer, ref LBSModule module)
    {
        this.module = layer.GetModule<TileMapModule<T>>();
        this.mainView = view;
    }

    private void StartFeedback()
    {
        if (feedback == null)
            return;

        MainView.AddElement(feedback);
        feedback.ActualizePositions(startClickPosition, startClickPosition);
    }

    private void UpdateFeedback()
    {
        if (feedback == null)
            return;

        if (!started)
            return;

        feedback.ActualizePositions(startClickPosition, moveClickPosition);
    }

    private void EndFeedback()
    {
        if (feedback == null)
            return;

        MainView.RemoveElement(feedback);
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnInternalMouseDown);
        target.RegisterCallback<MouseMoveEvent>(OnInternalMouseMove);
        target.RegisterCallback<MouseUpEvent>(OnInternalMouseUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnInternalMouseDown);
        target.UnregisterCallback<MouseMoveEvent>(OnInternalMouseMove);
        target.UnregisterCallback<MouseUpEvent>(OnInternalMouseUp);
    }

    protected void OnInternalMouseDown(MouseDownEvent e)
    {
        if (e.button != 0)
            return;

        this.started = true;
        this.startClickPosition = MainView.FixPos(e.localMousePosition).ToInt();
        StartFeedback();

        OnManipulationStart?.Invoke();
        OnMouseDown(e.target as VisualElement, startClickPosition, e);
    }

    protected void OnInternalMouseMove(MouseMoveEvent e)
    {
        if (e.button != 0)
            return;

        this.moveClickPosition = MainView.FixPos(e.localMousePosition).ToInt();
        UpdateFeedback();

        OnMouseMove(e.target as VisualElement, moveClickPosition, e);
        OnManipulationUpdate?.Invoke();
    }

    protected void OnInternalMouseUp(MouseUpEvent e)
    {
        if (e.button != 0)
            return;

        this.ended = true;
        this.endClickPosition = MainView.FixPos(e.localMousePosition).ToInt();
        EndFeedback();

        OnMouseUp(e.target as VisualElement, endClickPosition, e);
        OnManipulationEnd?.Invoke();

        this.ended = this.started = false;
    }

    protected abstract void OnMouseDown(VisualElement target, Vector2Int startPosition, MouseDownEvent e);

    protected abstract void OnMouseMove(VisualElement target, Vector2Int MovePosition, MouseMoveEvent e);

    protected abstract void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e);
}
