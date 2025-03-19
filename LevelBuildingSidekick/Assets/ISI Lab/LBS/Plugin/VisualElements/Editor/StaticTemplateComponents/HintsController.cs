using ISILab.LBS.Editor.Windows;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using UnityEditor;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.AI.Categorization;
using ISILab.LBS.Generators;
using UnityEditor.UIElements;
using Object = UnityEngine.Object;

namespace ISILab.LBS.VisualElements.Editor
{
    public class HintsController : VisualElement
    {
        #region UXMLFACTORY
        [UxmlElementAttribute]
        public new class UxmlFactory { }
        #endregion

        #region VIEW ELEMENTS
        private VisualElement tab1;
        private VisualElement tab2;
        private VisualElement tab3;
        private VisualElement tab4;
        
        private Button buttonTab1;
        private Button buttonTab2;
        
        private Button switch1;
        private Button switch2;
        private Button switch3;
        private Button switch4;
        
        #endregion

        #region FIELDS
    
        Dictionary<Button, VisualElement> tabs = new();
        
        #endregion

        #region PROPERTIES
        
        #endregion

        #region CONSTRUCTORS
        public HintsController()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("HintsController");
            visualTree.CloneTree(this);
            
            
        }
        
        #endregion

        #region METHODS

        // should pass the preset as parameter
        private void ChangeTab(int index)
        {
            
        }
        
        #endregion
       
    }
}