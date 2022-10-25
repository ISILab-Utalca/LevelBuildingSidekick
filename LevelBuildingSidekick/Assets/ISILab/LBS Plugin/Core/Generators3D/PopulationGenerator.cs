using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Representation;
using Utility;
using System.Linq;
using UnityEditor;

namespace LBS.Generator
{
    public class PopulationGenerator : Generator
    {
        private LBSStampGroupData population;
        private float tileSize = 1f;

        public override GameObject Generate()
        {
            if (population == null)
            {
                Debug.LogWarning("cannot be generated, there is no information about the population to load.");
                return null;
            }

            var pressets = DirectoryTools.GetScriptables<StampPresset>();
            var mainPivot = new GameObject("Population group");
            foreach (var popu in population.GetStamps())
            {
                var presset = pressets.Find(p => p.Label == popu.Label); 
                var prefabs = presset.Prefabs;
                var go = prefabs[Random.Range(0,prefabs.Count())];
                var inst = SceneView.Instantiate(go,mainPivot.transform);
                var pos = popu.Position;
                inst.transform.position = new Vector3(pos.x * tileSize, 0, -pos.y * tileSize);
            }

            return mainPivot;
        }

        public override void Init(LevelData levelData)
        {
            this.population = levelData.GetRepresentation<LBSStampGroupData>();
            tileSize = levelData.TileSize;
        }
    }
}