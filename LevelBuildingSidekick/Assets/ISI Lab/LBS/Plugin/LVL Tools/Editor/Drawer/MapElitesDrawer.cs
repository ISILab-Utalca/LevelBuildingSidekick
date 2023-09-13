using ISILab.AI.Optimization.Populations;
using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Drawer(typeof(AssistantMapElite))]
public class MapElitesDrawer : Drawer
{
    public override void Draw(object target, MainView view, Vector2 teselationSize)
    {
        var assitant = target as AssistantMapElite;
        var d = new DottedAreaFeedback();
        var size = assitant.Owner.TileSize * LBSSettings.Instance.general.TileSize;
        var start = assitant.Rect.min * size;
        var end = assitant.Rect.max * size;
        d.SetPosition(Rect.zero);
        d.ActualizePositions(start.ToInt(), end.ToInt());
        d.SetColor(Color.red);
        view.AddElement(d);
    }
}
