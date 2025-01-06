using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Optimization.Utils
{

    public class NumbersSet
    {
        private static NumbersSet instance = new NumbersSet();
        public static NumbersSet Instance
        {
            get => instance ?? new NumbersSet();
        }

        public Dictionary<int, List<string>> dict = new Dictionary<int, List<string>>();
        public Dictionary<string, List<int>> dictKeys;
        public Dictionary<string, List<int>> extraKeys = new() // parche
    {
        {"Corner", new List<int>()},
        {"Convex", new List<int>()},
        {"Concave", new List<int>()},
        {"Wall", new List<int>()}
    };

        // [R(1,0),RU(1,1),U(0,1),LU(-1,1),L(-1,0),LD(-1,-1),D(0,-1),RD(1,-1)]
        // [R    ,RU   ,U    ,LU    ,L     ,LD     ,D     ,RD    ]
        // [(1,0),(1,1),(0,1),(-1,1),(-1,0),(-1,-1),(0,-1),(1,-1)]
        private (string, string)[] words = new (string, string)[]
        {
        ("101xxxxx","Concave Corner RU"),
        ("xx101xxx","Concave Corner RD"),
        ("xxxx101x","Concave Corner LU"),
        ("1xxxxx10","Concave Corner LD"),
        ("000xxxxx","Convex Corner RU"),
        ("xx000xxx","Convex Corner RD"),
        ("xxxx000x","Convex Corner LU"),
        ("0xxxxx00","Convex Corner LD"),
        ("0xxxxxxx", "Wall Right"),
        ("xx0xxxxx", "Wall Up"),
        ("xxxx0xxx", "Wall Left"),
        ("xxxxxx0x", "Wall Down"),
        ("10001xxx", "Common Wall Right"),
        ("xx0xxxxx", "Common Wall Up"),
        ("xxxx0xxx", "Common Wall Left"),
        ("xxxxxx0x", "Common Wall Down"),
        };

        public NumbersSet()
        {
            var all = GetAll();

            for (int i = 0; i < all.Count; i++)
            {
                var ws = CheckMasks(all[i], words);
                AddWord(i, ws);
            }

            dictKeys = InvertDictionary(dict);

            // parche para encontrar los concaves, convexos, esquinas y paredes
            foreach (var (key, value) in dictKeys)
            {
                foreach (var (word, values) in extraKeys)
                {
                    if (key.Contains(word))
                        extraKeys[word].AddRange(value);
                }
            }
        }

        public void Show()
        {
            foreach (var item in dictKeys)
            {
                var msg = "Word: " + item.Key + "\n";
                foreach (var v in item.Value)
                {
                    msg += v + ", ";
                }
                Debug.Log(msg);
            }
        }

        private static Dictionary<string, List<int>> InvertDictionary(Dictionary<int, List<string>> original)
        {
            var toR = new Dictionary<string, List<int>>();

            foreach (var kvp in original)
            {
                int key = kvp.Key;
                List<string> values = kvp.Value;

                foreach (string value in values)
                {
                    if (!toR.ContainsKey(value))
                    {
                        toR[value] = new List<int>();
                    }
                    toR[value].Add(key);
                }
            }

            return toR;
        }

        private List<string> CheckMasks(BitArray value, (string, string)[] mask)
        {
            var toR = new List<string>();
            for (int j = 0; j < mask.Length; j++)
            {
                var x = true;
                for (int k = 0; k < 8; k++)
                {
                    var v = mask[j].Item1[k];

                    if (v == 'x')
                        continue;

                    var A = value.Get(k);
                    var B = ('1' == v);

                    if (value[k] != ('1' == v))
                    {
                        x = false;
                        break;
                    }
                }

                if (x) toR.Add(mask[j].Item2);
            }
            return toR;
        }

        private void AddWord(int i, List<string> ws)
        {
            if (dict.ContainsKey(i))
            {
                dict[i].AddRange(ws);
            }
            else
            {
                dict.Add(i, ws);
            }
        }

        private List<BitArray> GetAll()
        {
            var all = new List<BitArray>();

            for (int i = 0; i < 256; i++)
            {
                all.Add(GeneralUtils.IntToBitArray(i));
            }
            return all;
        }

        public static bool IsConvexCorner(int value)
        {
            var numbers = NumbersSet.Instance;
            numbers.extraKeys.TryGetValue("Convex", out var values);
            return values.Contains(value);
            //return ((value != 0) && (value % 3 == 0 || value == 7 || value == 11 || value == 13 || value == 14 || value == 0)); // FIX: Esto deberia usar el sistema de conjuntos por palabras
        }

        public static bool IsConcaveCorner(int value)
        {
            var numbers = NumbersSet.Instance;
            numbers.extraKeys.TryGetValue("Concave", out var values);
            return values.Contains(value);
            //return (value == 0 || value == 1 || value == 2 || value == 4 || value == 8); // FIX: Esto deberia usar el sistema de conjuntos por palabras
        }

        public static bool IsWall(int value)
        {
            var numbers = NumbersSet.Instance;
            numbers.extraKeys.TryGetValue("Wall", out var values);
            return values.Contains(value);
            //return (value == 1 || value == 2 || value == 4 || value == 8); // FIX: Esto deberia usar el sistema de conjuntos por palabras
        }
    }
}