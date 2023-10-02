using LBS.Bundles;
using LBS.Components;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;


public class WaveFunctionCollapseManipulator : ManipulateTeselation
{
    private struct Candidate
    {
        public string[] array;
        public float weight;
    }

    private Vector2Int first;

    private AssistantWFC assistant;

    public WaveFunctionCollapseManipulator() : base()
    {
        feedback.fixToTeselation = true;
    }

    public override void Init(LBSLayer layer, object provider)
    {
        base.Init(layer, provider);

        assistant = provider as AssistantWFC;
        feedback.TeselationSize = layer.TileSize;
        layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {
        first = assistant.Owner.ToFixedPosition(position);
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int position, MouseMoveEvent e)
    {

    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        var corners = assistant.Owner.ToFixedPosition(StartPosition, EndPosition);

        var positions = new List<Vector2Int>();
        for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
        {
            for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
            {
                positions.Add(new Vector2Int(i, j));
            }
        }

        assistant.Positions = positions;

        assistant.OverrideValues = e.ctrlKey;

        assistant.Execute();
    }
}
