using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class TagInfo : VisualElement
{
    #region FACTORY
    public new class UxmlFactory : UxmlFactory<TagInfo, VisualElement.UxmlTraits> { }
    #endregion

    private LBSIdentifier target;

    private ObjectField parentField;
    private TextField labelField;
    private ColorField colorField;
    private ObjectField iconField;

    public TagInfo()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("TagInfo");
        visualTree.CloneTree(this);

        // ParentField
        parentField = this.Q<ObjectField>("ParentField");
        parentField.RegisterCallback<ChangeEvent<Object>>(e => {
            var prevP = (e.previousValue as LBSIdentifierBundle);
            prevP?.Remove(target);

            var newP = (e.newValue as LBSIdentifierBundle);
            newP.Add(target);
        });

        // LabelField
        labelField = this.Q<TextField>("LabelField");
        labelField.RegisterCallback<BlurEvent>(e => target.Label = labelField.value);

        // ColorField
        colorField = this.Q<ColorField>("ColorField");
        colorField.RegisterCallback<BlurEvent>(e => target.Color = colorField.value);

        // IconField
        iconField = this.Q<ObjectField>("IconField");
        iconField.RegisterCallback<ChangeEvent<Object>>(e => target.Icon = (e.newValue as Texture2D));
    }

    public void SetInfo(LBSIdentifier target)
    {
        this.SetDisplay(target != null);
        this.target = target;

        var storage = LBSAssetsStorage.Instance;
        parentField.SetValueWithoutNotify(storage.Get<LBSIdentifierBundle>().Find(b => b.Tags.Contains(target)));

        labelField.value = target.Label;
        colorField.value = target.Color;
        iconField.value = target.Icon;

        target.OnChangeColor += (tag) => colorField.value = tag.Color;
        target.OnChangeIcon += (tag) => iconField.value = tag.Icon;
        target.OnChangeText += (tag) => labelField.value = tag.Label;
    }
}
