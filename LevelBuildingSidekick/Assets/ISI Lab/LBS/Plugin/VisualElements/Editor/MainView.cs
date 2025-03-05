using ISILab.Commons.Utility.Editor;
using LBS.Components;
using ISILab.LBS.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.VisualElements.Editor
{
    public class LayerContainer
    {
        private Dictionary<object, GraphElement> pairs = new();

        public void AddElement(object obj, GraphElement element)
        {
            pairs.Add(obj, element);
        }

        public void Actualize(object obj, object other)
        {
            
        }

        public void Repaint(object obj)
        {
            GraphElement ve = pairs[obj];
            ve.MarkDirtyRepaint();
        }

        public List<GraphElement> Clear()
        {
            var elements = new List<GraphElement>(pairs.Values);
            pairs.Clear();
            return elements;
        }
    }

    public class MainView : GraphView // Canvas or WorkSpace
    {
        #region UXML_FACTORY
        public new class UxmlFactory : UxmlFactory<MainView, UxmlTraits> { }
        #endregion

        #region SINGLETON
        private static MainView instance;
        public static MainView Instance
        {
            get => instance;
        }

        public Vector2 FixPos(Vector2 v)
        {
            var t = new Vector2(this.viewTransform.position.x, this.viewTransform.position.y);
            var newPos = (v - t) / this.scale;
            return newPos;
        }
        #endregion

        #region FIELDS
        private ExternalBounds bound;
        private List<Manipulator> manipulators = new List<Manipulator>();

        private LayerContainer defaultLayer = new LayerContainer();
        private Dictionary<LBSLayer, LayerContainer> layers = new Dictionary<LBSLayer, LayerContainer>();
        // shared manipulators such as drag, zoom
        private List<Manipulator> defaultManipulators = new List<Manipulator>();
        #endregion

        #region EVENTS
        public event Action OnClearSelection;
        #endregion

        #region CONSTRUCTORS
        public MainView()
        {
            Insert(0, new GridBackground());
            var styleSheet = DirectoryTools.GetAssetByName<StyleSheet>("MainViewUSS");
            styleSheets.Add(styleSheet);
            style.flexGrow = 1;
            
            SetBasicManipulators();
            InitBound(20000, int.MaxValue / 2);

            AddElement(bound);

            // Singleton
            if (instance != this)
                instance = this;
            
            AddManipulator(new ContextualMenuManipulator((evt) =>
            {
                // Prevent the default right-click menu
              //  evt.StopPropagation();
                evt.menu.ClearItems(); 
                
            }));

        }
        
        #endregion

        
        #region INTERNAL_METHODS
        private void InitBound(int interior, int exterior)
        {
            this.bound = new ExternalBounds(
                new Rect(
                    new Vector2(-interior, -interior),
                    new Vector2(interior * 2, interior * 2)
                    ),
                new Rect(
                    new Vector2(-exterior, -exterior),
                    new Vector2(exterior * 2, exterior * 2)
                    )
                );
        }
        #endregion

        #region METHODS_MANIPULATORS

        public void SetBasicManipulators() // (?) necesario aqui 
        {
            var setting = LBSSettings.Instance.general;

            var zoomer = new ContentZoomer();

            setting.OnChangeZoomValue = (min, max) =>
            {
                zoomer.maxScale = setting.zoomMax;
                zoomer.minScale = setting.zoomMin;
            };

            zoomer.maxScale = setting.zoomMax;
            zoomer.minScale = setting.zoomMin;

            var cDragger = new ContentDragger();
            var sDragger = new SelectionDragger();

            var manis = new List<Manipulator>() { zoomer, cDragger, sDragger };
            defaultManipulators = manis;
            SetManipulators(manis);
        }

        public override void ClearSelection() // (?)
        {
            base.ClearSelection();
            if (selection.Count == 0)
            {
                OnClearSelection?.Invoke();
            }
        }

        public void SetManipulator(Manipulator current, bool keepDefaults = false)
        {
            ClearManipulators();
            if(keepDefaults) AddManipulators(defaultManipulators);
            this.AddManipulator(current);
        }

        public void SetManipulators(List<Manipulator> manipulators)
        {
            ClearManipulators();
            AddManipulators(manipulators);
        }
        
        public void ClearManipulators()
        {
            foreach (var m in this.manipulators)
            {
                this.RemoveManipulator(m as IManipulator);
            }
            this.manipulators.Clear();
        }

        public void RemoveManipulator(Manipulator manipulator)
        {
            this.manipulators.Remove(manipulator);
            this.RemoveManipulator(manipulator as IManipulator);
        }

        public void RemoveManipulators(List<Manipulator> manipulators)
        {
            foreach (var m in manipulators)
            {
                this.manipulators.Remove(m);
                this.RemoveManipulator(m as IManipulator);
            }
        }

        public void AddManipulator(Manipulator manipulator)
        {
            if (manipulators.Contains(manipulator)) return;
            this.manipulators.Add(manipulator);
            this.AddManipulator(manipulator as IManipulator);
        }

        public void AddManipulators(List<Manipulator> manipulators)
        {
            foreach (var m in manipulators)
            {
                if (!this.manipulators.Contains(m))
                {
                    this.manipulators.Add(m);
                    this.AddManipulator(m as IManipulator);
                }
            }
        }
        
        public bool HasManipulator<T>() where T : Manipulator
        {
            return manipulators.Any(m => m is T);
        }



        #endregion

        #region METHODS_VIEW

        public void ClearLevelView()
        {
            this.graphElements.ForEach(e => this.RemoveElement(e));
            new List<LayerContainer>(this.layers.Values).ForEach(l => l.Clear());
            defaultLayer.Clear();
            AddElement(bound);
        }

        public void ClearLayerView(LBSLayer layer)
        {
            if (layers.Keys.Count <= 0)
                return;

            if (!layers.TryGetValue(layer, out LayerContainer container))
            {
                return;
            }

            var l = layers[layer];
            var elements = l.Clear();
            elements.ForEach(e => this.RemoveElement(e));
        }

        public void AddElement(object obj, GraphElement element)
        {
            defaultLayer.AddElement(obj, element);
            base.AddElement(element);
        }

        public void AddContainer(LBSLayer layer)
        {
            layers.Add(layer, new LayerContainer());
        }

        public void RemoveContainer(LBSLayer layer)
        {
            layers.Remove(layer);
        }

        public void AddElement(LBSLayer layer, object obj, GraphElement element)
        {
            LayerContainer container;
            layers.TryGetValue(layer, out container);

            if (container == null)
            {
                container = new LayerContainer();
                layers.Add(layer, container);
            }

            container.AddElement(obj, element);
            base.AddElement(element);
        }

        public LayerContainer GetLayerContainer(LBSLayer layer)
        {
            LayerContainer container;
            layers.TryGetValue(layer, out container);

            if (container == null)
            {
                container = new LayerContainer();
                layers.Add(layer, container);
            }

            return container;
        }
        
        #endregion

    }
}