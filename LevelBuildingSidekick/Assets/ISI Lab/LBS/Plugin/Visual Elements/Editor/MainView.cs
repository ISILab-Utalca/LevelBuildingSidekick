using LBS.Components;
using LBS.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class MainView : GraphView // Canvas or WorkSpace
{
    #region UXML_FACTORY
    public new class UxmlFactory : UxmlFactory<MainView, GraphView.UxmlTraits> { }
    #endregion

    #region SINGLETON
    private static MainView instance;
    public static MainView Instance
    {
        get => instance;
    }

    public Vector2 FixPos(Vector2 v) // (?) esto deberia estar aqui? // poner estatico
    {
        var t = new Vector2(this.viewTransform.position.x, this.viewTransform.position.y);
        var newPos = (v - t) / this.scale;
        return newPos;
    }
    #endregion

    #region FIELDS
    private ExternalBounds bound;
    private List<Manipulator> manipulators = new List<Manipulator>();
    #endregion

    #region EVENTS
    public event Action OnClearSelection;
    #endregion

    #region CONSTRUCTORS
    public MainView()
    {
        Insert(0, new GridBackground());
        var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("MainViewUSS");
        styleSheets.Add(styleSheet);
        style.flexGrow = 1;

        SetBasicManipulators();
        InitBound(20000, int.MaxValue / 2);

        AddElement(bound);

        // Singleton
        if (instance != this)
            instance = this;
    }
    #endregion

    #region METHODS
    public void SetBasicManipulators() // necesario aqui (?)
    {
        var setting = LBSSettings.Instance.general;

        var zoomer = new ContentZoomer();

        setting.OnChangeZoomValue = (min, max) =>
        {
            zoomer.maxScale = setting.zoomMax;
            zoomer.minScale = setting.zoomMin;
        };

        zoomer.maxScale = setting.zoomMax;
        zoomer.minScale = setting.zoomMin;

        var cDragger = new ContentDragger();
        var sDragger = new SelectionDragger();

        var manis = new List<Manipulator>() { zoomer, cDragger, sDragger };
        SetManipulators(manis);
    }

    public override void ClearSelection() // (?)
    {
        base.ClearSelection();
        if (selection.Count == 0)
        {
            OnClearSelection?.Invoke();
        }
    }

    public void SetManipulator(Manipulator current)
    {
        ClearManipulators();
        this.AddManipulator(current);
    }

    public void SetManipulators(List<Manipulator> manipulators)
    {
        ClearManipulators();
        AddManipulators(manipulators);
    }

    public void ClearManipulators()
    {
        foreach (var m in this.manipulators)
        {
            this.RemoveManipulator(m as IManipulator);
        }
        this.manipulators.Clear();
    }

    public void RemoveManipulator(Manipulator manipulator)
    {
        this.manipulators.Remove(manipulator);
        this.RemoveManipulator(manipulator as IManipulator);
    }

    public void RemoveManipulators(List<Manipulator> manipulators)
    {
        foreach (var m in manipulators)
        {
            this.manipulators.Remove(m);
            this.RemoveManipulator(m as IManipulator);
        }
    }

    public void AddManipulator(Manipulator manipulator)
    {
        this.manipulators.Add(manipulator);
        this.AddManipulator(manipulator as IManipulator);
    }

    public void AddManipulators(List<Manipulator> manipulators)
    {
        foreach (var m in manipulators)
        {
            if (!this.manipulators.Contains(m))
            {
                this.manipulators.Add(m);
                this.AddManipulator(m as IManipulator);
            }
        }
    }

    public void ClearView()
    {
        this.graphElements.ForEach(e => this.RemoveElement(e));
        AddElement(bound);
    }

    public new void AddElement(GraphElement element)
    {
        base.AddElement(element);
    }

    private void InitBound(int interior, int exterior)
    {
        this.bound = new ExternalBounds(
            new Rect(
                new Vector2(-interior, -interior),
                new Vector2(interior * 2, interior * 2)
                ),
            new Rect(
                new Vector2(-exterior, -exterior),
                new Vector2(exterior * 2, exterior * 2)
                )
            );
    }

    #endregion
}
