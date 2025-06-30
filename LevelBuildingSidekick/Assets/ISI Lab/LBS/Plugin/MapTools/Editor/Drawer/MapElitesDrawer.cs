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
        private DottedAreaFeedback dotArea;
        private bool _loaded = false;
        
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            var assistant = target as AssistantMapElite;
            if(assistant == null) return;

            if (!_loaded)
            {
                dotArea = new DottedAreaFeedback();
                view.AddElement(assistant.OwnerLayer,this,dotArea);
                _loaded = true;
            }
            
            var size = (assistant.OwnerLayer.TileSize * LBSSettings.Instance.general.TileSize).ToInt();
            var start = new Vector2(assistant.RawToolRect.min.x, -assistant.RawToolRect.min.y + 1) * size;
            var end = new Vector2(assistant.RawToolRect.max.x, -assistant.RawToolRect.max.y + 1) * size;
            
            dotArea.SetPosition(Rect.zero);
            dotArea.ActualizePositions(start.ToInt(), end.ToInt());
            dotArea.SetColor(LBSSettings.Instance.view.errorColor);
            
        }

        public override void HideVisuals(object target, MainView view, Vector2 teselationSize)
        {
            throw new System.NotImplementedException();
        }

        public override void ShowVisuals(object target, MainView view, Vector2 teselationSize)
        {
            throw new System.NotImplementedException();
        }
    }
}