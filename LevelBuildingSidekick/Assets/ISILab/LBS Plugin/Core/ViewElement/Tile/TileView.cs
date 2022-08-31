using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.ElementView
{
    public class TileView : GraphElement
    {
        //public new class UxmlFactory : UxmlFactory<TileView, VisualElement.UxmlTraits> { }

        private int wallthicc = 8;
        private VisualElement border;
        private Label label;

        public TileView()
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Tile");
            visualTree.CloneTree(this);

            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("TileW");
            this.styleSheets.Add(styleSheet);

            border = this.Q<VisualElement>("Border");
            label = this.Q<Label>();

            this.SetPadding(0);
            this.SetBorderWidth(0);

            capabilities |= Capabilities.Selectable | Capabilities.Deletable | Capabilities.Ascendable | Capabilities.Copiable | Capabilities.Snappable | Capabilities.Groupable;
            usageHints = UsageHints.DynamicTransform;
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

        public enum WallDirection
        {
            Top,
            Bottom,
            Left,
            Rigth,
        }
    }
}
