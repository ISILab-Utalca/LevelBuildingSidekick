using System;
using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISI_Lab.LBS.Plugin.MapTools.Editor
{
    [CustomEditor(typeof(LBSGenerated))]   
    public class LBSGeneratedEditor : UnityEditor.Editor
    {
        private LBSGeneratedMargin _margin;
        public override VisualElement CreateInspectorGUI()
        {
            LBSGenerated targetLbs = (LBSGenerated)target;
            
            _margin = new LBSGeneratedMargin();
            _margin.SetLBSRef(targetLbs);
            return _margin;
        }
    }
}
