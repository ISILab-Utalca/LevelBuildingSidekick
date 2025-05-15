using ISILab.LBS.Behaviours;
using ISILab.LBS.VisualElements;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using ISILab.LBS.VisualElements.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemovePopulationTile : LBSManipulator
    {
        PopulationBehaviour population;
        protected override string IconGuid { get => "ce08b36a396edbf4394f7a4e641f253d"; }
        
        public RemovePopulationTile() : base()
        {
            feedback = new AreaFeedback();
            feedback.fixToTeselation = true;
            name = "Remove Tile";
            description = "Click on an item in the graph to remove it.";
        }

        public override void Init(LBSLayer layer, object owner)
        {
            base.Init(layer, owner);
            
            population = owner as PopulationBehaviour;
            feedback.TeselationSize = layer.TileSize;
            layer.OnTileSizeChange += (val) => feedback.TeselationSize = val;
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
        {
            var x = LBSController.CurrentLevel;
            EditorGUI.BeginChangeCheck();
            Undo.RegisterCompleteObjectUndo(x, "Remove element population");

            var corners = population.OwnerLayer.ToFixedPosition(StartPosition, EndPosition);

            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    population.RemoveTileGroup(new Vector2Int(i, j));
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(x);
            }
            
        }
    }
}