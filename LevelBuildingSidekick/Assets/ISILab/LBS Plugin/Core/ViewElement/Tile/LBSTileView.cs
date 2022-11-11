using LBS.Representation;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.ElementView
{
    public class LBSTileView : LBSGraphElement
    {
       
        //public new class UxmlFactory : UxmlFactory<LBSTileView, VisualElement.UxmlTraits> { }

        public TileData Data;

        private int wallthicc = 8;
        private VisualElement border;
        private Label label;

        // Doors
        public VisualElement left;
        public VisualElement right;
        public VisualElement top;
        public VisualElement bottom;

        public LBSTileView(TileData tile, LBSGraphView root) : base(root)
        {
            Data = tile;

            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Tile");
            visualTree.CloneTree(this);

            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("TileW");
            this.styleSheets.Add(styleSheet);

            border = this.Q<VisualElement>("Border");
            label = this.Q<Label>();

            left = this.Q<VisualElement>(name: "DoorLeft");
            right = this.Q<VisualElement>(name: "DoorRight");
            top = this.Q<VisualElement>(name: "DoorTop");
            bottom = this.Q<VisualElement>(name: "DoorBottom");

            this.SetPadding(0);
            this.SetBorderWidth(0);
            HideAllDoors();

            capabilities |= Capabilities.Selectable | Capabilities.Ascendable | Capabilities.Copiable | Capabilities.Snappable | Capabilities.Groupable;
            usageHints = UsageHints.DynamicTransform;
        }


        public void ShowDir(Vector2Int dir)
        {
            var angle = Vector2.SignedAngle(Vector2.right, dir) % 360;
            if (angle < 0)
                angle = 360 + angle;

            if (angle < 45 || angle > 315)
            {
                right.visible = true;
            }
            else if (angle > 45 && angle <= 135)
            {
                bottom.visible = true;
            }
            else if (angle > 135 && angle <= 225)
            {
                left.visible = true;
            }
            else if (angle > 225 && angle <= 315)
            {
                top.visible = true;
            }
        }

        public void SetDoorColor(Color color)
        {
            left.style.backgroundColor = color;
            right.style.backgroundColor = color;
            top.style.backgroundColor = color;
            bottom.style.backgroundColor = color;
        }

        public void HideAllDoors()
        {
            left.visible = false;
            right.visible = false;
            top.visible = false;
            bottom.visible = false;
        }

        public void ShowLabel(bool value)
        {
            label.visible = value;
        }

        public void SetLabel(string value)
        {
            label.text = value;
        }

        public void SetLabel(Vector2 value)
        {
            SetLabel("( " + value.x + " x " + value.y + " )");
        }

        public void SetColor(Color color)
        {
            this.style.backgroundColor = color;
            SetDoorColor(color);
        }

        public void SetWalls(bool value,WallDirection[] dirs)
        {
            foreach (var dir in dirs)
            {
                switch(dir)
                {
                    case WallDirection.Top:
                        border.style.borderTopWidth = (value) ? wallthicc : 0;
                        break;
                    case WallDirection.Bottom:
                        border.style.borderBottomWidth = (value) ? wallthicc : 0;
                        break;
                    case WallDirection.Left:
                        border.style.borderLeftWidth = (value) ? wallthicc : 0;
                        break;
                    case WallDirection.Rigth:
                        border.style.borderRightWidth = (value) ? wallthicc : 0;
                        break;
                }
            }
        }

        private void SetPadding(int value)
        {
            this.style.paddingBottom = this.style.paddingLeft = this.style.paddingRight = this.style.paddingTop = value;
        }

        private void SetBorderWidth(int value)
        {
            this.style.borderBottomWidth = this.style.borderRightWidth = this.style.borderLeftWidth = this.style.borderTopWidth = value;
        }

        public void SetSize(int size)
        {
            SetSize(size, size);
        }

        public void SetSize(int x, int y)
        {
            this.style.width = this.style.maxWidth = this.style.minWidth = x;
            this.style.height = this.style.maxHeight = this.style.minHeight = y;
        }

        public override void OnDelete()
        {
            throw new System.NotImplementedException();
        }

        public enum WallDirection
        {
            Top,
            Bottom,
            Left,
            Rigth,
        }
    }
}
