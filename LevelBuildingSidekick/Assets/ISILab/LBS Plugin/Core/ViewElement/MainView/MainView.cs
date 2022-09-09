using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.ElementView
{
    public class MainView : LBSGraphView
    {
        public Action<ContextualMenuPopulateEvent> OnBuild;

        //manipulators
        internal ContentZoomer zoomer = new ContentZoomer();
        internal ContentDragger dragger = new ContentDragger();
        internal SelectionDragger selectionDragger = new SelectionDragger();
        internal RectangleSelector rectagleSelector = new RectangleSelector();

        private List<Manipulator> manipulators = new List<Manipulator>();

        public Action OnClearSelection;

        public new class UxmlFactory : UxmlFactory<MainView, GraphView.UxmlTraits> { }

        public MainView()
        {
            Insert(0,new GridBackground());
            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("MainViewUSS");
            styleSheets.Add(styleSheet);

            SetBasicManipulators();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            OnBuild?.Invoke(evt);
        }

        public void SetBasicManipulators()
        {
            var manis = new List<Manipulator>() { zoomer, dragger, selectionDragger, rectagleSelector };
            SetManipulators(manis);
        }

        public void SetManipulator(Manipulator current)
        {
            RemoveManipulators(manipulators);
            this.AddManipulator(current);
        }

        public void SetManipulators(List<Manipulator> manipulators)
        {
            ClearManipulators();
            AddManipulators(manipulators);
        }

        public override void ClearSelection()
        {
            base.ClearSelection();
            if (selection.Count == 0)
            {
                OnClearSelection?.Invoke();
                //LBSController.ShowLevelInspector();
            }
        }

        public void ClearManipulators()
        {
            foreach (var m in this.manipulators)
            {
                this.RemoveManipulator(m);
            }
            this.manipulators.Clear();
        }

        public void RemoveManipulators(List<Manipulator> manipulators)
        {
            foreach (var m in manipulators)
            {
                this.manipulators.Remove(m);
                this.RemoveManipulator(m);
            }
        }

        public void AddManipulators(List<Manipulator> manipulators)
        {
            foreach (var m in manipulators)
            {
                if (!this.manipulators.Contains(m))
                {
                    this.manipulators.Add(m);
                    this.AddManipulator(m);
                }
            }
        }

    }
}
