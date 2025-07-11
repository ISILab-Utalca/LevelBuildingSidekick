using System.Collections;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using UnityEngine;

namespace ISILab.AI.Categorization
{
    public abstract class ChromosomeBase2D : ChromosomeBase
    {
        public Rect Rect { get; set; }

        protected ChromosomeBase2D(Rect rect, int[] immutables = null, int[] invalids = null) : base((int)(rect.width * rect.height), immutables, invalids)
        {
            Rect = rect;
        }

        protected ChromosomeBase2D() : base()
        {
            Rect = Rect.zero;
            immutableIndexes = new int[0];
            invalidIndexes = new int[0];
        }

        /// <summary>
        /// Converts a 2D position to an index in a 1D array.
        /// </summary>
        /// <param name="pos">The 2D position to convert.</param>
        public int ToIndex(Vector2 pos)
        {
            if (pos.x < 0 || pos.x >= Rect.width || pos.y < 0 || pos.y >= Rect.height)
                return -1;
            return (int)(pos.y * Rect.width + pos.x);
        }

        /// <summary>
        /// Converts an index in a 1D array to a 2D position.
        /// </summary>
        /// <param name="index">The index in the 1D array to convert.</param>
        public Vector2Int ToMatrixPosition(int index)
        {
            return new Vector2Int((int)(index % Rect.width), (int)(index / Rect.width));
        }
    }
}