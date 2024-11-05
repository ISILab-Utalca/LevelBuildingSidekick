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
        private ListView obstacleList;
        private ListView tagList;
        #endregion

        #region CONSTRUCTORS
        public PathOSTriggerInfoPanel()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("PathOSTriggerInfoPanel");
            visualTree.CloneTree(this);

            // Obstacle List
            this.obstacleList = this.Q<ListView>("ObstacleList");
            // Tag List
            this.tagList = this.Q<ListView>("TagList");
        }
        #endregion

        #region METHODS
        public void AddObstacles(PathOSTile tile)
        {
            // Clear obstacles
            ClearObstacles();

            // Create PathOSObstacleView objects, then add.
            var obstacles = tile.GetObstacles();
            foreach(var obstacle in obstacles)
            {
                //GABO TODO: TERMINAR (terminar PathOSObstacleView y conectar aqui)
                PathOSObstacleView newView = new(obstacle.Item1, obstacle.Item2);
                obstacleList.Add(newView);
            }
        }

        public void ClearObstacles() { obstacleList.Clear(); }
        public void ClearTags() { tagList.Clear(); }
        #endregion

    }
}
