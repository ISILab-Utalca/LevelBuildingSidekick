using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Components.Teselation
{
    [System.Serializable]
    public abstract class TeselationModule : LBSModule
    {
        public static Vector2 CellSize;

        public Vector2 SnapPosition(Vector2 position)
        {
            Vector2 snappedPosition = new Vector2(
                Mathf.Round(position.x / CellSize.x) * CellSize.x,
                Mathf.Round(position.y / CellSize.y) * CellSize.y
            );
            return snappedPosition;
        }

        public Vector2Int ToMatrixPosition(Vector2 position)
        {
            Vector2 snappedPosition = SnapPosition(position);
            Vector2Int matrixPosition = new Vector2Int(
                Mathf.RoundToInt(snappedPosition.x / CellSize.x),
                Mathf.RoundToInt(snappedPosition.y / CellSize.y)
            );
            return matrixPosition;
        }

        public Vector2 ToWorldPosition(Vector2Int matrixPosition)
        {
            Vector2 worldPosition = new Vector2(
                matrixPosition.x * CellSize.x,
                matrixPosition.y * CellSize.y
            );
            return worldPosition;
        }
    }
}

