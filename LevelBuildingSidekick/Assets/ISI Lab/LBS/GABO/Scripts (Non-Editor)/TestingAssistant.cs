using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.Assistants
{
    public class TestingAssistant : LBSAssistant
    {
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
    }
}