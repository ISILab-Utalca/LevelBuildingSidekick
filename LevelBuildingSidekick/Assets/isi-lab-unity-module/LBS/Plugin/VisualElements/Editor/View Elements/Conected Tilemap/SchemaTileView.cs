using ISILab.Commons.Utility.Editor;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;

namespace ISILab.LBS.VisualElements
{
    public class SchemaTileView : GraphElement
    {
        #region VIEW FIELDS
        private static VisualTreeAsset view;

        private const float borderThickness = 2f;
        private const float backgroundOpacity = 0.2f;
        
        private VisualElement left;
        private VisualElement right;
        private VisualElement top;
        private VisualElement bottom;
        private VisualElement border;
        #endregion

        SchemaTileConnectionView leftConnectionView;
        SchemaTileConnectionView rightConnectionView;
        SchemaTileConnectionView topConnectionView;
        SchemaTileConnectionView bottomConnectionView;

        public SchemaTileView()
        {
            if (view == null)
            {
                view = DirectoryTools.GetAssetByName<VisualTreeAsset>("SchemaTileView");
            }
            view.CloneTree(this);

            left = this.Q<VisualElement>("Left");
            right = this.Q<VisualElement>("Right");
            top = this.Q<VisualElement>("Top");
            bottom = this.Q<VisualElement>("Bottom");
            border = this.Q<VisualElement>("Border");

            this.SetMargins(0);
            this.SetPaddings(0);
            this.SetBorderRadius(0);
            this.SetBorder(Color.black, 1);
        }

        public void SetBackgroundColor(Color color)
        {
            color.a = backgroundOpacity;
            border.style.backgroundColor = color;
            right.style.backgroundColor = color;
            top.style.backgroundColor = color;
            left.style.backgroundColor = color;
            bottom.style.backgroundColor = color;
        }

        public void SetBorderColor(Color color, float thickness)
        {
            border.style.borderRightColor = color;
            border.style.borderLeftColor = color;
            border.style.borderTopColor = color;
            border.style.borderBottomColor = color;
            this.SetBorder(color, thickness);
        }

        public void SetConnections(string[] tags)
        {
           // right.SetDisplay(tags[0].Equals("Door"));
           // top.SetDisplay(tags[1].Equals("Door"));
            //left.SetDisplay(tags[2].Equals("Door"));
            //bottom.SetDisplay(tags[3].Equals("Door"));
            right.SetDisplay(false);
            top.SetDisplay(false);
            left.SetDisplay(false);
            bottom.SetDisplay(false);
            
            border.style.borderRightWidth = tags[0].Equals("Empty") ? 0f : borderThickness;
            border.style.borderTopWidth = tags[1].Equals("Empty") ? 0f : borderThickness;
            border.style.borderLeftWidth = tags[2].Equals("Empty") ? 0f : borderThickness;
            border.style.borderBottomWidth = tags[3].Equals("Empty") ? 0f : borderThickness;
        }

        public void CreateConnectionView(VectorImage icon, Vector2 pos, string key)
        {
            var connectionView = new SchemaTileConnectionView(icon)
            {
                style =
                {
                    width = 64,
                    height = 64,
                    backgroundColor = Color.clear,
                    position = Position.Absolute,
                    left = pos.x-2.5f,
                    top = pos.y-2.5f
                }
            };

            switch (key)
            {
                case "right":
                    rightConnectionView = connectionView;
                    break;

                case "top":
                    topConnectionView = connectionView;
                    break;

                case "left":
                    leftConnectionView = connectionView;
                    break;

                case "bottom":
                    bottomConnectionView = connectionView;
                    break;

                default:
                    Debug.LogWarning("Wrong key type");
                    break;
            }

            Add(connectionView);
        }

        public void RemoveConnectionViews()
        {
            if (rightConnectionView     is not null) Remove(rightConnectionView);
            if (topConnectionView       is not null) Remove(topConnectionView);
            if (leftConnectionView      is not null) Remove(leftConnectionView);
            if (bottomConnectionView    is not null) Remove(bottomConnectionView);

            rightConnectionView = topConnectionView = leftConnectionView = bottomConnectionView = null;
        }

        /// <summary>
        /// returns a dictionary of the connection points, where:
        /// - key = direction (example: "up")
        /// - value = connection type (example: "door")
        ///
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static Dictionary<string,string> GetConnectionPoints(List<string> tags)
        {
            Dictionary<string, string> ConnectionPoints = new Dictionary<string, string>();

            if(tags.Count > 0)  ConnectionPoints.Add("right", tags[0]);
            if(tags.Count > 1)  ConnectionPoints.Add("top", tags[1]);
            if(tags.Count > 2) ConnectionPoints.Add("left", tags[2]);
            if(tags.Count > 3) ConnectionPoints.Add("bottom", tags[3]);

            return ConnectionPoints; 
        }


    }
}