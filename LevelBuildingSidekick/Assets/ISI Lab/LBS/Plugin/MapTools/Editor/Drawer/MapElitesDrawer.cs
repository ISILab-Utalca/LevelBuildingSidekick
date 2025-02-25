using ISILab.LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ISILab.Extensions;
using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.Assistants;
using ISILab.LBS.VisualElements;

namespace ISILab.LBS.Drawers
{
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
    }
}