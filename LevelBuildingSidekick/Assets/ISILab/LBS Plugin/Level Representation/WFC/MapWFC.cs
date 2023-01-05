using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class MapWFC : MonoBehaviour
{
    [SerializeField]
    private float tileSize = 1;

    [SerializeField]
    public int m_extraBorder = 5;

    private List<TileWFC_struct> tiles = new List<TileWFC_struct>();

    public float TileSize { get => tileSize; }

    public List<TileWFC_struct> Tiles { get => tiles; }

    /// <summary>
    /// Si no se encuentran tiles, se retorna la posición inicial "0,0"
    /// en caso de que no sea así, se busca la posición x,y con el menor valor numerico
    /// al no encontrar alguna otra se determina el valor final y se retorna
    /// </summary>
    /// <returns> Un vector nuevo con la posición de los valores minimos para X e Y </returns>
    public Vector2Int GetMin()
    {
        if (Tiles.Count <= 0)
            return new Vector2Int(0, 0);

        var minX = int.MaxValue;
        var minY = int.MaxValue;
        foreach (var tile in Tiles)
        {
            if (tile.position.x < minX)
                minX = tile.position.x;
            if (tile.position.y < minY)
                minY = tile.position.y;
        }
        return new Vector2Int(minX, minY);
    }

    /// <summary>
    /// Si no se encuentran tiles, se retorna la posición inicial "0,0"
    /// en caso de que no sea así, se busca la posición x,y con el mayor valor numerico
    /// al no encontrar alguna otra se determina el valor final y se retorna
    /// </summary>
    /// <returns>Un vector nuevo con la posición de los valores maximos para X e Y </returns>
    public Vector2Int GetMax()
    {
        if (Tiles.Count <= 0)
            return new Vector2Int(0, 0);

        var maxX = int.MinValue;
        var maxY = int.MinValue;
        foreach (var tile in Tiles)
        {
            if (tile.position.x > maxX)
                maxX = tile.position.x;
            if (tile.position.y > maxY)
                maxY = tile.position.y;
        }
        return new Vector2Int(maxX, maxY);
    }

    /// <summary>
    /// Se evalua si se se puede añadir un tile o si el lugar está utilizado
    /// al estar utilizado, manda un mensaje de error y retorna vacio
    /// si no existe un tile en la posición, se añade el tile
    /// </summary>
    /// <param name="tile"> Variable estructura para los tiles </param>
    public void Add(TileWFC_struct tile)
    {
        if(tiles.Any(t => t.position == tile.position))
        {
            Debug.LogWarning("the mosaic cannot be added because that space is already occupied");
            return;
        }
        tiles.Add(tile);
    }

    /// <summary>
    /// Se elimina un tile de una posición
    /// para luego añadir otro en el mismo lugar
    /// </summary>
    /// <param name="tile"> Variable estructura para los tiles </param>
    public void Replace(TileWFC_struct tile)
    {
        try
        {
            var last = tiles.First(t => t.position == tile.position);
            tiles.Remove(last);
        }
        catch (System.Exception)
        {
            //throw;
        }
        tiles.Add(tile);
    }
}

public struct TileWFC_struct
{
    public TileConnectWFC data;
    public Vector2Int position;
    public bool locked;

    public TileWFC_struct(TileConnectWFC data, Vector2Int position, bool locked)
    {
        this.data = data;
        this.position = position;
        this.locked = locked;
    }
}


