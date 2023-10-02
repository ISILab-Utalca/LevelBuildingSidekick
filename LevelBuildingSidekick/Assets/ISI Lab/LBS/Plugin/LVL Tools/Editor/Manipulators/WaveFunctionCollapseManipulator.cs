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


        /*
        var bundles = LBSAssetsStorage.Instance.Get<Bundle>();
        var fathers = bundles
            .Select(b => b.GetCharacteristics<LBSDirectionedGroup>()[0])    // obtiene todos los bundles que tengan DirsGroup
            .Where(e => e != null)                                      // que no sean nulls
            .Where(e => !e.Owner.IsPresset)                              // ni pressets;
            .ToList();

        var father = fathers[0];

        if(fathers.Count <= 0)
        {
            Debug.Log("[ISI Lab]: There are no structured bundles available for this tool.");
            return;
        }

        var toCalc = new List<ConnectedTile>();
        for (int i = min.x; i <= max.x; i++)
        {
            for (int j = min.y; j <= max.y; j++)
            {
                var current = module.GetTile(new Vector2Int(i, j)) as ConnectedTile;
                if (current == null)
                    continue;

                if (e.ctrlKey)
                {
                    current.SetConnections(new string[4] { "", "", "", "" });
                }
                toCalc.Add(current);
            }
        }

        while (toCalc.Count > 0)
        {
            var currentCalcs = new List<Tuple<ConnectedTile, List<Candidate>>>();
            foreach (var tile in toCalc)
            {
                var candidates = CalcCandidates(tile, father);

                var cTile = new Tuple<ConnectedTile, List<Candidate>>(tile, candidates);
                currentCalcs.Add(cTile);
            }

            var current = currentCalcs.OrderBy(e => e.Item2.Count).First();

            if (currentCalcs.Count <= 0)
            {
                toCalc.Remove(current.Item1);
                continue;
            }

            // collapse
            var selected = current.Item2.RandomRullete(c => c.weight);
            current.Item1.SetConnections(selected.array);

            //var neigs = module.GetTileNeighbors(current.Item1 as T, dirs).Select(t => t as ConnectedTile);
            //SetConnectionNei(current.Item1.Connections,neigs.ToArray());

            toCalc.Remove(current.Item1);
        }
        */
    }


}
