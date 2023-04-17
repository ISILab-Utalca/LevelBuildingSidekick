using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Components.Teselation
{
    [System.Serializable]
    public abstract class TeselationModule : LBSModule
    {
        #region FIELDS

        public static Vector2 CellSize;

        #endregion

        #region CONSTRUCTOR

        public TeselationModule() : base() { Key = GetType().Name; }

        public TeselationModule(string key) : base(key) { }

        #endregion

        #region METHODS

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

        public Vector2Int ToMatrixPosition(int index)
        {
            var r = GetBounds();
            return new Vector2Int((int)(index % r.width), (int)(index / r.width));
        }

        public Vector2 ToWorldPosition(Vector2Int matrixPosition)
        {
            Vector2 worldPosition = new Vector2(
                matrixPosition.x * CellSize.x,
                matrixPosition.y * CellSize.y
            );
            return worldPosition;
        }

        public int ToIndex(Vector2 matrixPosition)
        {
            var r = GetBounds();
            var pos = matrixPosition - r.position;
            return (int)(pos.y * r.width + pos.x);
        }

        public abstract List<Vector2> OccupiedPositions();

        public abstract List<Vector2> EmptyPositions();

        public abstract List<int> OccupiedIndexes();

        public abstract List<int> EmptyIndexes();

        public override abstract bool IsEmpty();

        #endregion
    }
}

