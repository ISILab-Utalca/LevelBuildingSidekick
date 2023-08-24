using LBS;
using LBS.Components.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

[Obsolete]
public class LBSNodeView_Old<T> : GraphElement where T : LBSNode
{
    private T data;

    public T Data => data;

    public Action<Vector2Int> OnMoving;

    // Visual elements
    private Label label;
    private VisualElement background;

    public Color common = Color.white;
    public Color selcted = new Color(150 / 255f, 243 / 255f, 255 / 255f);

    public Action<MeshGenerationContext> OnGVC;

    public LBSNodeView_Old(T data, Vector2 position, Vector2 size)
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("NodeUxml");
        visualTree.CloneTree(this);

        var ss = DirectoryTools.SearchAssetByName<StyleSheet>("NodeUSS");
        this.styleSheets.Add(ss);

        this.data = data;

        capabilities |= Capabilities.Selectable | Capabilities.Movable | Capabilities.Ascendable | Capabilities.Copiable | Capabilities.Snappable | Capabilities.Groupable;
        usageHints = UsageHints.DynamicTransform;

        RegisterCallback<MouseDownEvent>(OnMouseDown);
        RegisterCallback<MouseUpEvent>(OnMouseUp);

        this.SetPosition(new Rect(position, size));

        // Label
        label = this.Q<Label>();
        background = this.Q<VisualElement>("Background");

        background.SetBorderRadius(size.x / 2f);
        this.generateVisualContent += OnGenerateVisualContent;
    }

    private void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        OnGVC?.Invoke(mgc);
    }

    internal void SetColor(Color color)
    {
        background.style.backgroundColor = color;
    }

    internal void SetText(string text)
    {
        if(text.Length > 11)
        {
            text = text.Substring(0, 8) + "...";
        }

        label.text = text;
    }

    private void OnMouseDown(MouseDownEvent evt)
    {

    }

    private void OnMouseUp(MouseUpEvent evt)
    {

    }

    /// <summary>
    /// Set a new position by parameter.
    /// </summary>
    /// <param name="newPos"> New position given.</param>
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        var center = base.GetPosition().center;
        var nPos = new Vector2Int((int)center.x, (int)center.y);
        Data.Position = nPos;
        OnMoving?.Invoke(nPos);
        this.MarkDirtyRepaint(); 
    }

    public override void OnSelected()
    {
        base.OnSelected();

        background.SetBorder(selcted, 8);

        var il = Reflection.MakeGenericScriptable(Data);

        LBSEvents.OnSelectElementInWorkSpace?.Invoke(il);
        Selection.SetActiveObjectWithContext(il, il);
    }

    public override void OnUnselected()
    {
        base.OnUnselected();
        background.SetBorder(common, 8);
    }

}