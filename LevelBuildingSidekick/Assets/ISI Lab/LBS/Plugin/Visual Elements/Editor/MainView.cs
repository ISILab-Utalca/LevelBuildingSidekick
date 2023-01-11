using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class MainView : LBSGraphView
{
    public new class UxmlFactory : UxmlFactory<MainView, GraphView.UxmlTraits> { }

    public ExternalBounds visualBound;

    public Action<ContextualMenuPopulateEvent> OnBuild;
    public Action OnClearSelection;

    private List<Manipulator> manipulators = new List<Manipulator>();

    public MainView()
    {
        Insert(0, new GridBackground());
        var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("MainViewUSS");
        styleSheets.Add(styleSheet);
        style.flexGrow = 1;

        SetBasicManipulators();

        this.visualBound = new ExternalBounds(
            new Rect(
                new Vector2(-40000, -40000),
                new Vector2(80000, 80000)
                ),
            new Rect(
                new Vector2(-10000, -10000),
                new Vector2(20000, 20000)
                )
            );
        AddElement(visualBound);
    }

    public void SetBasicManipulators() // necesario aqui (?)
    {
        var manis = new List<Manipulator>() {
                new ClickSelector(),
                new ContentZoomer(),
                new ContentDragger(),
                new SelectionDragger(),
                new RectangleSelector()
            };

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
}
