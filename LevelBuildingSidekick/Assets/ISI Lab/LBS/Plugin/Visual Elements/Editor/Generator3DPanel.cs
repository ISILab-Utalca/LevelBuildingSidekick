using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Generator;
using LBS.Components;

public class Generator3DPanel : VisualElement
{
    ClassDropDown dropDown;
    Vector3Field position;
    Vector2Field scale;
    TextField objName;

    Generator generator;
    Button action;
    Toggle destroyPrev;

    LBSLayer layer;

    public Generator Generator
    {
        get => generator;
        set => generator = value;
    }

    public new class UxmlFactory : UxmlFactory<Generator3DPanel, VisualElement.UxmlTraits> { }

    public Generator3DPanel() { }

    public Generator3DPanel(LBSLevelData data)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Generator3DPanel"); // Editor
        visualTree.CloneTree(this);

        Init();
    }

    public void Init()
    {
        position = this.Q<Vector3Field>(name: "Position");

        scale = this.Q<Vector2Field>(name: "Scale");
        scale.value = Vector2.one;

        objName = this.Q<TextField>(name: "ObjName");

        dropDown = this.Q<ClassDropDown>(name: "Generator");
        dropDown.Label = "Gennerator";
        dropDown.Type = typeof(Generator);

        destroyPrev = this.Q<Toggle>(name: "DestroyPrev");

        action = this.Q<Button>(name: "Action");

        action.clicked += Execute;
    }

    public void Execute()
    {
        if (destroyPrev.value)
        {
            var prev = GameObject.Find(objName.value);
            if(prev != null)
            {
                GameObject.Destroy(prev);
            }
        }

        if(!generator.GetType().Name.Equals(dropDown.Value))
        {
            generator = dropDown.GetChoiceInstance() as Generator;
        }

        generator.ObjName = objName.value;
        generator.Position = position.value;
        generator.Scale = scale.value;

        generator.Generate(layer);
    }


}
