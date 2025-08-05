using LBS.Components;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Assistants
{
    public class TestingAssistant : LBSAssistant
    {
        private PathOSWindow pathOSOriginalWindow;

        public System.Action OnDetach;

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
            OnDetach?.Invoke();
            Object.DestroyImmediate(pathOSOriginalWindow);
        }

        public override bool Equals(object obj)
        {
            if(obj is not TestingAssistant other) return false;

            if(!Equals(Name, other.Name)) return false;

            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}