using Optimization.ACO;
using Optimization.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Optimization.Utils
{

    public static class GeneralUtils
    {
        public static int BitArrayToInt(BitArray bits)
        {
            if (bits.Length != 8)
                throw new ArgumentException("El BitArray debe tener exactamente 8 bits.");

            byte[] bytes = new byte[1];
            bits.CopyTo(bytes, 0);
            return bytes[0];
        }

        public static BitArray IntToBitArray(int number)
        {
            return new BitArray(new[] { number });
        }

        public static void GenerateSizedImage(Map map, Graph graph, int pixelSize, string fileName, string path)
        {
            var ((cords, rooms, tiles), w, h) = map.ToTileMatrix();
            var max = Math.Max(w, h);
            var t = new Texture2D(max * pixelSize, max * pixelSize);
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    var id = rooms[j, i];

                    if (string.IsNullOrEmpty(id))
                    {
                        for (int x = 0; x < pixelSize; x++)
                        {
                            for (int y = 0; y < pixelSize; y++)
                            {
                                t.SetPixel(j * pixelSize + x, i * pixelSize + y, Color.black);

                                if (x == 0 || y == 0)
                                {
                                    t.SetPixel(j * pixelSize + x, i * pixelSize + y, Color.white);
                                }
                            }
                        }
                    }

                    var node = graph.nodes.Find(n => n.id == id);

                    for (int x = 0; x < pixelSize; x++)
                    {
                        for (int y = 0; y < pixelSize; y++)
                        {
                            t.SetPixel(j * pixelSize + x, i * pixelSize + y, node.color);

                            if (x == 0 || y == 0)
                            {
                                t.SetPixel(j * pixelSize + x, i * pixelSize + y, Color.white);
                            }
                        }
                    }
                }
            }

            byte[] pngData = t.EncodeToPNG();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Console.WriteLine("Folder created successfully.");
            }

            string filePath = Path.Combine(path, fileName);
            File.WriteAllBytes(filePath, pngData);

            Debug.Log("Textura guardada como " + fileName + " en la ruta: " + filePath);
        }

        public static void GenerateImage(Map map, Graph graph, string fileName, string path)
        {
            var ((cords, rooms, tiles), w, h) = map.ToTileMatrix();
            var t = new Texture2D(w, h);
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    var id = rooms[j, i];

                    if (string.IsNullOrEmpty(id))
                        t.SetPixel(j, i, Color.black);

                    var node = graph.nodes.Find(n => n.id == id);

                    t.SetPixel(j, i, node.color);
                }
            }

            byte[] pngData = t.EncodeToPNG();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Console.WriteLine("Folder created successfully.");
            }

            string filePath = Path.Combine(path, fileName);
            File.WriteAllBytes(filePath, pngData);

            Debug.Log("Textura guardada como " + fileName + " en la ruta: " + filePath);
        }

        // FIX?: Esto se podria hacer de otra forma pa que sea menos ciclos.
        [Obsolete]
        public static List<Vector2Int> GetNeigborPositions(List<Vector2Int> pos, List<Vector2Int> dirs)
        {
            var toR = new List<Vector2Int>();
            toR.AddRange(pos);

            for (int i = 0; i < pos.Count; i++)
            {
                for (int j = 0; j < dirs.Count; j++)
                {
                    var n = pos[i] + dirs[j];
                    if (!toR.Contains(n))
                        toR.Add(n);
                }
            }
            return toR;
        }

        public static T RandomRullete<T>(this List<T> list, Func<T, float> predicate)
        {
            if (list.Count <= 0)
            {
                Debug.LogError("La lista esta vacia.");
                return default(T);
            }

            var pairs = new List<Tuple<T, float>>();
            for (int i = 0; i < list.Count(); i++)
            {
                var value = predicate(list[i]);

                if (value <= 0)
                    continue;

                pairs.Add(new Tuple<T, float>(list[i], value));
            }

            if (pairs.Count <= 0)
            {
                Debug.LogWarning("No hay valores validos para la ruleta.");
                return default(T);
            }

            var total = pairs.Sum(p => p.Item2);
            var rand = Random.Range(0.0f, total);

            var cur = 0f;
            for (int i = 0; i < pairs.Count; i++)
            {
                cur += pairs[i].Item2;
                if (rand <= cur)
                {
                    return pairs[i].Item1;
                }
            }
            return default(T);
        }

        public static T GetRandom<T>(this List<T> list)
        {
            if (list.Count <= 0)
            {
                return default(T);
            }

            return list[Random.Range(0, list.Count)];
        }

        public static List<Vector2Int> GetPointsBetween(Vector2Int start, Vector2Int end)
        {
            var toR = new List<Vector2Int>();

            if (start.x == end.x) // Alineado en el eje Y
            {
                int minY = Math.Min(start.y, end.y);
                int maxY = Math.Max(start.y, end.y);
                for (int y = minY; y <= maxY; y++)
                {
                    toR.Add(new Vector2Int(start.x, y));
                }
            }
            else if (start.y == end.y) // Alineado en el eje X
            {
                int minX = Math.Min(start.x, end.x);
                int maxX = Math.Max(start.x, end.x);
                for (int x = minX; x <= maxX; x++)
                {
                    toR.Add(new Vector2Int(x, start.y));
                }
            }
            else
            {
                throw new ArgumentException(
                    "The points must be aligned either" +
                    " on the x-axis or the y-axis.");
            }

            return toR;
        }

        internal static void GenerateCSV<T>(T data, string fileName, string folderPath)
        {
            // Verificar si el tipo de datos es Data
            if (data is _Data experimentData)
            {
                // Construir la ruta completa del archivo
                string filePath = Path.Combine(folderPath, fileName);

                // Usar StringBuilder para crear el contenido del CSV
                StringBuilder csvContent = new StringBuilder();

                // Añadir encabezados
                csvContent.AppendLine(
                    "Generation;" +             // 1
                    "TotalTime;" +              // 2
                    "ExplorationTime;" +        // 3
                    "EvaluatorTime;" +          // 4
                    "BestGenPath;" +            // 6
                    "AveragePathValue;" +       // 7
                    "SuccessfulPathCount;" +    // 8
                    "DeadEndCount;" +            // 9
                    "NeigExplored");            // 10

                // Iterar sobre las generaciones y añadir datos
                for (int i = 0; i < experimentData.generations.Count; i++)
                {
                    var gen = experimentData.generations[i];
                    string line = $"{(i + 1)};" +             // Generación 1
                        $"{gen.totalTime}e-03;" +           // TotalTime 2
                        $"{gen.explorationTime}e-03;" +     // ExplorationTime 3
                        $"{gen.evaluatorTime}e-03;" +       // EvaluatorTime 4
                        $"{gen.genBest};" +                 // BestGenPath 6
                        $"{gen.average};" +                 // AveragePathValue 7
                        $"{gen.successfulPathCount};" +     // SuccessfulPathCount 8
                        $"{gen.deadEndCount};" +             // DeadEndCount 9
                        $"{gen.niegExplored}";             // neigExplored 10
                    csvContent.AppendLine(line);
                }

                // Añadir tiempo total de ejecución al final del archivo CSV
                csvContent.AppendLine($"Total Execution Time;{experimentData.totalTime}");
                csvContent.AppendLine($"TotalNeigExplored;{experimentData.totalNeigsExplored}");
                csvContent.AppendLine($"Best;{experimentData.best.Item2}");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    Console.WriteLine("Folder created successfully.");
                }

                // Escribir el contenido en el archivo CSV
                File.WriteAllText(filePath, csvContent.ToString(), Encoding.UTF8);

                Debug.Log($"Data exported to CSV file at: {filePath}");
            }
            else
            {
                Debug.LogError("Invalid data type. Expected type is Data.");
            }
        }
    }
}