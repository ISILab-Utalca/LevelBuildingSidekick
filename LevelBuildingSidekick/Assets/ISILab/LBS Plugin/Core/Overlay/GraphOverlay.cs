using LBS.Transformers;
using LBS.Windows;
using LBS;
using LBS.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using LBS.Manipulators;
using LBS.VisualElements;

namespace LBS.Overlays
{
    [Overlay(typeof(LBSGraphRCWindow), ID, "Tools", "GraphOverlayUSS", defaultDisplay = true)]
    public class GraphOverlay : Overlay // esta corresponderia a todas las funciones bases del derivados del graphview.
    {
        private const string ID = "GraphOverlayTools";
         
        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();

            var btnGroup = new ButtonGroup();
            {

                var allMode = new PresedBtn();
                {
                    var wnd = EditorWindow.GetWindow<LBSGraphRCWindow>();
                    allMode.clicked += () => wnd.MainView.SetBasicManipulators();
                    allMode.text = "All mode";
                }
                btnGroup.Add(allMode);


                var select = new PresedBtn();
                {
                    var wnd = EditorWindow.GetWindow<LBSGraphRCWindow>();
                    select.clicked += () => wnd.MainView.SetManipulator(new RectangleSelector()); // mm no me gusta (!) 
                    select.text = "Select mode";
                }
                btnGroup.Add(select);

                var drag = new PresedBtn();
                {
                    var wnd = EditorWindow.GetWindow<LBSGraphRCWindow>();
                    drag.clicked += () => wnd.MainView.SetManipulator(new SelectionDragger()); // mm no me gusta (!) 
                    drag.text = "Drag mode";
                }
                btnGroup.Add(drag);

                var zoom = new PresedBtn();
                {
                    var wnd = EditorWindow.GetWindow<LBSGraphRCWindow>();
                    zoom.clicked += () => wnd.MainView.SetManipulator(new ContentZoomer()); // mm no me gusta (!) 
                    zoom.text = "Zoom mode";
                }
                btnGroup.Add(zoom);

                var b = new Box();
                b.style.minHeight = 10;
                btnGroup.Add(b);

                var delete = new PresedBtn();
                {
                    delete.clicked += () =>
                    {
                        var wnd = EditorWindow.GetWindow<LBSGraphRCWindow>();
                        var c = wnd.GetController<LBSGraphRCController>();
                        wnd.MainView.SetManipulator(new DeleteNodeManipulator(wnd,c));
                    };
                    delete.text = "Delete mode";
                }
                btnGroup.Add(delete);

                var addNode = new PresedBtn();
                {
                    addNode.clicked += () => {
                        var wnd = EditorWindow.GetWindow<LBSGraphRCWindow>();
                        var c = wnd.GetController<LBSGraphRCController>();
                        wnd.MainView.SetManipulator(new AddNodeManipulator(wnd,c)); 
                    };
                    addNode.text = "Add node mode";
                }
                btnGroup.Add(addNode);

                var connect = new PresedBtn();
                {
                    connect.clicked += () =>
                    {
                        var wnd = EditorWindow.GetWindow<LBSGraphRCWindow>();
                        var c = wnd.GetController<LBSGraphRCController>();
                        wnd.MainView.SetManipulator(new ConnectionManipulator(wnd,c));
                    };
                    connect.text = "Connect mode";
                }
                btnGroup.Add(connect);
            }
            btnGroup.Init();
            root.Add(btnGroup);


            return root;
        }
    }

}