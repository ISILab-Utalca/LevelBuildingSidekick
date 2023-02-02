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
    Vector2Field resize;
    TextField objName;

    Generator3D generator;
    Button action;
    Toggle destroyPrev;

    LBSLayer layer;

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

        action = this.Q<Button>(name: "Action");
        action.clicked += Execute;
    }

    public Generator3DPanel(LBSLayer layer)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("Generator3DPanel"); // Editor
        visualTree.CloneTree(this);

        Init(layer);


        action = this.Q<Button>(name: "Action");
        action.clicked += Execute;
    }

    public void Init(LBSLayer layer = null)
    {
        this.layer = layer;

        position = this.Q<Vector3Field>(name: "Position");

        scale = this.Q<Vector2Field>(name: "Resize");
        scale.value = Vector2.one;

        resize = this.Q<Vector2Field>(name: "ReferenceSize");
        resize.value = Vector2.one;

        objName = this.Q<TextField>(name: "ObjName");

        dropDown = this.Q<ClassDropDown>(name: "Generator");
        dropDown.Label = "Gennerator";
        dropDown.Type = typeof(Generator3D);

        destroyPrev = this.Q<Toggle>(name: "DestroyPrev");

        if(layer != null)
        {
            generator = layer.Assitant.Generator;
            if(generator != null)
            {
                dropDown.Value = generator.GetType().Name;
                scale.value = generator.Resize;
                position.value = generator.Position;
                objName.value = generator.ObjName;
                resize.value = generator.Resize;
            }
        }

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

        if(generator == null || !generator.GetType().Name.Equals(dropDown.Value))
        {
            generator = dropDown.GetChoiceInstance() as Generator3D;
        }

        generator.ObjName = objName.value;
        generator.Position = position.value;
        generator.Resize = resize.value;
        generator.Scale = scale.value;

        generator.Generate(layer);

    }


}
