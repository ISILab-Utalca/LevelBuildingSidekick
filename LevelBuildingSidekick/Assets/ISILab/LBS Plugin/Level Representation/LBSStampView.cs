using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class LBSStampView : LBSGraphElement // esto se puede simplificar y combiniar con nodeView
{
    private static readonly float size = 64; //esto debería ser ajustable
    private Texture2D texture;

    private StampData data;

    public StampData Data => data;

    public LBSStampView(StampData data,LBSGraphView root) : base(root)
    {
        this.data = data;
        SetPosition(new Rect(data.Position, Vector2.one * size));

        var stampPrest = DirectoryTools.GetScriptable<StampPresset>(data.Label);
        this.style.backgroundImage = stampPrest.Icon;
        this.style.minHeight = this.style.minWidth = this.style.maxHeight = this.style.maxWidth = size;

        capabilities |= Capabilities.Selectable | Capabilities.Movable | Capabilities.Deletable | Capabilities.Ascendable | Capabilities.Copiable | Capabilities.Snappable | Capabilities.Groupable;
        usageHints = UsageHints.DynamicTransform;
    }

    public override void OnDelete()
    {
        throw new System.NotImplementedException();
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        data.Position = new Vector2Int((int)newPos.x, (int)newPos.y);
    }
}
