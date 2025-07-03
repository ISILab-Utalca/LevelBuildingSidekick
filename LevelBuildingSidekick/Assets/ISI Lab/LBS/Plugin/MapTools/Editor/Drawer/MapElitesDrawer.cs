using ISILab.LBS.Settings;
using UnityEngine;
using ISILab.Extensions;
using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.Assistants;
using ISILab.LBS.VisualElements;
using UnityEngine.UIElements;

namespace ISILab.LBS.Drawers
{
    [Drawer(typeof(AssistantMapElite))]
    public class MapElitesDrawer : Drawer
    {
        private DottedAreaFeedback _dotArea;
        
        public override void Draw(object target, MainView view, Vector2 teselationSize)
        {
            var assistant = target as AssistantMapElite;
            if(assistant == null) return;

            if (!Loaded)
            {
                _dotArea = new DottedAreaFeedback();
                view.AddElement(assistant.OwnerLayer,this,_dotArea);
                Loaded = true;
            }

            if (!assistant.visible) return;
            
            var size = (assistant.OwnerLayer.TileSize * LBSSettings.Instance.general.TileSize).ToInt();
            var start = new Vector2(assistant.RawToolRect.min.x, -assistant.RawToolRect.min.y + 1) * size;
            var end = new Vector2(assistant.RawToolRect.max.x, -assistant.RawToolRect.max.y + 1) * size;
            
            _dotArea.SetPosition(Rect.zero);
            _dotArea.ActualizePositions(start.ToInt(), end.ToInt());
            _dotArea.SetColor(LBSSettings.Instance.view.errorColor);
            _dotArea.layer = assistant.OwnerLayer.index;
        }

        public override void HideVisuals(object target, MainView view, Vector2 teselationSize)
        {
            if (!Loaded) return;
            _dotArea.style.display = DisplayStyle.None;
        }

        public override void ShowVisuals(object target, MainView view, Vector2 teselationSize)
        {
            if (!Loaded) return;
            _dotArea.style.display = DisplayStyle.Flex;
        }
    }
}