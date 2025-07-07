using ISILab.LBS.Behaviours;
using LBS.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemoveTileExterior : ManipulateTeselation
    {
        private ExteriorBehaviour _exterior;
        protected override string IconGuid => "ce08b36a396edbf4394f7a4e641f253d";
        
        public RemoveTileExterior()
        {
            Name = "Remove Tile";
            Description = "Click on a Tile or select an area to remove multiple tiles.";
        }
        
        public override void Init(LBSLayer layer, object provider)
        {
            base.Init(layer, provider);

            _exterior = provider as ExteriorBehaviour;
            LBSLayer = layer;
        }

        protected override void OnMouseUp(VisualElement element, Vector2Int position, MouseUpEvent e)
        {
            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Remove Tiles");

            var corners = _exterior.OwnerLayer.ToFixedPosition(StartPosition, EndPosition);

            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    var pos = new Vector2Int(i, j);
                    var tile = _exterior.GetTile(pos);
                    if (tile == null)
                        continue;
                    _exterior.RemoveTile(tile);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
        }
    }
}