using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//GABO TODO: TERMINAR
namespace ISILab.LBS.VisualElements
{
    public class PathOSObstacleView : VisualElement
    {

        #region FIELDS
        //PathOSTile triggerTile;
        #endregion

        #region FIELDS VIEW
        public VisualElement icon;
        public Label positionLabel;
        public Label stateLabel;
        public Label removeButton;
        #endregion

        #region PROPERTIES
        public Texture2D Icon
        {
            set => icon.style.backgroundImage = value;
        }
        #endregion

        #region CONSTRUCTORS
        public PathOSObstacleView(PathOSTile obstacleTile, PathOSObstacleConnections.Category category)
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("PathOSObstacleView");
            visualTree.CloneTree(this);

            this.icon = this.Q<VisualElement>("Icon");
            this.positionLabel = this.Q<Label>("PositionLabel");
            this.stateLabel = this.Q<Label>("StateLabel");
            this.removeButton = this.Q<Label>("RemoveButton");

            // Set fields
            SetFields(obstacleTile, category);
        }
        #endregion

        #region METHODS
        private void SetFields(PathOSTile obstacleTile, PathOSObstacleConnections.Category category)
        {
            // Obstacle object+trigger tile check
            if (!obstacleTile.IsDynamicObstacleObject) { Debug.LogWarning("PathOSObstacleView.SetFields(): Tile dada no es obstaculo!"); return; }

            //this.triggerTile = triggerTile;
            icon.style.backgroundImage = obstacleTile.Tag.Icon;
            positionLabel.text = $"{obstacleTile.X} x {obstacleTile.Y}";
            stateLabel.text = category.ToString();
        }
        #endregion
    }
}