using ISILab.LBS.Behaviours;
using ISILab.LBS.Components;
using ISILab.LBS.Modules;
using LBS.Components.TileMap;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GABO TODO: FALTA VER COMO TRATAR ELEMENT TAGS Y EVENT TAGS. VER PARA Q SIRVE SERIALIZEREFERENCE
public class PathOSTile
{
    #region FIELDS
    [SerializeField, JsonRequired]
    private int x, y;
    [SerializeField, JsonRequired]
    private PathOSTag tag;
    // Booleanos para Event Tags
    [SerializeField, JsonRequired]
    private bool isDynamicTagObject = false;
    [SerializeField, JsonRequired]
    private bool isDynamicTagTrigger = false;
    [SerializeField, JsonRequired]
    private bool isDynamicObstacleObject = false;
    [SerializeField, JsonRequired]
    private bool isDynamicObstacleTrigger = false;
    //private List<PathOSTag> tags = new List<PathOSTag>();
    #endregion

    #region CONSTRUCTORS
    public PathOSTile(int x, int y, PathOSTag tag = null)
    {
        this.x = x;
        this.y = y;
        if (tag != null) { this.tag = tag; }
    }
    #endregion

    #region PROPERTIES
    public int X { get { return x; } set { x = value; } }
    public int Y { get { return y; } set { y = value; } }
    public PathOSTag Tag { get { return tag; } set { tag = value; } }
    public bool IsDynamicTagObject { get {  return isDynamicTagObject; } set { isDynamicTagObject = value; } }
    public bool IsDynamicTagTrigger { get {  return isDynamicTagTrigger; } set { isDynamicTagTrigger = value; } }
    public bool IsDynamicObstacleObject { get {  return isDynamicObstacleObject; } set { isDynamicObstacleObject = value; } }
    public bool IsDynamicObstacleTrigger { get {  return isDynamicObstacleTrigger; } set { isDynamicObstacleTrigger = value; } }
    //public PathOSTag Tags { get { return tags; } set { tags = value; } }
    #endregion

    // GABO TODO: HACER METODOS PARA ACTIVAR Y DESACTIVAR LOS EVENT TAGS
    #region METHODS
    #endregion

    #region NOT_IN_USE
    //public void AddTag(PathOSTag tag)
    //{
    //    // Chequeo nulo
    //    if (tag == null) { Debug.LogWarning("PathOSTile.AddTag(): Tag nulo!"); return; }

    //    // Remocion tag antiguo (si existe) y agregar nuevo
    //    var t = GetTag(tag);
    //    if (t != null)
    //    {
    //        tags.Remove(t);
    //    }
    //    tags.Add(tag);
    //}

    //public PathOSTag GetTag(PathOSTag tag)
    //{
    //    if (tags.Count <= 0)
    //        return null;
    //    return tags.Find(t => t.Label.Equals(tag.Label));

    //}

    //public void RemoveTag(PathOSTag tag)
    //{
    //    // Chequeo nulo
    //    if (tag == null) { Debug.LogWarning("PathOSTile.RemoveTag(): Tag nulo!"); return; }

    //    var t = GetTag(tag);
    //    if (t != null)
    //    {
    //        tags.Remove(t);
    //    }
    //}
    #endregion


}
