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

        public RoomData cTemp = null;

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
                        wnd.MainView.SetManipulator(new DeleteTileManipulator(wnd,c));
                    };
                   removeTile.text = "Delete tile mode";
                }
                btnGroup.Add(removeTile);

                var addTile = new PresedBtn();
                {
                    addTile.clicked += () =>
                    {
                        var wnd = EditorWindow.GetWindow<LBSSchemaWindow>();
                        var c = wnd.GetController<LBSTileMapController>();
                        wnd.MainView.SetManipulator(new AddTileManipulator(wnd,c,cTemp));
                    };
                    addTile.text = "Add tile mode";
                }
                btnGroup.Add(addTile);

                var changeRoom = new Button();
                {
                    var box = new Box();
                    changeRoom.Add(box);
                    changeRoom.clicked += () =>
                    {
                        var wnd = EditorWindow.GetWindow<LBSSchemaWindow>();
                        var c = wnd.GetController<LBSStampTileMapController>();
                        var d = c.GetData() as LBSTileMapData;
                        var x = d.GetRooms()[Random.Range(0, d.GetRooms().Count)];
                        cTemp = x;
                        box.style.color = x.Color;
                    };
                    changeRoom.text = "Change room:";

                }
                btnGroup.Add(changeRoom);

                var addDoor = new PresedBtn();
                {
                    var wnd = EditorWindow.GetWindow<LBSSchemaWindow>();
                    var c = wnd.GetController<LBSStampTileMapController>();
                    //delete.clicked += () => wnd.MainView.SetManipulator(new DeleteManipulator(c));
                    addDoor.text = "Add door mode";
                }
                btnGroup.Add(addDoor);

                var removeDoor = new PresedBtn();
                {
                    var wnd = EditorWindow.GetWindow<LBSSchemaWindow>();
                    var c = wnd.GetController<LBSStampTileMapController>();
                    //delete.clicked += () => wnd.MainView.SetManipulator(new DeleteManipulator(c));
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