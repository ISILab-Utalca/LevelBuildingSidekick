using ISILab.LBS.Assistants;
using LBS.Components;
using ISILab.LBS.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemoveZoneConnection : LBSManipulator
    {
        private HillClimbingAssistant _hillclimbing;

        public RemoveZoneConnection()
        {
            Name = "Remove Assistant zone connection";
            Description = "Click a connection between zones to remove it.";
        }

        protected override string IconGuid => "42830f36abd22544fb35c697171f8374";

        public override void Init(LBSLayer layer, object provider = null)
        {
            base.Init(layer, provider);
            
            _hillclimbing = provider as HillClimbingAssistant;
            layer.OnTileSizeChange += (val) => Feedback.TeselationSize = val;
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int position, MouseUpEvent e)
        {
            base.OnMouseUp(element, position, e);

            //If esc key was pressed, cancel the operation
            if (ForceCancel)
            {
                ForceCancel = false;
                return;
            }

            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Remove Zone Connection");

            Vector2 pos = position / (_hillclimbing.OwnerLayer.TileSize * LBSSettings.Instance.general.TileSize);
            pos = new Vector2(pos.x, -(pos.y - 1));

            _hillclimbing.RemoveZoneConnection(pos, 0.2f);


            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }
    }
}