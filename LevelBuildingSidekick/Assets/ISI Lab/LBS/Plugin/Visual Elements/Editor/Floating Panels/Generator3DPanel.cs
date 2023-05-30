using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Generator;
using LBS.Components;
using LBS;
using System;
using UnityEditor;

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

    public Action OnExecute;

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

        position = this.Q<Vector3Field>(name: "Position");

        resize = this.Q<Vector2Field>(name: "Resize");
        resize.value = Vector2.one;

        scale = this.Q<Vector2Field>(name: "ReferenceSize");
        scale.value = new Vector2(2, 2);

        objName = this.Q<TextField>(name: "ObjName");

        dropDown = this.Q<ClassDropDown>(name: "Generator");

        dropDown.label = "Gennerator";
        dropDown.Type = typeof(Generator3D);

        destroyPrev = this.Q<Toggle>(name: "DestroyPrev");

        action = this.Q<Button>(name: "Action");
        action.clicked += OnExecute;
        action.clicked += Execute;
    }

    public void Init(LBSLayer layer)
    {
        this.layer = layer;

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
        if(dropDown.index < 0)
        {
            Debug.LogWarning("[ISI LAB]: No has seleccionado un tipo de generador.");
            return;
        }

        if(this.layer == null)
        {
            Debug.LogError("[ISI LAB]: no se tiene referencia de ninguna layer para generar.");
            return;
        }

        if (destroyPrev.value)
        {
            var prev = GameObject.Find(objName.value);
            if(prev != null)
            {
                GameObject.DestroyImmediate(prev);
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

        var obj = generator.Generate(this.layer);
        Undo.RegisterCreatedObjectUndo(obj, "Create my GameObject");
    }


}
