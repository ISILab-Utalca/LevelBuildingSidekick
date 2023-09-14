using LBS.Components;
using LBS.Components.Teselation;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.PackageManager.UI;
using LBS.Tools.Transformer;
using LBS.Behaviours;
using LBS;

public class SetZoneConnection : LBSManipulator
{
    private LBSLayer layer;
    private HillClimbingAssistant assistant;
    private Vector2Int first;

    public SetZoneConnection() : base()
    {
        feedback = new ConnectedLine();
        feedback.fixToTeselation = false;
    }

    public override void Init(LBSLayer layer, object provider)
    {
        this.layer = layer;
        this.assistant = provider as HillClimbingAssistant;

        feedback.TeselationSize = layer.TileSize;
        layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
    }

    protected override void OnMouseDown(VisualElement target, Vector2Int position, MouseDownEvent e)
    {
        first = layer.ToFixedPosition(position);
    }

    protected override void OnMouseMove(VisualElement target, Vector2Int position, MouseMoveEvent e)
    {

    }

    protected override void OnMouseUp(VisualElement target, Vector2Int position, MouseUpEvent e)
    {
        
        var z1 = assistant.GetZone(first);

        if (z1 == null)
        {
            Debug.Log("No seleccionaste ninguna zona de comienzo");
            return;
        }

        var pos = layer.ToFixedPosition(position);
        var z2 = assistant.GetZone(pos);
        if (z2 == null)
        {
            Debug.Log("No seleccionaste ninguna zona de final");
            return;
        }

        if (z1.Equals(z2))
        {
            Debug.Log("No puedes conectar una zona con sigo misma");
            return;
        }

        assistant.ConnectZones(z1, z2);
        
    }
}
