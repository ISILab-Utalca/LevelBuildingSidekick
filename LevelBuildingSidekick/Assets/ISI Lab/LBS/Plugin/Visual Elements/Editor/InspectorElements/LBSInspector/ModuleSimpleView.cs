using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ModuleSimpleView : VisualElement
{
    private Label labelName;
    private Label labelID;
    private VisualElement icon;

    public ModuleSimpleView()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ModuleSimpleView");
        visualTree.CloneTree(this);

        this.labelName = this.Q<Label>("LabelName");
        this.icon = this.Q<VisualElement>("Icon");
        this.labelID = this.Q<Label>("LabelID");
    }

    public void SetInfo(string name,string id = "",Texture2D icon = null)
    {
        labelName.text = name;


        labelID.text = (id == "") ? "" : "[" + id + "]";

        if(icon != null)
            this.icon.style.backgroundImage = icon;
    }
}
