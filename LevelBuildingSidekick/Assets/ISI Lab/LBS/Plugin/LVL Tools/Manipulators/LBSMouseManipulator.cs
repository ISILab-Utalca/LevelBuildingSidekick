using LBS.ElementView;
using LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Manipulators
{
    public abstract class LBSMouseManipulator : MouseManipulator
    {
        private IRepController controller;
        private MainView view;

        public LBSMouseManipulator(MainView view, IRepController controller)
        {
            this.controller = controller;
            this.view = view;
        }
    }
}