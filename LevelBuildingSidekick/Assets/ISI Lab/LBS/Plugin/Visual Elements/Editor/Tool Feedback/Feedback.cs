using LBS.Settings;
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
    private Vector2Int startOffset = new Vector2Int(0, 0);
    private Vector2Int endOffset = new Vector2Int(0, 0);
    private Vector2Int teselationSize = new Vector2Int(100, 100);
    private State currentState; 
    
    protected Color currentColor = Color.white;
    protected Vector2Int startPosition = new Vector2Int();
    protected Vector2Int endPosition = new Vector2Int();

    public bool fixToTeselation = false;
    #endregion

    #region PROPERTIES
    public Vector2Int TeselationSize
    {
        get => teselationSize;
        set => teselationSize = (LBSSettings.Instance.general.TileSize * value).ToInt();
    }

    protected Vector2Int StartOffset
    {
        get => startOffset;
        set => startOffset = value;
    }

    protected Vector2Int EndOffset
    {
        get => endOffset;
        set => endOffset = value;
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
    protected Vector2Int CalcFixTeselation(Vector2Int position)
    {
        var spX = (position.x >= 0) ?
                (position.x / TeselationSize.x) :
                (position.x / TeselationSize.x) - 1;
        var spY = (position.y >= 0) ?
            (position.y / TeselationSize.y) :
            (position.y / TeselationSize.y) - 1;
        return new Vector2Int(spX, spY);
    }

    protected abstract void OnGenerateVisualContent(MeshGenerationContext mgc);

    public abstract void ActualizePositions(Vector2Int p1, Vector2Int p2);
    #endregion

    public enum State
    {
        Correct = 0,
        Problem = 1,
        Error = 2
    }
}