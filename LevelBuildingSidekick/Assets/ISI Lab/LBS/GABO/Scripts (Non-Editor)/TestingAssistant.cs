using LBS.Components;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Assistants
{
    public class TestingAssistant : LBSAssistant
    {
        private PathOSWindow pathOSOriginalWindow;

        public PathOSWindow PathOSOriginalWindow { get => pathOSOriginalWindow; set => pathOSOriginalWindow = value; }

        public TestingAssistant(VectorImage icon, string name, Color colorTint) : base(icon, name, colorTint)
        {
        }

        public override object Clone()
        {
            return new TestingAssistant(Icon, Name, ColorTint);
        }

        public override void OnGUI()
        {
            
        }

        public override void OnDetachLayer(LBSLayer layer)
        {
            base.OnDetachLayer(layer);
            Object.DestroyImmediate(pathOSOriginalWindow);
        }
    }
}