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
        var size = (assitant.Owner.TileSize * LBSSettings.Instance.general.TileSize).ToInt();
        var start = new Vector2(assitant.Rect.min.x, -assitant.Rect.min.y + 1) * size;
        var end = new Vector2(assitant.Rect.max.x, -assitant.Rect.max.y + 1) * size;
        d.SetPosition(Rect.zero);
        d.ActualizePositions(start.ToInt(), end.ToInt());
        Debug.Log(start.ToInt() + " - " + end.ToInt());
        d.SetColor(Color.red);
        view.AddElement(d);
    }
}
