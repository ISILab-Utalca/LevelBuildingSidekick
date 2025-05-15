using ISILab.Commons;
using ISILab.LBS.Behaviours;
using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Manipulators
{
    public class RemoveConnectionInArea : ManipulateTeselation
    {
        private ExteriorBehaviour exterior;

        protected override string IconGuid { get => "ce08b36a396edbf4394f7a4e641f253d"; }

        public RemoveConnectionInArea():base(){}

        public override void Init(LBSLayer layer, object owner)
        {
            base.Init(layer, owner);
            
            exterior = (ExteriorBehaviour)owner;
            lbsLayer = layer;
        }

        protected override void OnMouseUp(VisualElement target, Vector2Int endPosition, MouseUpEvent e)
        {
            var corners = exterior.OwnerLayer.ToFixedPosition(StartPosition, EndPosition);

            for (int i = corners.Item1.x; i <= corners.Item2.x; i++)
            {
                for (int j = corners.Item1.y; j <= corners.Item2.y; j++)
                {
                    var dirs = Directions.Bidimencional.Edges;
                    for (int k = 0; k < dirs.Count; k++)
                    {
                        var neigthPos = new Vector2Int(i, j) + dirs[k];
                        // Cheack if outside
                        if (neigthPos.x >= corners.Item1.x || neigthPos.x < corners.Item2.x ||
                            neigthPos.y >= corners.Item1.y || neigthPos.y < corners.Item2.y)
                        {
                            // Get tile
                            var tileNeigth = exterior.GetTile(neigthPos);

                            if (tileNeigth != null)
                                exterior.SetConnection(tileNeigth, (k + 2) % 4, "", true);
                        }

                        // Get tile
                        var tile = exterior.GetTile(new Vector2Int(i, j));

                        // Check if neigth is null
                        if (tile == null)
                            continue;

                        // Remove connection
                        exterior.SetConnection(tile, k, "", true);

                    }
                }
            }
        }
    }
}