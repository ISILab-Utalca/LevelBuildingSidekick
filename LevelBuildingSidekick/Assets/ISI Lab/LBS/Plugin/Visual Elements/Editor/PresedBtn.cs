using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{

    public class PresedBtn : Button, IGrupable
    {
        public new class UxmlFactory : UxmlFactory<PresedBtn, UxmlTraits> { }

        public Color selected = new Color(0.35f, 0.35f, 0.35f, 1f);
        public Color unselected = new Color(0.27f, 0.38f, 0.49f, 1f);
        public Texture2D Icon;

        public void AddEvent(Action action)
        {
            this.clicked += action;
        }

        void IGrupable.OnBlur() 
        {
            this.style.backgroundColor = unselected;
        }

        void IGrupable.OnFocus()
        {
            this.style.backgroundColor = selected;
        }
    }
}