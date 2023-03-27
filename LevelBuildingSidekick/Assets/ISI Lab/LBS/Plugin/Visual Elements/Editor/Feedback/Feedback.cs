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
    protected Vector2Int startPosition = new Vector2Int();
    protected Vector2Int endPosition = new Vector2Int();

    public bool fixToTeselation = false;
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

    public Feedback()
    {
        this.ActualizePositions(startPosition, endPosition);
        this.SetPosition(new Rect(startPosition, new Vector2(10, 10)));
        this.generateVisualContent += OnGenerateVisualContent;
    }

    #endregion

    #region METHODS
    protected abstract void OnGenerateVisualContent(MeshGenerationContext mgc);

    public void ActualizePositions(Vector2Int p1, Vector2Int p2)
    {
        startPosition = new Vector2Int(Mathf.Min(p1.x, p2.x), Mathf.Min(p1.y, p2.y));
        endPosition = new Vector2Int(Mathf.Max(p1.x, p2.x), Mathf.Max(p1.y, p2.y));
        
        if (fixToTeselation)
        {
            var spX = (startPosition.x >= 0) ?
                (startPosition.x / teselationSize.x) :
                (startPosition.x / teselationSize.x) - 1;
            var spY = (startPosition.y >= 0) ?
                (startPosition.y / teselationSize.y) :
                (startPosition.y / teselationSize.y) - 1;
            startPosition = new Vector2Int(spX, spY);

            var epX = (endPosition.x >= 0) ?
                (endPosition.x / teselationSize.x) :
                (endPosition.x / teselationSize.x) - 1;
            var epY = (endPosition.y >= 0) ?
                (endPosition.y / teselationSize.y) :
                (endPosition.y / teselationSize.y) - 1;
            endPosition = new Vector2Int(epX, epY);

            endPosition.x += 1;
            endPosition.y += 1;

            startPosition *= teselationSize;
            endPosition *= teselationSize;
        }


        this.MarkDirtyRepaint();
    }
    #endregion

    public enum State
    {
        Correct = 0,
        Problem = 1,
        Error = 2
    }
}

public class AreaFeedback : Feedback
{
    public AreaFeedback(Vector2Int p1, Vector2Int p2) : base(p1, p2) { }

    public AreaFeedback() : base() { }

    protected override void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var painter = mgc.painter2D;
        var fillColor = currentColor * new Color(1, 1, 1, 0.1f);

        var points = new List<Vector2>()
        {
            new Vector2(startPosition.x, startPosition.y),
            new Vector2(startPosition.x, endPosition.y),
            new Vector2(endPosition.x, endPosition.y),
            new Vector2(endPosition.x, startPosition.y),
        };
        painter.DrawPoligon(points, fillColor, currentColor, 4, true);
    }
}

public class ConectedLine : Feedback
{
    public ConectedLine(Vector2Int p1, Vector2Int p2) : base(p1, p2) { }
    public ConectedLine() : base() { }

    protected override void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var painter = mgc.painter2D;
        var line = new List<Vector2>() { startPosition, endPosition };
        painter.DrawPoligon(line, new Color(0,0,0,0), currentColor, 4, false);
        painter.DrawCircle(startPosition, 16, currentColor);
        painter.DrawCircle(endPosition, 16, currentColor);
    }
}