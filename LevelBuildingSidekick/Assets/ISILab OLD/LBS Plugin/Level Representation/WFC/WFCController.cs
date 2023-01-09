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

        /// <summary>
        /// Se ingresa un vector2 posición
        /// </summary>
        /// <param name="position"> Vector2 posición del tile</param>
        /// <returns> Un nuevo vector2 </returns>
        public Vector2 FromTileCoords(Vector2 position)
        {
            //throw new System.NotImplementedException();
            return new Vector2();
        }

        /// <summary>
        /// Función para solicitar el nombre
        /// </summary>
        /// <returns> Retorna un string con el nombre en especifico</returns>
        public override string GetName()
        {
            return "WFC controller";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="cmpe"></param>
        public override void OnContextualBuid(MainView view, ContextualMenuPopulateEvent cmpe)
        {
           // throw new System.NotImplementedException();
        }

        /// <summary>
        /// Elimina los elementos en pantalla, para posteriormente
        /// crear una visualización de los tiles, por cada tile
        /// </summary>
        /// <param name="view"> Vista actual </param>
        public override void PopulateView(MainView view)
        {
            this.view = view;
            view.DeleteElements(elements);

            foreach (var tile in data.Tiles)
            {
                var tv = CreateTileView(tile, tile.Position, new Vector2(100, 100));
                elements.Add(tv);
                view.AddElement(tv);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2Int ToTileCoords(Vector2 position)
        {
            //throw new System.NotImplementedException();
            return new Vector2Int();
        }

        /// <summary>
        /// Funcion para crear la ventana de los tiles
        /// </summary>
        /// <param name="tileData"> Info de los tiles </param>
        /// <param name="tilePos"> Vector2 con la posición de los tiles </param>
        /// <param name="size"> Vector2 con el tamaño de la ventana</param>
        /// <returns> Variable tile de tipo "TileSimple" </returns>
        private TileView CreateTileView(TileData tileData, Vector2Int tilePos, Vector2 size)
        {
            var tile = new TileSimple();
            tile.style.marginBottom = tile.style.marginLeft = tile.style.marginRight = tile.style.marginTop = 0;
            tile.style.paddingBottom = tile.style.paddingLeft = tile.style.paddingRight = tile.style.paddingTop = 0;
            tile.style.borderRightWidth = tile.style.borderBottomWidth = tile.style.borderLeftWidth = tile.style.borderTopWidth = 0;
            var rs = tile.resolvedStyle;
            var s = tile.style;

            tile.SetView(tileData.Connections);
            tile.SetPosition(new Rect(tilePos * size, size));
            tile.SetSize((int)size.x, (int)size.y);
            //tile.SetLabel(tilePos);
            return tile;
        }

    }
}