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
        private Dictionary<LBSLayer, List<GraphElement>> pairs = new();

        public void AddElement(LBSLayer layer, GraphElement element)
        {
            if (!pairs.TryGetValue(layer, out var list))
            {
                list = new List<GraphElement>();
                pairs[layer] = list;
            }
            list.Add(element);
        }

        public List<GraphElement> GetElements()
        {
            List<GraphElement> elements = new List<GraphElement>();
            foreach (List<GraphElement> list in pairs.Values)
            {
                foreach (GraphElement element in list)
                {
                    elements.Add(element); 
                }
            }
            return elements;
        }
    }

    [UxmlElement]
    public partial class MainView : GraphView // Canvas or WorkSpace
    {
        #region UXML_FACTORY
       // public new class UxmlFactory : UxmlFactory<MainView, UxmlTraits> { }
        #endregion

        #region SINGLETON
        private static MainView instance;
        public static MainView Instance
        {
            get => instance;
        }

        public Vector2 FixPos(Vector2 v)
        {
            var t = new Vector2(viewTransform.position.x, viewTransform.position.y);
            var newPos = (v - t) / scale;
            return newPos;
        }
        #endregion

        #region FIELDS
        private ExternalBounds bound;
        private List<Manipulator> manipulators = new();

        private LayerContainer defaultLayer = new();
        private Dictionary<LBSLayer, LayerContainer> layers = new();
        
        // shared manipulators such as drag, zoom
        private List<Manipulator> defaultManipulators = new();
        private ContentZoomer zoomer;
        private ContentDragger cDragger;
        private SelectionDragger sDragger;
        private bool zoomEnabled = true;

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
            bound = new ExternalBounds(
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

        public void SetBasicManipulators()
        {
            var setting = LBSSettings.Instance.general;

            zoomer = new ContentZoomer();

            setting.OnChangeZoomValue = (min, max) =>
            {
                zoomer.maxScale = setting.zoomMax;
                zoomer.minScale = setting.zoomMin;
            };

            zoomer.maxScale = setting.zoomMax;
            zoomer.minScale = setting.zoomMin;

            RegisterCallback<WheelEvent>(evt => { zoomer.target = !zoomEnabled ? null : this; });
            
            cDragger = new ContentDragger();
            sDragger = new SelectionDragger();

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
            AddManipulator(current);
        }

        public void SetManipulators(List<Manipulator> manipulators)
        {
            ClearManipulators();
            AddManipulators(manipulators);
        }
        
        public void ClearManipulators()
        {
            foreach (var m in manipulators)
            {
                this.RemoveManipulator(m as IManipulator);
            }
            manipulators.Clear();
        }

        public void RemoveManipulator(Manipulator manipulator)
        {
            manipulators.Remove(manipulator);
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
            manipulators.Add(manipulator);
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

        public void SetManipulatorZoom(bool enable)
        {
            zoomEnabled = enable;
        }

        #endregion

        #region METHODS_VIEW

        public void ClearLevelView()
        {
            graphElements.ForEach(e => RemoveElement(e));
            new List<LayerContainer>(layers.Values).ForEach(l => l.GetElements(this));
            defaultLayer.GetElements(this);
            AddElement(bound);
        }

        public void RemoveLayer(LBSLayer layer)
        {
            if (!layers.TryGetValue(layer, out var container)) return;

            // Remove elements from the view
            container.GetElements(this); // Returns List<List<GraphElement>>
           // layers.Remove(layer);
        }
        
        public void ClearLayerView(LBSLayer layer)
        {
            if (!layers.Keys.Any() || !layers.TryGetValue(layer, out var l))  return;
            l.GetElements(this);
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

            element.layer = layer.index;
            
            container.AddElement(layer, element);
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