using LBS.Behaviours;
using LBS.Components;
using LBS.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Drawer
{
    public string modeID; // (?)

    public Vector2 DefalutSize
    {
        get => LBSSettings.Instance.general.TileSize;
    }

    public Drawer() { }

    public abstract void Draw(object target, MainView view, Vector2 teselationSize);

    public virtual Texture2D GetTexture(object target, Rect sourceRect, Vector2Int teselationSize) { return null; }
}
