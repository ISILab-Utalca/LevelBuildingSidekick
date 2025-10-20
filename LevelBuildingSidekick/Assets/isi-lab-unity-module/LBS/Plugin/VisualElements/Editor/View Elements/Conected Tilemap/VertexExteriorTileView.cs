using ISILab.Commons.Utility.Editor;
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
    public class VertexExteriorTileView : ExteriorTileView
    {
        private static VisualTreeAsset view;

        private VisualElement upperRightFill;
        private VisualElement upperLeftFill;
        private VisualElement lowerLeftFill;
        private VisualElement lowerRightFill;

        //private VisualElement fill, center;
        private VisualElement center;

        public VertexExteriorTileView(List<string> connections = null) : base(connections, "ConnectedVertexBasedTile")
        {
            connections ??= new List<string>() { "", "", "", "" };

            if (view == null)
            {
                view = DirectoryTools.GetAssetByName<VisualTreeAsset>("ConnectedVertexBasedTile");
            }
            view.CloneTree(this);

            upperRightFill = this.Q<VisualElement>("UpperRight");
            upperLeftFill   = this.Q<VisualElement>("UpperLeft");
            lowerLeftFill   = this.Q<VisualElement>("LowerLeft");
            lowerRightFill  = this.Q<VisualElement>("LowerRight");

            center = this.Q<VisualElement>("CenterFill");

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
                SetBackgroundColor(upperRightFill, BrightenColor(color));

                if (!ConnectionColors.TryAdd(color, 1)) ConnectionColors[color]++;
            }
            else
            {
                SetBackgroundColor(upperRightFill, invalidColor);
            }

            if (!string.IsNullOrEmpty(tags[1]))
            {
                color = tts.Find(t => t.Label.Equals(tags[1])).Color;
                SetBackgroundColor(upperLeftFill, BrightenColor(color));

                if (!ConnectionColors.TryAdd(color, 1)) ConnectionColors[color]++;
            }
            else
            {
                SetBackgroundColor(upperLeftFill, invalidColor);
            }

            if (!string.IsNullOrEmpty(tags[2]))
            {
                color = tts.Find(t => t.Label.Equals(tags[2])).Color;
                SetBackgroundColor(lowerLeftFill, BrightenColor(color));

                if (!ConnectionColors.TryAdd(color, 1)) ConnectionColors[color]++;
            }
            else
            {
                SetBackgroundColor(lowerLeftFill, invalidColor);
            }

            if (!string.IsNullOrEmpty(tags[3]))
            {
                color = tts.Find(t => t.Label.Equals(tags[3])).Color;
                SetBackgroundColor(lowerRightFill, BrightenColor(color));

                if (!ConnectionColors.TryAdd(color, 1)) ConnectionColors[color]++;
            }
            else
            {
                SetBackgroundColor(lowerRightFill, invalidColor);
            }

            // paints center if there are connections and to the most connections
            if (ConnectionColors.Count > 0)
            {
                var orderedConnectionColors = ConnectionColors
                    .OrderByDescending(kvp => kvp.Value)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                SetBackgroundColor(center, orderedConnectionColors.First().Key);
            }
            else
            {
                SetBackgroundColor(center, invalidColor);
            }
        }

        //public override void SetTileCenter(LBSTag identifier)
        //{
        //    var color = identifier.Color;
        //    SetBackgroundColor(center, color);
        //    SetImageTint(bottomSide, BrightenColor(color));
        //    SetImageTint(topSide, BrightenColor(color));
        //    SetImageTint(leftSide, BrightenColor(color));
        //    SetImageTint(rightSide, BrightenColor(color));
        //}
    }
}