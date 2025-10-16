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
    public class EdgeExteriorTileView : ExteriorTileView
    {
        private static VisualTreeAsset view;

        private VisualElement leftConnection, leftSide;
        private VisualElement rightConnection, rightSide;
        private VisualElement topConnection, topSide;
        private VisualElement bottomConnection, bottomSide;

        private VisualElement fill, center;

        public EdgeExteriorTileView(List<string> connections = null) : base(connections, "ConnectedTile")
        {
            connections ??= new List<string>() { "", "", "", "" };

            if (view == null)
            {
                view = DirectoryTools.GetAssetByName<VisualTreeAsset>("ConnectedTile");
            }
            view.CloneTree(this);

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

        public override void SetConnections(string[] tags)
        {
            var tts = LBSAssetsStorage.Instance.Get<LBSTag>();
            Color invalidColor = Color.white;
            Color color = invalidColor;
            Dictionary<Color, int> ConnectionColors = new Dictionary<Color, int>();
            if (!string.IsNullOrEmpty(tags[0]))
            {
                color = tts.Find(t => t.Label.Equals(tags[0])).Color;
                SetBackgroundColor(rightConnection, color);
                SetImageTint(rightSide, BrightenColor(color));
                rightConnection.style.display = DisplayStyle.Flex;

                if (!ConnectionColors.TryAdd(color, 1)) ConnectionColors[color]++;

            }
            else
            {
                SetImageTint(rightSide, invalidColor);
                rightConnection.style.display = DisplayStyle.None;
            }

            if (!string.IsNullOrEmpty(tags[1]))
            {
                color = tts.Find(t => t.Label.Equals(tags[1])).Color;
                SetBackgroundColor(topConnection, color);
                SetImageTint(topSide, BrightenColor(color));
                topConnection.style.display = DisplayStyle.Flex;

                if (!ConnectionColors.TryAdd(color, 1)) ConnectionColors[color]++;
            }
            else
            {
                SetImageTint(topSide, invalidColor);
                topConnection.style.display = DisplayStyle.None;
            }

            if (!string.IsNullOrEmpty(tags[2]))
            {
                color = tts.Find(t => t.Label.Equals(tags[2])).Color;
                SetBackgroundColor(leftConnection, color);
                SetImageTint(leftSide, BrightenColor(color));
                leftConnection.style.display = DisplayStyle.Flex;

                if (!ConnectionColors.TryAdd(color, 1)) ConnectionColors[color]++;
            }
            else
            {
                SetImageTint(leftSide, invalidColor);
                leftConnection.style.display = DisplayStyle.None;
            }

            if (!string.IsNullOrEmpty(tags[3]))
            {
                color = tts.Find(t => t.Label.Equals(tags[3])).Color;
                SetBackgroundColor(bottomConnection, color);
                SetImageTint(bottomSide, BrightenColor(color));
                bottomConnection.style.display = DisplayStyle.Flex;

                if (!ConnectionColors.TryAdd(color, 1)) ConnectionColors[color]++;
            }
            else
            {
                SetImageTint(bottomSide, invalidColor);
                bottomConnection.style.display = DisplayStyle.None;
            }

            // paints center if there are connections and to the most connections
            if (ConnectionColors.Count > 0)
            {
                var orderedConnectionColors = ConnectionColors
                    .OrderByDescending(kvp => kvp.Value)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                SetBackgroundColor(center,
                    tags.Contains("") ? invalidColor : orderedConnectionColors.First().Key);
            }
            else
            {
                SetBackgroundColor(center, invalidColor);
            }
        }

        public override void SetTileCenter(LBSTag identifier)
        {
            var color = identifier.Color;
            SetBackgroundColor(center, color);
            SetImageTint(bottomSide, BrightenColor(color));
            SetImageTint(topSide, BrightenColor(color));
            SetImageTint(leftSide, BrightenColor(color));
            SetImageTint(rightSide, BrightenColor(color));
        }
    }
}
