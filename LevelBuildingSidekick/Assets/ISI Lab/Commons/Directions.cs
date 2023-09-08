using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Directions
{
    /// <summary>
    /// A static class that provides predefined lists of 3D vectors
    /// representing centers, edges, and diagonals in a 3D space.
    /// </summary>
    public static class Tridimencional
    {
        private static readonly List<Vector3Int> centers = new List<Vector3Int>()
        {
            new Vector3Int(1,0,0),
            new Vector3Int(0,1,0),
            new Vector3Int(0,0,1),
            new Vector3Int(-1,0,0),
            new Vector3Int(0,-1,0),
            new Vector3Int(0,0,-1),
        };

        private static readonly List<Vector3Int> diagonals = new List<Vector3Int>()
        {
            new Vector3Int(1, 1, 1),
            new Vector3Int(-1, 1, 1),
            new Vector3Int(1, -1, 1),
            new Vector3Int(1, 1, -1),
            new Vector3Int(-1, -1, 1),
            new Vector3Int(-1, 1, -1),
            new Vector3Int(1, -1, -1),
            new Vector3Int(-1, -1, -1)
        };

        private static readonly List<Vector3Int> edges = new List<Vector3Int>()
        {
            new Vector3Int(1, 1, 0),
            new Vector3Int(1, -1, 0),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, -1, 0),
            new Vector3Int(1, 0, 1),
            new Vector3Int(1, 0, -1),
            new Vector3Int(-1, 0, 1),
            new Vector3Int(-1, 0, -1),
            new Vector3Int(0, 1, 1),
            new Vector3Int(0, 1, -1),
            new Vector3Int(0, -1, 1),
            new Vector3Int(0, -1, -1)
        };

        public static List<Vector3Int> Centers => centers;
        public static List<Vector3Int> Edges => edges;
        public static List<Vector3Int> Diagonals => diagonals;
        public static List<Vector3Int> All
        {
            get
            {
                var all = new List<Vector3Int>();
                all.AddRange(centers);
                all.AddRange(edges);
                all.AddRange(diagonals);
                return all;
            }
        }

        /// <summary>
        /// Finds the perpendicular directions to a given axis.
        /// </summary>
        /// <param name="axis">The axis for which to find the perpendicular directions.</param>
        /// <returns>Perpendicular directions to the axis.</returns>
        public static List<Vector3Int> GetPerpendicularDirs(Vector3Int axis, bool whitDiagonals = false)
        {
            var toR = new List<Vector3Int>();

            foreach (var direction in Centers)
            {
                if (Vector3.Dot(axis, direction) == 0)
                {
                    toR.Add(direction);
                }
            }

            if (!whitDiagonals)
                return toR;

            foreach (var direction in Edges)
            {
                if (Vector3.Dot(axis, direction) == 0)
                {
                    toR.Add(direction);
                }
            }

            return toR;
        }
    }

    /// <summary>
    /// A static class that provides predefined lists of 2D vectors 
    /// representing edges, diagonals, and all directions.
    /// </summary>
    public static class Bidimencional
    {
        private readonly static List<Vector2Int> edges = new List<Vector2Int>()
        {
            new Vector2Int(1, 0),    // Derecha
            new Vector2Int(0, 1),    // Arriba
            new Vector2Int(-1, 0),   // Izquierda
            new Vector2Int(0, -1),   // Abajo
        };

        private readonly static List<Vector2Int> diagonals = new List<Vector2Int>()
        {
            new Vector2Int(1, 1),    // Diagonal superior derecha
            new Vector2Int(-1, 1),   // Diagonal superior izquierda
            new Vector2Int(-1, -1),  // Diagonal inferior izquierda
            new Vector2Int(1, -1)    // Diagonal inferior derecha
        };

        private readonly static List<Vector2Int> all = new List<Vector2Int>()
        {
            new Vector2Int(1, 0),     // Derecha
            new Vector2Int(1, 1),      // Diagonal superior derecha
            new Vector2Int(0, 1),     // Arriba
            new Vector2Int(-1, 1),    // Diagonal superior izquierda
            new Vector2Int(-1, 0),    // Izquierda
            new Vector2Int(-1, -1),   // Diagonal inferior izquierda
            new Vector2Int(0, -1),    // Abajo
            new Vector2Int(1, -1)    // Diagonal inferior derecha
        };

        /// <summary>
        /// List of vectors representing edges.
        /// </summary>
        public static List<Vector2Int> Edges => edges;

        /// <summary>
        /// List of vectors representing diagonals.
        /// </summary>
        public static List<Vector2Int> Diagonals => diagonals;

        /// <summary>
        /// List of vectors representing all directions.
        /// </summary>
        public static List<Vector2Int> All => all;
    }
}
