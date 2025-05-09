using ISILab.LBS.Settings;
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
            var assistant = target as AssistantMapElite;
            var dotArea = new DottedAreaFeedback();
            var size = (assistant.OwnerLayer.TileSize * LBSSettings.Instance.general.TileSize).ToInt();
            var start = new Vector2(assistant.RawToolRect.min.x, -assistant.RawToolRect.min.y + 1) * size;
            var end = new Vector2(assistant.RawToolRect.max.x, -assistant.RawToolRect.max.y + 1) * size;
            dotArea.SetPosition(Rect.zero);
            dotArea.ActualizePositions(start.ToInt(), end.ToInt());
            dotArea.SetColor(LBSSettings.Instance.view.errorColor);
            view.AddElement(assistant.OwnerLayer,this,dotArea);
        }
    }
}