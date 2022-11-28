using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using LBS.Manipulators;
using LBS.VisualElements;
using LBS.Windows;
using LBS.Representation.TileMap;

namespace LBS.Overlays
{
    [Overlay(typeof(LBSSchemaWindow), ID, "Tools", "SchemaOverlayUSS", defaultDisplay = true)]
    public class SchemaOverlay : Overlay
    {
        private const string ID = "SchemaOverlayTools";

        // mejorable (!)
        public RoomData cTemp = null; 
        public Box box;
        public int index = 0;

        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();

            var btnGroup = new ButtonGroup();
            {
                var allMode = new PresedBtn();
                {
                    var wnd = EditorWindow.GetWindow<LBSSchemaWindow>();
                    allMode.clicked += () => wnd.MainView.SetBasicManipulators();
                    allMode.text = "All mode";
                }
                btnGroup.Add(allMode);

                var select = new PresedBtn();
                {
                    var wnd = EditorWindow.GetWindow<LBSSchemaWindow>();
                    select.clicked += () => wnd.MainView.SetManipulator(new RectangleSelector()); // mm no me gusta (!) 
                    select.text = "Select mode";
                }
                btnGroup.Add(select);

                var drag = new PresedBtn();
                {
                    var wnd = EditorWindow.GetWindow<LBSSchemaWindow>();
                    drag.clicked += () => wnd.MainView.SetManipulator(new SelectionDragger()); // mm no me gusta (!) 
                    drag.text = "Drag mode";
                }
                btnGroup.Add(drag);

                var zoom = new PresedBtn();
                {
                    var wnd = EditorWindow.GetWindow<LBSSchemaWindow>();
                    zoom.clicked += () => wnd.MainView.SetManipulator(new ContentZoomer()); // mm no me gusta (!) 
                    zoom.text = "Zoom mode";
                }
                btnGroup.Add(zoom);

                var b = new Box();
                b.style.minHeight = 10;
                btnGroup.Add(b);

                var removeTile = new PresedBtn();
                {
                    removeTile.clicked += () =>
                    {
                        var wnd = EditorWindow.GetWindow<LBSSchemaWindow>();
                        var c = wnd.GetController<LBSTileMapController>();
                        wnd.MainView.SetManipulator(new DeleteTileManipulator_other(wnd,c));
                    };
                   removeTile.text = "Delete tile mode";
                }
                btnGroup.Add(removeTile);

                var addTile = new PresedBtn();
                {
                    addTile.style.flexDirection = FlexDirection.Row;
                    addTile.style.alignItems = Align.Center;
                    box = new Box();
                    box.style.minHeight = box.style.minWidth = box.style.maxHeight = 10;
                    addTile.Add(box);
                    addTile.clicked += () =>
                    {
                        var wnd = EditorWindow.GetWindow<LBSSchemaWindow>();
                        var c = wnd.GetController<LBSTileMapController>();
                        var d = c.GetData() as LBSSchemaData;
                        cTemp = d.GetRooms()[index];
                        box.style.backgroundColor = cTemp.Color;
                        index = (index + 1) % d.GetRooms().Count;
                        wnd.MainView.SetManipulator(new AddTileManipulatorSchema(wnd,c,cTemp));
                    };
                    addTile.text = "Add tile mode";
                }
                btnGroup.Add(addTile);


                var addDoor = new PresedBtn();
                {
                    addDoor.clicked += () =>
                    {
                        var wnd = EditorWindow.GetWindow<LBSSchemaWindow>();
                        var c = wnd.GetController<LBSTileMapController>();
                        wnd.MainView.SetManipulator(new AddDoorManipulator(wnd,c));
                    };
                    addDoor.text = "Add door mode";
                }
                btnGroup.Add(addDoor);

                var removeDoor = new PresedBtn();
                {
                    removeDoor.clicked += () =>
                    {
                        var wnd = EditorWindow.GetWindow<LBSSchemaWindow>();
                        var c = wnd.GetController<LBSTileMapController>();
                        wnd.MainView.SetManipulator(new RemoveDoorManipulator(wnd, c));
                    };
                    removeDoor.text = "Remove door mode";
                }
                btnGroup.Add(removeDoor);

            }
            btnGroup.Init();
            root.Add(btnGroup);


            return root; 
        }
    }
}