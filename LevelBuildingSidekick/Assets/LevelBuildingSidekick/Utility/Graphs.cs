using System.Collections.Generic;
using UnityEngine;
using System;
using GraphPlanarityTesting.PlanarityTesting.BoyerMyrvold;
using GraphPlanarityTesting.Graphs.DataStructures;
using System.Drawing;

namespace Utility
{
    ///////////////////////
    public struct Links
    {
        public PointF p1;
        public PointF p2;
    }

    public class Graphs
    {
        public static Boolean grafoPlanar(List<Vector2> listaPuntos, Boolean[,] matriz)
        {
            UndirectedAdjacencyListGraph<PointF> grafo = new UndirectedAdjacencyListGraph<PointF>();
            List<PointF> listaPuntos2 = new List<PointF>();

            ///Agregamos puntos
            foreach (Vector2 p1 in listaPuntos)
            {
                PointF p2 = new PointF(p1.x, p1.y);
                listaPuntos2.Add(p2);
                grafo.AddVertex(p2);
            }

            ///Agregamos Conexiones
            for (int fila = 0; fila < listaPuntos.Count; fila++)
            {
                for (int columna = 0; columna < listaPuntos.Count; columna++)
                {
                    if (matriz[fila, columna])
                    {

                        Links e1 = new Links();
                        e1.p1 = listaPuntos2[fila]; e1.p2 = listaPuntos2[columna];

                        grafo.AddEdge(e1.p1, e1.p2);
                    }

                }
            }


            BoyerMyrvold<PointF> k8 = new BoyerMyrvold<PointF>();

            if (k8.IsPlanar(grafo))
                return true;
            else
                return false;

        }
    }
}
