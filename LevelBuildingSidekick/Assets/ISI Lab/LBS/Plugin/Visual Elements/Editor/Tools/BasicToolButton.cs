using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BasicToolButton : VisualElement, IGrupable
{
    public Color color = new Color(72 / 255f, 72 / 255f, 72 / 255f);
    public Color selected = new Color(207 / 255f, 100 / 255f, 29 / 255f);

    private Button button;
    private VisualElement icon;
    private Label label;

    // Event
    public event Action OnFocus; // se llama cuando se selecciona el boton
    public event Action OnBlur; // se llama cuando se dejo de seleccionar el boton
   
    public BasicToolButton(Texture2D texture, string name)
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("BasicToolButton"); // Editor
        visualTree.CloneTree(this);

        // Button
        button = this.Q<Button>("Button");
        //button.clicked += OnFocus;

       // Icon
       icon = this.Q<VisualElement>("Icon");
        icon.style.backgroundImage = texture;

        // LabelName
        label = this.Q<Label>("LabelName");
        label.text = name;
    }

    public void AddEvent(Action action)
    {
        button.clicked += action;
    }

    void IGrupable.OnBlur()
    {
        button.style.backgroundColor = color;
        OnBlur?.Invoke();
    }

    void IGrupable.OnFocus()
    {
        button.style.backgroundColor = selected;
        OnFocus?.Invoke();
    }
}

