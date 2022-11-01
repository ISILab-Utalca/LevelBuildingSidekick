using LBS.ElementView;
using LBS.VisualElements;
using LBS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Overlays;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;
using UnityEngine.Networking;

namespace LBS.Windows
{
    public abstract class GenericGraphWindow : EditorWindow, ISupportsOverlays
    {
        protected List<Tuple<string, Action>> actions = new List<Tuple<string, Action>>();
        protected List<IRepController> controllers = new List<IRepController>();
        public IRepController CurrentController { get; protected set; }

        protected VisualElement root;
        protected MainView mainView;
        protected FloatingPanel panel;
        protected Type nextWindow, prevWindow;

        private Label label;

        private bool _createGui = false; //parche

        public MainView MainView => mainView;

        //public abstract void OnCreateGUI();

        public abstract void OnLoadControllers();

        public abstract void OnInitPanel();

        public T GetControllerByType<T>() where T : IRepController
        {
            var x = controllers.Find(c => c.GetType() == typeof(T));
            return (T)x;
        }

        public void AddController(IRepController controller)
        {
            controllers.Add(controller);
        }

        private void OnInspectorUpdate()
        {
            if (!_createGui)
                return;

            var fileInfo = LBSController.CurrentLevel.FileInfo;
            if (fileInfo != null)
            {
                label.text = "file*: ''" + fileInfo.Name + "''";
                label.style.color = new Color(0.6f, 0.6f, 0.6f);
            }
            else
            {
                label.text = "file: ''Unsaved''";
                label.style.color = Color.white;
            }

        }

        public void CreateGUI()
        {
            root = rootVisualElement;
            this.ImportUXML("GenericGraphWindowUXML");
            mainView = rootVisualElement.Q<MainView>();
            InitToolBar();

            RefreshView();
            _createGui = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnFocus()
        {
            var data = LBSController.CurrentLevel.data;
            controllers.ForEach(c => c.PopulateView(MainView));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SwithController(int value)
        {
            if (value < 0 || value >= controllers.Count)
            {
                Debug.LogWarning("Index <b>'" + value + "'</b> is out of bounds.");
            }
            CurrentController = controllers[value];
        }


        public void RefreshView()
        {
            foreach (var e in mainView.graphElements)
            {
                mainView.RemoveElement(e);
            }
            //mainView.graphElements.ForEach(e => mainView.RemoveElement(e));
            mainView.ClearView();

            controllers.Clear();

            OnLoadControllers();
            InitContextualMenu();
            InitPanel();
            Populate();

            if(CurrentController == null)
                CurrentController = controllers[0];
            mainView.OnClearSelection = () =>
            {
            // puede que esto generar clases que ya existe y se dupliquen revisar si puede llegar a ser un problema (?)
            var il = Reflection.MakeGenericScriptable(CurrentController.GetData());
                Selection.SetActiveObjectWithContext(il, il);
            };
        }

        public T GetController<T>()
        {
            return (T)controllers.Find(c => c is T);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Populate()
        {
            //clear(); (?)
            controllers.ForEach(c => c.PopulateView(mainView));
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitContextualMenu()
        {
            //clear(); (?)
            controllers.ForEach(c => c.SetContextualMenu(mainView));
        }

        private void InitPanel() // esto talvez deberia sacarse a otra clase para separar el panel del la ventana (??)
        {
            actions.Clear();
            OnInitPanel();
            var panel = new FloatingPanel("Option panel", actions, controllers, prevWindow, nextWindow);

            mainView.Add(panel);
        }


        private void InitToolBar()
        {
            // root toolbar
            var toolBar = new Toolbar();
            root.Insert(0, toolBar);

            // File menu option
            var fileMenu = new ToolbarMenu();
            fileMenu.text = "File";
            fileMenu.menu.AppendAction("New", (dma) => { LBSController.CreateNewLevel("new file", new Vector3(100, 100, 100)); GenericGraphWindow.RefeshAll(this); });
            fileMenu.menu.AppendAction("Load", (dma) => { LBSController.LoadFile(); GenericGraphWindow.RefeshAll(this); });
            fileMenu.menu.AppendAction("Save", (dma) => { LBSController.SaveFile(); });
            fileMenu.menu.AppendAction("Save as", (dma) => { LBSController.SaveFileAs(); });
            fileMenu.menu.AppendSeparator();
            fileMenu.menu.AppendAction("Representation.../Load", (dma) => { Debug.LogError("[Implementar loadRep]"); }); // ver si es necesaria (!)
            fileMenu.menu.AppendAction("Representation.../Save", (dma) => { Debug.LogError("[Implementar saveRep]"); }); // ver si es necesaria (!)
            fileMenu.menu.AppendAction("Representation.../Save as", (dma) => { Debug.LogError("[Implementar saveRep]"); }); // ver si es necesaria (!)
            fileMenu.menu.AppendSeparator();
            fileMenu.menu.AppendAction("Help.../Documentation", (dma) => { Debug.LogError("[Implementar documnetation]"); }); // ver si es necesaria (!)
            fileMenu.menu.AppendAction("Help.../About", (dma) => { Debug.LogError("[Implementar about]"); }); // ver si es necesaria (!)
            fileMenu.menu.AppendSeparator();
            fileMenu.menu.AppendAction("Close", (dma) => { this.Close(); });
            fileMenu.menu.AppendAction("Close All", (dma) => { GenericGraphWindow.CloseAll(this); });
            toolBar.Add(fileMenu);

            // search object in current window
            var search = new ToolbarPopupSearchField();
            search.tooltip = "[Implementar]";
            toolBar.Add(search);

            // file name label
            label = new Label();
            toolBar.Add(label);
        }

        private static void RefeshAll(GenericGraphWindow current)
        {
            var types = Reflection.GetAllSubClassOf<GenericGraphWindow>().ToList();
            types.ForEach((t) =>
            {
                MethodInfo method = typeof(EditorWindow).GetMethod(nameof(EditorWindow.HasOpenInstances)); // magia
                MethodInfo generic = method.MakeGenericMethod(t);
                if ((bool)generic?.Invoke(current, null))
                {
                    var w = (GenericGraphWindow)EditorWindow.GetWindow(t);
                    w.RefreshView();
                }
            });
        }

        private static void CloseAll(GenericGraphWindow current)
        {
            var types = Reflection.GetAllSubClassOf<GenericGraphWindow>().ToList();
            types.ForEach((t) =>
            {
                MethodInfo method = typeof(EditorWindow).GetMethod(nameof(EditorWindow.HasOpenInstances)); // magia
                MethodInfo generic = method.MakeGenericMethod(t);
                if ((bool)generic?.Invoke(current, null))
                    EditorWindow.GetWindow(t).Close();
            });
        }

        protected void ImportUXML(string name)
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>(name);
            visualTree.CloneTree(root);
        }

        protected void ImportStyleSheet(string name)
        {
            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>(name);
            root.styleSheets.Add(styleSheet);
        }

    }
}