using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public class PresedBtnOld : Button, IGrupableOld
    {
        public new class UxmlFactory : UxmlFactory<PresedBtnOld, UxmlTraits> { }

        public Color selected = new Color(0.35f, 0.35f, 0.35f, 1f);
        public Color unselected = new Color(0.27f, 0.38f, 0.49f, 1f);
        public Texture2D Icon;

        public void AddEvent(Action action)
        {
            this.clicked += action;
        }

        public void SetActive(bool value)
        {
            this.style.backgroundColor = !value ? selected : unselected;
        }
    }
}