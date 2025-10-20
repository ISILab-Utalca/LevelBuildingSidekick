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

namespace ISILab.LBS.VisualElements
{
    public class SchemaTileConnectionView : GraphElement
    {
        #region VIEW FIELDS
        private static VisualTreeAsset view;
        private VisualElement Icon;
        
        #endregion

        public SchemaTileConnectionView(VectorImage icon)
        {
            if (view == null)
            {
                view = DirectoryTools.GetAssetByName<VisualTreeAsset>("SchemaTileConnectionView");
            }
            view.CloneTree(this);

            Icon = this.Q<VisualElement>("Icon");
            if (icon == null)
            {
                style.display = DisplayStyle.None;
                return;
            }
            
            Icon.style.backgroundImage = new StyleBackground(icon);
        }

    }
}