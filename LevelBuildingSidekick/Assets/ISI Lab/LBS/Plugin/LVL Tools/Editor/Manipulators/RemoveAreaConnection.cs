using ISILab.AI.Optimization;
using LBS;
using LBS.Behaviours;
using LBS.Components;
using LBS.Components.TileMap;
using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RemoveAreaConnection : LBSManipulator
{
    HillClimbingAssistant hillclimbing;

    public RemoveAreaConnection() : base()
    {
        //feedback = new ConnectedLine();
        //feedback.fixToTeselation = true;
    }

    public override void Init(LBSLayer layer, object owner)
    {
        hillclimbing = owner as HillClimbingAssistant;
        //feedback.TeselationSize = layer.TileSize;
        layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int position, MouseMoveEvent e)
    {

    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        Vector2 pos = position / (hillclimbing.Owner.TileSize * LBSSettings.Instance.general.TileSize);
        pos = new Vector2(pos.x, -(pos.y - 1));

        hillclimbing.RemoveZoneConnection(pos, 0.2f);
    }


}
