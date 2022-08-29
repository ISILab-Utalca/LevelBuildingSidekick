using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.ElementView
{
    public class Tile : GraphElement
    {
        public new class UxmlFactory : UxmlFactory<Tile, VisualElement.UxmlTraits> { }

        public Tile()
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Tile");
            visualTree.CloneTree(this);

            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("TileW");
            this.styleSheets.Add(styleSheet);

            this.SetPadding(0);
            this.SetBorderWidth(0);
            
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
    }
}
