using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Modules;
using ISILab.LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//GABO TODO: Terminar
namespace ISILab.LBS.VisualElements
{
    public class PathOSTriggerInfoPanel : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<PathOSTriggerInfoPanel, UxmlTraits> { }
        #endregion

        #region FIELDS
        #endregion

        #region FIELDS VIEW
        private ScrollView obstacleList;
        private ScrollView tagList;
        #endregion

        #region CONSTRUCTORS
        public PathOSTriggerInfoPanel()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("PathOSTriggerInfoPanel");
            visualTree.CloneTree(this);

            // Obstacle List
            this.obstacleList = this.Q<ScrollView>("ObstacleList");
            // Tag List
            this.tagList = this.Q<ScrollView>("TagList");
        }
        #endregion

        #region METHODS
        public void RefreshObstacles(PathOSTile tile)
        {
            // Clear old obstacles
            ClearObstacles();

            // Create PathOSObstacleView objects, then add.
            var obstacles = tile.GetObstacles();
            if (obstacles == null) return;

            foreach(var obstacle in obstacles)
            {
                PathOSObstacleView newView = new(tile, obstacle.Item1, obstacle.Item2);
                obstacleList.Add(newView);
            }
        }

        public void ClearObstacles() { obstacleList.Clear(); }
        public void ClearTags() { tagList.Clear(); }
        #endregion

    }
}
