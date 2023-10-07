using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

[LBSCustomEditor("Constraint", typeof(Constraint))]
public class ConstraintView : LBSCustomEditor
{
    #region FIELDS
    private Constraint constraint;
    #endregion

    #region VIEW FIELDS
    private Foldout foldout;

    private FloatField widthMin;
    private FloatField widthMax;

    private FloatField heightMin;
    private FloatField heightMax;
    #endregion

    #region CONSTRUCTORS
    public ConstraintView()
    {
        CreateVisualElement();
    }
    #endregion

    #region METHODS
    public void SetName(string name)
    {
        this.foldout.text = name;
    }

    public void SetData(ConstraintPair pair)
    {
        SetName(pair.Zone.ID);
        this.constraint = pair.Constraint;

        widthMin.value = pair.Constraint.minWidth;
        widthMax.value = pair.Constraint.maxWidth;

        heightMin.value = pair.Constraint.minHeight;
        heightMax.value = pair.Constraint.maxHeight;
    }

    public override void SetInfo(object target)
    {
        var t = target as Constraint;
        constraint = t;

        widthMin.value = t.minWidth;
        widthMax.value = t.maxWidth;

        heightMin.value = t.minHeight;
        heightMax.value = t.maxHeight;
    }

    protected override VisualElement CreateVisualElement()
    {
        var visualTree = DirectoryTools.SearchAssetByName<VisualTreeAsset>("ConstraintView");
        visualTree.CloneTree(this);

        // Foldout
        this.foldout = this.Q<Foldout>();

        // Width input
        this.widthMin = this.Q<FloatField>("WidthMin");
        widthMin.RegisterCallback<ChangeEvent<float>>((evt) => { constraint.minWidth = evt.newValue; DrawManager.ReDraw(); });
        this.widthMax = this.Q<FloatField>("WidthMax");
        widthMax.RegisterCallback<ChangeEvent<float>>((evt) => { constraint.maxWidth = evt.newValue; DrawManager.ReDraw(); });

        // Height input
        this.heightMin = this.Q<FloatField>("HeightMin");
        heightMin.RegisterCallback<ChangeEvent<float>>((evt) => { constraint.minHeight = evt.newValue; DrawManager.ReDraw(); });
        this.heightMax = this.Q<FloatField>("HeightMax");
        heightMax.RegisterCallback<ChangeEvent<float>>((evt) => { constraint.maxHeight = evt.newValue; DrawManager.ReDraw(); });

        return this;
    }
    #endregion
}