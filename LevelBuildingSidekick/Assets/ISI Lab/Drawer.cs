using ISILab.LBS.VisualElements.Editor;
using LBS.Components;
using LBS.Settings;
using UnityEngine;

[System.Serializable]
public abstract class Drawer
{
    public Vector2 DefalutSize
    {
        get => LBSSettings.Instance.general.TileSize;
    }

    public Drawer() { }

    public abstract void Draw(object target, MainView view, Vector2 teselationSize);

    public virtual void ReDraw(LBSLayer layer, object[] olds, object[] news, MainView view, Vector2 teselationSize) { }

    public virtual Texture2D GetTexture(object target, Rect sourceRect, Vector2Int teselationSize) 
    {
        LBSMainWindow.MessageNotify("Texture generation not implemented.", LogType.Warning);
        return new Texture2D(16, 16);
    }
}
