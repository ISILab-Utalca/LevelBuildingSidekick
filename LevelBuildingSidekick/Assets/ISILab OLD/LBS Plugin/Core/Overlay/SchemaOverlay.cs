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
        public bool dragSelector = false;
        public Box box, dragBox, brushBox, diagonalBox;
        public int index = 0;

        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();

            var btnGroup = new ButtonGroupOld();
            {
                var allMode = new PresedBtnOld();
                {
                    var wnd = EditorWindow.GetWindow<LBSSchemaWindow>();
                    allMode.clicked += () => wnd.MainView.SetBasicManipulators();
                    allMode.text = "Handling mode";
                }
                btnGroup.Add(allMode);

                /*
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

                */

                var b = new Box();
                b.style.minHeight = 10;
                btnGroup.Add(b);

                var removeTile = new PresedBtnOld();
                ////Remove tile Mode
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

                var addTile = new PresedBtnOld();
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
                        if (d.GetRooms() == null || d.GetRooms().Count <= 0)
                            return;

                        cTemp = d.GetRooms()[index];
                        box.style.backgroundColor = cTemp.Color;
                        index = (index + 1) % d.GetRooms().Count;
                        wnd.MainView.SetManipulator(new AddTileManipulatorSchema(wnd,c,cTemp));
                    };
                    addTile.text = "Add tile mode";
                }
                btnGroup.Add(addTile);


                var addDoor = new PresedBtnOld();
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

                var removeDoor = new PresedBtnOld();
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

                var dragTiles = new PresedBtnOld();
                {
                    dragTiles.style.flexDirection = FlexDirection.Row;
                    dragTiles.style.alignItems = Align.Center;
                    dragBox = new Box();
                    dragBox.style.minHeight = dragBox.style.minWidth = dragBox.style.maxHeight = 10;
                    dragTiles.Add(dragBox);
                    dragTiles.clicked += () =>
                    {
                        var wnd = EditorWindow.GetWindow<LBSSchemaWindow>();
                        var c = wnd.GetController<LBSTileMapController>();
                        var d = c.GetData() as LBSSchemaData;
                        if (d.GetRooms() == null || d.GetRooms().Count <= 0)
                            return;

                        cTemp = d.GetRooms()[index];
                        dragBox.style.backgroundColor = cTemp.Color;
                        index = (index + 1) % d.GetRooms().Count;
                        wnd.MainView.SetManipulator(new CreateTileDragingManipulatorSchema(wnd, c, cTemp));
                    };
                    dragTiles.text = "Add drag mode";
                }
                btnGroup.Add(dragTiles);

                //Drag delete mode
                var dragDeleteTiles = new PresedBtnOld();
                {
                    dragDeleteTiles.clicked += () =>
                    {
                        var wnd = EditorWindow.GetWindow<LBSSchemaWindow>();
                        var c = wnd.GetController<LBSTileMapController>();
                        //wnd.MainView.SetManipulator(new DeleteTileGridingManipulator_Scheme(wnd, c));
                    };
                    dragDeleteTiles.text = "Delete drag mode";
                }
                btnGroup.Add(dragDeleteTiles);

                //Brush tile mode
                var brushTiles = new PresedBtnOld();
                {
                    brushTiles.style.flexDirection = FlexDirection.Row;
                    brushTiles.style.alignItems = Align.Center;
                    brushBox = new Box();
                    brushBox.style.minHeight = brushBox.style.minWidth = brushBox.style.maxHeight = 10;
                    brushTiles.Add(brushBox);
                    brushTiles.clicked += () =>
                    {
                        var wnd = EditorWindow.GetWindow<LBSSchemaWindow>();
                        var c = wnd.GetController<LBSTileMapController>();
                        var d = c.GetData() as LBSSchemaData;
                        if (d.GetRooms() == null || d.GetRooms().Count <= 0)
                            return;

                        cTemp = d.GetRooms()[index];
                        brushBox.style.backgroundColor = cTemp.Color;
                        index = (index + 1) % d.GetRooms().Count;
                        wnd.MainView.SetManipulator(new CreateTileDragingManipulatorSchema(wnd, c, cTemp));
                    };
                    brushTiles.text = "Add brush mode";
                }
                btnGroup.Add(brushTiles);

                //Diagonal tile mode
                var diagonalTiles = new PresedBtnOld();
                {
                    diagonalTiles.style.flexDirection = FlexDirection.Row;
                    diagonalTiles.style.alignItems = Align.Center;
                    diagonalBox = new Box();
                    diagonalBox.style.minHeight = diagonalBox.style.minWidth = diagonalBox.style.maxHeight = 10;
                    diagonalTiles.Add(diagonalBox);
                    diagonalTiles.clicked += () =>
                    {
                        var wnd = EditorWindow.GetWindow<LBSSchemaWindow>();
                        var c = wnd.GetController<LBSTileMapController>();
                        var d = c.GetData() as LBSSchemaData;
                        if (d.GetRooms() == null || d.GetRooms().Count <= 0)
                            return;

                        cTemp = d.GetRooms()[index];
                        diagonalBox.style.backgroundColor = cTemp.Color;
                        index = (index + 1) % d.GetRooms().Count;
                        wnd.MainView.SetManipulator(new CreateDiagonalTilesManipulatorsSchema(wnd, c, cTemp));
                    };
                    diagonalTiles.text = "Add diagonal mode";
                }
                btnGroup.Add(diagonalTiles);
            }
            
            btnGroup.Init();
            root.Add(btnGroup);

            return root; 
        }
    }
}