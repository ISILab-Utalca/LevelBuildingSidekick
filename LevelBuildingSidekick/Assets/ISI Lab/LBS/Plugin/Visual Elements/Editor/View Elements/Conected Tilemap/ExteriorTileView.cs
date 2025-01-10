using System;
using ISILab.Commons.Utility.Editor;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Internal;
using ISILab.LBS.Components;

namespace ISILab.LBS.VisualElements
{
    public class ExteriorTileView : GraphElement
    {
        
        private VisualElement leftConnection, leftSide;
        private VisualElement rightConnection,rightSide;
        private VisualElement topConnection, topSide;
        private VisualElement bottomConnection, bottomSide;
        
        private VisualElement fill, center;

        private static VisualTreeAsset view;
        
        private static float scaleFactor = 1f;
        private static Color fillColor = Color.grey;
        private static float colorBrightenner = 0.1f;
        public ExteriorTileView(List<string> connections)
        {
            if (view == null)
            {
                view = DirectoryTools.GetAssetByName<VisualTreeAsset>("ConnectedTile");
            }
            view.CloneTree(this);

            this.SetMargins(0);
            this.SetPaddings(0);

            this.SetBackgroundColor(fillColor);
            this.SetBorderRadius(0);
            this.transform.scale = Vector3.one * scaleFactor;
      
            leftConnection = this.Q<VisualElement>("LeftConnection");
            rightConnection = this.Q<VisualElement>("RightConnection");
            topConnection = this.Q<VisualElement>("TopConnection");
            bottomConnection = this.Q<VisualElement>("BottomConnection");
            
            fill = this.Q<VisualElement>("Fill");
            leftSide = fill.Q<VisualElement>("LeftFill");
            rightSide = fill.Q<VisualElement>("RightFill");
            topSide = fill.Q<VisualElement>("TopFill");
            bottomSide = fill.Q<VisualElement>("BottomFill");
            center = fill.Q<VisualElement>("CenterFill");
            
            SetConnections(connections.ToArray());
        }

        private static void SetBackgroundColor(VisualElement ve, Color color)
        {
            if (ve == null) return;
            ve.style.backgroundColor = color;
        }
        
        private static void SetImageTint(VisualElement ve, Color color)
        {
            if (ve == null) return;
            if (ve.style.backgroundImage == null) return;
            ve.style.unityBackgroundImageTintColor = color;

        }
        
        private static void SetBorderColor(VisualElement ve, Color color)
        {
            if (ve == null) return;
            ve.style.borderBottomColor = color;
            ve.style.borderLeftColor = color;
            ve.style.borderTopColor = color;
            ve.style.borderRightColor = color;
            
        }
        
        public void SetConnections(string[] tags)
        {
            var tts = LBSAssetsStorage.Instance.Get<LBSTag>();
            Color color = Color.magenta; // invalid color
            Dictionary<Color,int> ConnectionColors = new Dictionary<Color, int>();
            if (!string.IsNullOrEmpty(tags[0]))
            {
                color = tts.Find(t => t.Label.Equals(tags[0])).Color;
                SetBackgroundColor(rightConnection, color);
                SetImageTint(rightSide, BrightenColor(color));
                rightConnection.style.display = DisplayStyle.Flex;
                
                if(!ConnectionColors.TryAdd(color, 1)) ConnectionColors[color]++;
                
            }
            else
            {
                rightConnection.style.display = DisplayStyle.None;
            }

            if (!string.IsNullOrEmpty(tags[1]))
            {
                color = tts.Find(t => t.Label.Equals(tags[1])).Color;
                SetBackgroundColor(topConnection, color);
                SetImageTint(topSide, BrightenColor(color));
                topConnection.style.display = DisplayStyle.Flex;
                
                if(!ConnectionColors.TryAdd(color, 1)) ConnectionColors[color]++;
            }
            else
            {
                topConnection.style.display = DisplayStyle.None;
            }

            if (!string.IsNullOrEmpty(tags[2]))
            {
                color = tts.Find(t => t.Label.Equals(tags[2])).Color;
                SetBackgroundColor(leftConnection, color);
                SetImageTint(leftSide, BrightenColor(color));
                leftConnection.style.display = DisplayStyle.Flex;
                
                if(!ConnectionColors.TryAdd(color, 1)) ConnectionColors[color]++;
            }
            else
            {
                leftConnection.style.display = DisplayStyle.None;
            }

            if (!string.IsNullOrEmpty(tags[3]))
            {
                color = tts.Find(t => t.Label.Equals(tags[3])).Color;
                SetBackgroundColor(bottomConnection, color);
                SetImageTint(bottomSide, BrightenColor(color));
                bottomConnection.style.display = DisplayStyle.Flex;
               
                if(!ConnectionColors.TryAdd(color, 1)) ConnectionColors[color]++;
            }
            else
            {
                bottomConnection.style.display = DisplayStyle.None;
            }
            
            // paints center if there are connections and to the most connections
            if (ConnectionColors.Count > 0)
            {
                var orderedConnectionColors = ConnectionColors
                    .OrderByDescending(kvp => kvp.Value)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                SetBackgroundColor(center,orderedConnectionColors.First().Key);
            }
   
        }

        public void SetTileCenter(LBSTag identifier)
        {
            var color = identifier.Color;
            SetBackgroundColor(center,color);
            SetImageTint(bottomSide, BrightenColor(color));
            SetImageTint(topSide, BrightenColor(color));
            SetImageTint(leftSide, BrightenColor(color));
            SetImageTint(rightSide, BrightenColor(color));
        }

        private static Color BrightenColor(Color color)
        {
            color.r += colorBrightenner;
            color.b += colorBrightenner;
            color.g += colorBrightenner;
            return color;
        }
    }
}