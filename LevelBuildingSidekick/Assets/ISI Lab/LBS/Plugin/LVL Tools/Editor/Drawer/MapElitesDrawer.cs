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
        var start = new Vector2(assitant.RawToolRect.min.x, -assitant.RawToolRect.min.y + 1) * size;
        var end = new Vector2(assitant.RawToolRect.max.x, -assitant.RawToolRect.max.y + 1) * size;
        d.SetPosition(Rect.zero);
        d.ActualizePositions(start.ToInt(), end.ToInt());
        d.SetColor(Color.red);
        view.AddElement(d);
    }

    public override Texture2D GetTexture(object target, Rect sourceRect, Vector2Int teselationSize)
    {
        throw new System.NotImplementedException();
    }
}
