using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Feedback : GraphElement
{
    #region INTERNAL_FIELDS
    private readonly Color[] _colors = new Color[] {
        Color.green,
        Color.yellow,
        Color.red 
    };
    #endregion

    #region FIELDS
    private Vector2Int teselationOffset = new Vector2Int(0, 0);
    private Vector2Int teselationSize = new Vector2Int(100, 100);
    private State currentState; 
    
    protected Color currentColor = Color.white;
    protected Vector2Int startPosition;
    protected Vector2Int endPosition;

    protected bool fixToTeselation = false;
    #endregion

    #region PROPERTIES
    protected Vector2Int TeselationSize
    {
        get => teselationSize;
        set => teselationSize = value;
    }

    protected Vector2Int Offset
    {
        get => teselationOffset;
        set => teselationOffset = value;
    }

    protected State CurrentState
    {
        get => currentState;
        set
        {
            currentColor = _colors[(int)value];
            currentState = value;
        }
    }
    #endregion

    #region CONSTRUCTORS
    public Feedback(Vector2Int p1, Vector2Int p2)
    {
        this.ActualizePositions(p1, p2);

        this.SetPosition(new Rect(startPosition, new Vector2(10, 10)));
        this.generateVisualContent += OnGenerateVisualContent;
    }
    #endregion

    #region METHODS
    protected abstract void OnGenerateVisualContent(MeshGenerationContext mgc);

    protected void ActualizePositions(Vector2Int p1, Vector2Int p2)
    {
        startPosition = new Vector2Int(Mathf.Min(p1.x, p2.x), Mathf.Min(p1.y, p2.y));
        endPosition = new Vector2Int(Mathf.Max(p1.x, p2.x), Mathf.Max(p1.y, p2.y));

        if (fixToTeselation)
        {
            startPosition = new Vector2Int(
                startPosition.x / teselationSize.x,
                startPosition.y / teselationSize.y) 
                * teselationSize;
            endPosition = new Vector2Int(
                endPosition.x / teselationSize.x,
                endPosition.y / teselationSize.y)
                * teselationSize;
        }

    }
    #endregion

    public enum State
    {
        Correct = 0,
        Problem = 1,
        Error = 2
    }
}

public class DotedAreaFeedback : Feedback
{
    public DotedAreaFeedback(Vector2Int p1, Vector2Int p2) : base(p1, p2)
    {

    }

    protected override void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var painter = mgc.painter2D;
        var view = this.GetFirstAncestorOfType<MainView>();

        painter.DrawBox(startPosition, endPosition, currentColor, 4);
    }
}