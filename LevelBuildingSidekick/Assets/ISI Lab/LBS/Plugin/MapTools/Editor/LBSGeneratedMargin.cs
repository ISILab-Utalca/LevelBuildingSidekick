using System;
using System.Collections.Generic;
using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using ISILab.Extensions;
using ISILab.LBS.Characteristics;
using ISILab.LBS.VisualElements;
using LBS.Bundles;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


namespace ISI_Lab.LBS.Plugin.MapTools.Editor
{
    [UxmlElement]
    public partial class LBSGeneratedMargin : VisualElement
    {
        private ObjectField _bundleRef;
        private ObjectField _bundleTemp;
        private EnumField _spreadType;

        private VisualElement _marginGraph;

        private LBSGenerated lbsRef;

        public LBSGeneratedMargin()
        {
            CreateVisualElement();
        }
        
        private VisualElement CreateVisualElement()
        {
            var visualTree = Resources.Load("LBSGeneratedMargin") as VisualTreeAsset;
            visualTree.CloneTree(this);
            
            _bundleRef = this.Q<ObjectField>("BundleRef");
            _bundleTemp = this.Q<ObjectField>("BundleTemp");
            _spreadType = this.Q<EnumField>("SpreadType");
            _spreadType.RegisterValueChangedCallback(evt => OnSpreadChanged(evt.newValue));
            
            _marginGraph = this.Q<VisualElement>("MarginGraph");
            
            return this;
        }
        
        //Set all fields according to reference 
        public void SetLBSRef(LBSGenerated LBSGen)
        {
            lbsRef = LBSGen;
            
            _bundleRef.value = LBSGen.BundleRef;
            _bundleTemp.value = LBSGen.BundleTemp;
            _spreadType.value = LBSGen.Spread;

            MarkDirtyRepaint();
        }

        public void SetGraphDisplay(bool display)
        {
            _marginGraph.SetDisplay(display);
        }

        private void OnSpreadChanged(Enum evtNewValue)
        {
            lbsRef.Spread = (LBSGenerated.SpreadType)evtNewValue;
        }

    }   
}
