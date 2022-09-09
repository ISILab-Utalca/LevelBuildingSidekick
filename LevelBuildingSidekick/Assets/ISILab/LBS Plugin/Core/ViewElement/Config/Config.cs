using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Config : Button
{
    public new class UxmlFactory : UxmlFactory<Config, VisualElement.UxmlTraits> { }


    public Label lTop;
    public Label labelBot;
    public VisualElement img;
    public Button arrowConf;

    public Config() 
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ConfigUXML");
        visualTree.CloneTree(this);

        lTop = this.Q<Label>(name: "labelTop");
        img = this.Q<VisualElement>(name: "img");
        labelBot = this.Q<Label>(name: "labelBot");
        arrowConf = this.Q<Button>(name: "arrowConf");

        lTop.text = "Defaul label";
        labelBot.text = "Defalult";
    }

    public Config(string labelText, string fileName, Texture2D texture2D)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ConfigUXML");
        visualTree.CloneTree(this);

        lTop = this.Q<Label>(name: "labelTop");
        img = this.Q<VisualElement>(name: "img");
        labelBot = this.Q<Label>(name: "labelBot");

        lTop.text = labelText;
        labelBot.text = fileName;
        img.style.backgroundImage = texture2D;
    }

}
