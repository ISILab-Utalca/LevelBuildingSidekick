using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

// GABO TODO: TERMINARRR
namespace ISILab.LBS.VisualElements
{
    public class PathOSTileView : GraphElement
    {

        #region FIELDS
        #endregion

        #region FIELDS VIEW
        private static VisualTreeAsset view;
        // Test : PositionLabel
        Label positionLabel;
        // Icono del tag
        VisualElement background;
        VisualElement elementTag;
        // Event tags
        VisualElement dynamicTagObject;
        VisualElement dynamicTagTrigger;
        VisualElement dynamicObstacleObject;
        VisualElement dynamicObstacleTrigger;
        #endregion

        #region CONSTRUCTORS
        public PathOSTileView(PathOSTile tile)
        {
            if (view == null)
            {
                view = DirectoryTools.GetAssetByName<VisualTreeAsset>("PathOSTileView");
            }
            view.CloneTree(this);

            //Test
            positionLabel = this.Q<Label>(name: "PositionLabel");
            //
            background = this.Q<VisualElement>(name: "Background");
            elementTag = this.Q<VisualElement>(name: "ElementTag");
            dynamicTagObject = this.Q<VisualElement>(name: "DynamicTagObject");
            dynamicTagTrigger = this.Q<VisualElement>(name: "DynamicTagTrigger");
            dynamicObstacleObject = this.Q<VisualElement>(name: "DynamicObstacleObject");
            dynamicObstacleTrigger = this.Q<VisualElement>(name: "DynamicObstacleTrigger");

            PathOSStorage storage = PathOSStorage.Instance;
            // Set data
            if(tile.Tag != null)
            {
                PathOSStorage.SimulationEntityData data = tile.Tag.Label.Equals("Player") ?
                    storage.agentData :
                    storage.entityDataPool[tile.Tag.EntityType];
                //SetColor(tile.Tag.Color);
                //SetImage(tile.Tag.Icon);
                SetColor(data.color);
                SetImage(data.image);
            }
            SetEvents(tile);
        }
        #endregion

        #region METHODS
        public void SetColor(Color color)
        {
            background.style.backgroundColor = color;
        }

        public void SetImage(VectorImage image)
        {
            elementTag.style.backgroundImage = new StyleBackground(image);
        }

        public void SetImage(Texture2D image)
        {
            elementTag.style.backgroundImage = image;
        }

        // Asigna posicion y modifica opacidad de elementos visuales asoc. a Event Tags,
        // segun info recibida del eventTile.
        public void SetEvents(PathOSTile tile)
        {
            // Chequeo nulo
            if (tile == null) { Debug.LogWarning("PathOSTileView.SetEvents(): Tile nulo!"); return; }
            // Chequeo tag nulo
            if (tile.Tag == null) { Debug.LogWarning("PathOSTileView.SetEvents(): Tile tiene tag nulo!"); }

            // Setear posicion
            //positionLabel.text = $"{tile.X} x {tile.Y}";

            // Setear opacidad de event tags segun info del tile
            dynamicTagObject.style.opacity = tile.IsDynamicTagObject ? 1f : 0f;
            dynamicTagTrigger.style.opacity = tile.IsDynamicTagTrigger ? 1f : 0f;
            dynamicObstacleObject.style.opacity = tile.IsDynamicObstacleObject ? 1f : 0f;
            dynamicObstacleTrigger.style.opacity = tile.IsDynamicObstacleTrigger ? 1f : 0f;
        }
        #endregion
    }
}
