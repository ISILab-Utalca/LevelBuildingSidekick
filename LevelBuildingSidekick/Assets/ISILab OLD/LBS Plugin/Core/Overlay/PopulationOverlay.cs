using LBS.Manipulators;
using LBS.VisualElements;
using LBS.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Overlays
{
    [Overlay(typeof(LBSPopulationWindow), ID, "Tools", "SchemaOverlayUSS", defaultDisplay = true)]
    public class PopulationOverlay : Overlay
    {
        private const string ID = "SchemaOverlayTools";

        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();

            var btnGroup = new ButtonGroupOld();
            {
                var allMode = new PresedBtnOld();
                {
                    var wnd = EditorWindow.GetWindow<LBSPopulationWindow>();
                    allMode.clicked += () => wnd.MainView.SetBasicManipulators();
                    allMode.text = "Handling mode";
                }
                btnGroup.Add(allMode);

                /*
                var select = new PresedBtn();
                {
                    var wnd = EditorWindow.GetWindow<LBSPopulationWindow>();
                    select.clicked += () => wnd.MainView.SetManipulator(new RectangleSelector()); // mm no me gusta (!) 
                    select.text = "Select mode";
                }
                btnGroup.Add(select);

                var drag = new PresedBtn();
                {
                    var wnd = EditorWindow.GetWindow<LBSPopulationWindow>();
                    drag.clicked += () => wnd.MainView.SetManipulator(new SelectionDragger()); // mm no me gusta (!) 
                    drag.text = "Drag mode";
                }
                btnGroup.Add(drag);

                var zoom = new PresedBtn();
                {
                    var wnd = EditorWindow.GetWindow<LBSPopulationWindow>();
                    zoom.clicked += () => wnd.MainView.SetManipulator(new ContentZoomer()); // mm no me gusta (!) 
                    zoom.text = "Zoom mode";
                }
                btnGroup.Add(zoom);
                */

                var b = new Box();
                b.style.minHeight = 10;
                btnGroup.Add(b);

                var addStamp = new PresedBtnOld();
                {
                    addStamp.clicked += () =>
                    {
                        var wnd = EditorWindow.GetWindow<LBSPopulationWindow>();
                        var c = wnd.GetController<LBSStampTileMapController>();
                        wnd.MainView.SetManipulator(new AddStampManipulator(wnd,c));
                    };
                    addStamp.text = "Add stamp mode";
                }
                btnGroup.Add(addStamp);

                var removeStamp = new PresedBtnOld();
                {
                    removeStamp.clicked += () =>
                    {
                        var wnd = EditorWindow.GetWindow<LBSPopulationWindow>();
                        var c = wnd.GetController<LBSStampTileMapController>();
                        wnd.MainView.SetManipulator(new RemoveStampManipulator(wnd,c));
                    };
                    removeStamp.text = "Remove stamp mode";
                }
                btnGroup.Add(removeStamp);

            }
            btnGroup.Init();
            root.Add(btnGroup);


            return root;
        }
    }
}
