using ISILab.Commons.Utility.Editor;
using LBS.Components;
using ISILab.LBS.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.VisualElements.Editor
{
    public class LayerContainer
    {
        private readonly Dictionary<object, List<GraphElement>> _pairs = new();

        /// <summary>
        /// Adds a graphElement into the container under the drawer's list. 
        /// </summary>
        /// <param name="obj">Drawer, or other element, from which the element was created. If already present in dictionary, other graphElements will be added to its list.</param>
        /// <param name="element">Graph element to be added under the key's list.</param>
        public void AddElement(object obj, GraphElement element)
        {
            if (!_pairs.TryGetValue(obj, out var list))
            {
                list = new List<GraphElement>();
                _pairs[obj] = list;
            }
            
            list.Add(element);
        }
        
        public List<GraphElement> GetElement(object obj)
        {
            return _pairs.GetValueOrDefault(obj);
        }
        
        public List<GraphElement> ClearElement(object obj)
        {
            return _pairs.Remove(obj, out var list) ? list : null;
        }
        public void Repaint(object obj)
        {
            if (!_pairs.TryGetValue(obj, out var elements)) return;
            foreach (var element in elements)
            {
                element.MarkDirtyRepaint();
            }
        }

        public List<GraphElement> Clear()
        {
            var erasedElements = new List<GraphElement>();
            for (int i = 0; i < _pairs.Count; i++)
            {
                var list = _pairs.ElementAt(i).Value;
                for (int j = 0; j < list.Count; j++)
                {
                    var graph = list[j];
                    
                    erasedElements.Add(graph);
                    list.RemoveAt(j);
                    j--;
                }
                
                // If list empty, remove item from dictionary
                if (list.Count > 0) continue;
                _pairs.Remove(_pairs.ElementAt(i).Key);
                i--;
            }
            return erasedElements;
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
        public static MainView Instance => instance;

        public Vector2 FixPos(Vector2 v)
        {
            var t = new Vector2(this.viewTransform.position.x, this.viewTransform.position.y);
            var newPos = (v - t) / this.scale;
            return newPos;
        }
        #endregion

        #region FIELDS
        private ExternalBounds _bound;
        private readonly List<Manipulator> _manipulators = new();

        private readonly LayerContainer _defaultLayer = new();
        private readonly Dictionary<LBSLayer, LayerContainer> _layers = new();
        
        // shared manipulators such as drag, zoom
        private List<Manipulator> _defaultManipulators = new();
        private ContentZoomer _zoomer;
        private ContentDragger _cDragger;
        private SelectionDragger _sDragger;
        private bool _zoomEnabled = true;

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

            AddElement(_bound);

            // Singleton
            if (instance != this) instance = this;
                
            
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
            this._bound = new ExternalBounds(
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

        private void SetBasicManipulators()
        {
            var setting = LBSSettings.Instance.general;

            _zoomer = new ContentZoomer();

            setting.OnChangeZoomValue = (min, max) =>
            {
                _zoomer.maxScale = setting.zoomMax;
                _zoomer.minScale = setting.zoomMin;
            };

            _zoomer.maxScale = setting.zoomMax;
            _zoomer.minScale = setting.zoomMin;

            RegisterCallback<WheelEvent>(evt => { _zoomer.target = !_zoomEnabled ? null : this; });
            
            _cDragger = new ContentDragger();
            _sDragger = new SelectionDragger();

            var manis = new List<Manipulator>() { _zoomer, _cDragger, _sDragger };
            _defaultManipulators = manis;
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
            if(keepDefaults) AddManipulators(_defaultManipulators);
            this.AddManipulator(current);
        }

        private void SetManipulators(List<Manipulator> manipulators)
        {
            ClearManipulators();
            AddManipulators(manipulators);
        }

        private void ClearManipulators()
        {
            foreach (var m in this._manipulators)
            {
                this.RemoveManipulator(m as IManipulator);
            }
            this._manipulators.Clear();
        }

        public void RemoveManipulator(Manipulator manipulator)
        {
            this._manipulators.Remove(manipulator);
            this.RemoveManipulator(manipulator as IManipulator);
        }

        public void RemoveManipulators(List<Manipulator> manipulators)
        {
            foreach (var m in manipulators)
            {
                this._manipulators.Remove(m);
                this.RemoveManipulator(m as IManipulator);
            }
        }

        public void AddManipulator(Manipulator manipulator)
        {
            if (_manipulators.Contains(manipulator)) return;
            this._manipulators.Add(manipulator);
            this.AddManipulator(manipulator as IManipulator);
        }

        private void AddManipulators(List<Manipulator> manipulators)
        {
            foreach (var m in manipulators)
            {
                if (!this._manipulators.Contains(m))
                {
                    this._manipulators.Add(m);
                    this.AddManipulator(m as IManipulator);
                }
            }
        }
        
        public bool HasManipulator<T>() where T : Manipulator
        {
            return _manipulators.Any(m => m is T);
        }

        public void SetManipulatorZoom(bool enable)
        {
            _zoomEnabled = enable;
        }

        #endregion

        #region METHODS_VIEW

        public void ClearLevelView()
        {
            graphElements.ForEach(RemoveElement);
            new List<LayerContainer>(_layers.Values).ForEach(l => l.Clear());
            _defaultLayer.Clear();
            AddElement(_bound);
        }

        public void ClearLayerView(LBSLayer layer, bool deepClean = false)
        {
            if (!_layers.TryGetValue(layer, out var container)) return;
            
            // Remove all elements
            if (deepClean)
            {
                foreach (var element in container.Clear())
                {
                    RemoveElement(element);
                }
            }
            
            // Remove expired tiles in behaviours
            foreach (var tile in layer.Behaviours.SelectMany(behaviour => behaviour.RetrieveExpiredTiles()))
            {
                ClearElementView(tile, container);
            }
            // Remove expired tiles in assistants
            foreach (var assistant in layer.Assistants)
            {
                foreach (var tile in assistant.RetrieveExpiredTiles())
                {
                    ClearElementView(tile, container);
                }
            }
        }

        private void ClearElementView(object tile, LayerContainer container)
        {
            if (tile == null) return;
            var gElements = container.ClearElement(tile);
            if(gElements == null) return;

            foreach (var g in gElements)
            {
                RemoveElement(g);
            }
        }
        
        public void ClearLayerComponentView(LBSLayer layer, object component)
        {
            if (!_layers.TryGetValue(layer, out var container)) return;
            if(component is null) return;
            
            var elements = container.GetElement(component);
            if(elements is null || !elements.Any()) return;
            
            foreach (var element in elements)
            {
                RemoveElement(element);
            }
        }
        
        public void AddContainer(LBSLayer layer)
        {
            _layers.Add(layer, new LayerContainer());
        }

        public void RemoveContainer(LBSLayer layer)
        {
            ClearLayerView(layer, true);
            _layers.Remove(layer);
        }

        /// <summary>
        /// Saves a graphElement into a LayerContainer.
        /// </summary>
        /// <param name="layer">Layer where the graph element will be put. An LBSLayer does not hold graphElements, but it will create or get a LayerContainer object.</param>
        /// <param name="obj">The drawer from where the graphElement is created.</param>
        /// <param name="element">The graphElement to draw in screen.</param>
        public void AddElement(LBSLayer layer, object obj, GraphElement element)
        {
            var container = GetOrCreateLayerContainer(layer);
            if(container == null) return;
            element.layer = layer.index;

            container.AddElement(obj, element);
            base.AddElement(element);
        }

        public List<GraphElement> GetElements(LBSLayer layer, object key)
        {
            return _layers.TryGetValue(layer, out LayerContainer container) ? container.GetElement(key) : null;
        }

        private LayerContainer GetOrCreateLayerContainer(LBSLayer layer)
        {
            _layers.TryGetValue(layer, out var container);
            return container;
        }
        
        #endregion

 
    }
}