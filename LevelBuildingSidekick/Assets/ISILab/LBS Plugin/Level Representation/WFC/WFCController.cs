using LBS.ElementView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Representation
{
    public class WFCController : LBSRepController<MapData>, ITileMap
    {
        public WFCController(LBSGraphView view, MapData data) : base(view, data)
        {

        }

        public float Subdivision => throw new System.NotImplementedException();

        public float TileSize => throw new System.NotImplementedException();

        public int MatrixWidth => throw new System.NotImplementedException();

        public Vector2 FromTileCoords(Vector2 position)
        {
            //throw new System.NotImplementedException();
            return new Vector2();
        }

        public override string GetName()
        {
            return "WFC controller";
        }

        public override void OnContextualBuid(MainView view, ContextualMenuPopulateEvent cmpe)
        {
           // throw new System.NotImplementedException();
        }

        public override void PopulateView(MainView view)
        {
            //throw new System.NotImplementedException();
        }

        public Vector2Int ToTileCoords(Vector2 position)
        {
            //throw new System.NotImplementedException();
            return new Vector2Int();
        }

    }
}