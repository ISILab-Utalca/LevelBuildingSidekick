using ISILab.Commons.Utility.Editor;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Bundles;
using LBS.Bundles.Tools;
using UnityEditor.Experimental.GraphView;

namespace LBS.VisualElements
{
    public class MarginGraph : GraphElement
    {
        private static VisualTreeAsset visualTree;
        
        private MicroGenTool.Margin _marginRef;

        private FloatField _positiveXField;
        private FloatField _positiveZField;
        private FloatField _negativeXField;
        private FloatField _negativeZField;

        public MarginGraph()
        {   
            visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("MarginGraph");;
            visualTree.CloneTree(this);

            _positiveXField = this.Q<FloatField>("positiveX");
            _positiveXField.RegisterValueChangedCallback(evt => OnPositiveXChanged(evt.newValue));
            _positiveZField = this.Q<FloatField>("positiveZ");
            _positiveZField.RegisterValueChangedCallback(evt => OnPositiveZChanged(evt.newValue));
            _negativeXField = this.Q<FloatField>("negativeX");
            _negativeXField.RegisterValueChangedCallback(evt => OnNegativeXChanged(evt.newValue));
            _negativeZField = this.Q<FloatField>("negativeY");
            _negativeZField.RegisterValueChangedCallback(evt => OnNegativeZChanged(evt.newValue));
        }
        
        
        //Set all fields according to reference 
        public void SetMarginRef(MicroGenTool.Margin margin)
        {
            _marginRef = margin;
            
            _positiveXField.value = margin.positiveX;
            _positiveZField.value = margin.positiveZ;
            _negativeXField.value = margin.negativeX;
            _negativeZField.value = margin.negativeZ;

            MarkDirtyRepaint();
        }

        #region FIELD CHANGED METHODS
        private void OnPositiveXChanged(float evtNewValue)
        {
            //Out of range (the changes will call the function again)
            if (evtNewValue > 1)
            {
                _positiveXField.value = 1;
                return;
            }
            
            if (evtNewValue < 0)
            {
                _positiveXField.value = 0;
                return;
            }
            
            //Apply changes in reference
            _marginRef.positiveX = evtNewValue;
        }
        private void OnNegativeXChanged(float evtNewValue)
        {
            //Out of range (the changes will call the function again)
            if (evtNewValue > 1)
            {
                _negativeXField.value = 1;
                return;
            }
            
            if (evtNewValue < 0)
            {
                _negativeXField.value = 0;
                return;
            }
            
            //Apply changes in reference
            _marginRef.negativeX = evtNewValue;
        }
        private void OnPositiveZChanged(float evtNewValue)
        {
            //Out of range (the changes will call the function again)
            if (evtNewValue > 1)
            {
                _positiveZField.value = 1;
                return;
            }
            
            if (evtNewValue < 0)
            {
                _positiveZField.value = 0;
                return;
            }
            
            //Apply changes in reference
            _marginRef.positiveZ = evtNewValue;
        }
        private void OnNegativeZChanged(float evtNewValue)
        {
            //Out of range (the changes will call the function again)
            if (evtNewValue > 1)
            {
                _negativeZField.value = 1;
                return;
            }
            
            if (evtNewValue < 0)
            {
                _negativeZField.value = 0;
                return;
            }
            
            //Apply changes in reference
            _marginRef.negativeZ = evtNewValue;
        }
        #endregion
        

    }   
}
