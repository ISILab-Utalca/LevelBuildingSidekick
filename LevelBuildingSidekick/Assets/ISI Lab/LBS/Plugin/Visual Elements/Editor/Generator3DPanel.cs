using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Generator;
using LBS.Components;
using LBS;

public class Generator3DPanel : VisualElement
{
    ClassDropDown dropDown;
    Vector3Field position;
    Vector2Field scale;
    TextField objName;

    Generator3D generator;
    Button action;
    Toggle destroyPrev;

    public Generator3D Generator
    {
        get => generator;
        set => generator = value;
    }

    public new class UxmlFactory : UxmlFactory<Generator3DPanel, VisualElement.UxmlTraits> { }

    public Generator3DPanel() 
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Generator3DPanel"); // Editor
        visualTree.CloneTree(this);

        Init();
    }

    public Generator3DPanel(LBSLayer layer)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Generator3DPanel"); // Editor
        visualTree.CloneTree(this);

        Init(layer);
    }

    public void Init(LBSLayer layer = null)
    {
        position = this.Q<Vector3Field>(name: "Position");

        scale = this.Q<Vector2Field>(name: "Scale");
        scale.value = Vector2.one;

        objName = this.Q<TextField>(name: "ObjName");

        dropDown = this.Q<ClassDropDown>(name: "Generator");
        dropDown.Label = "Gennerator";
        dropDown.Type = typeof(Generator3D);

        destroyPrev = this.Q<Toggle>(name: "DestroyPrev");

        action = this.Q<Button>(name: "Action");

        action.clicked += () => Execute(layer);

        if(layer != null)
        {
            generator = layer.Assitant.Generator;
            if(generator != null)
            {
                dropDown.Value = generator.GetType().Name;
                scale.value = generator.Scale;
                position.value = generator.Position;
                objName.value = generator.ObjName;

            }
        }

    }

    public void Execute(LBSLayer layer)
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
            generator = dropDown.GetChoiceInstance() as Generator3D;
        }

        generator.ObjName = objName.value;
        generator.Position = position.value;
        generator.Scale = scale.value;

        generator.Generate(layer);
    }


}
