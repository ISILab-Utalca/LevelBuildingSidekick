using LBS.Behaviours;
using LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSTool
{
    #region FIELDS
    private Texture2D icon;
    private string name;
    private LBSManipulator manipulator;
    #endregion

    #region PROPERTIES
    public Texture2D Icon => icon;
    public string Name => name;
    #endregion

    #region EVENTS
    public event Action OnStart;
    public event Action OnPressed;
    public event Action  OnEnd;
    #endregion

    #region CONSTRUCTORS
    public LBSTool(Texture2D icon, string name, LBSManipulator manipulator,
        Feedback feedback = null,
        Action OnStart = null,
        Action OnUpdate = null,
        Action OnEnd = null)
    {
        this.icon = icon;
        this.name = name;
        this.manipulator = manipulator;
    }
    #endregion

    #region METHODS
    public virtual void Init(MainView view, LBSLayer layer, LBSBehaviour behaviour)
    {
        manipulator.AddManipulationStart(OnStart);
        manipulator.AddManipulationUpdate(OnPressed);
        manipulator.AddManipulationEnd(OnEnd);

        manipulator.Init(view, layer, behaviour);

    }

    /*
    public void Link(LBSGrupableButton button)
    {
        button.OnFocusEvent += () => {
            view.AddManipulator(tool.manipulator);
        };
        button.OnBlurEvent += () => {
            view.RemoveManipulator(manipulator);
        };
    }
    */
    #endregion;
}
