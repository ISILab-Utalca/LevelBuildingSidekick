using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ClassFoldout : Foldout
{
    public new class UxmlFactory : UxmlFactory<ClassFoldout, UxmlTraits>
    {
        public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
        {
            var instance = base.Create(bag, cc) as ClassFoldout;
            var ve = instance.Q<VisualElement>(name: "unity-checkmark").parent;

            ve.style.flexShrink = 1;

            instance.icon = new VisualElement() { name = "Icon" };
            instance.dropdown = new ClassDropDown() { name = "ClassDropDown" };
            instance.content = new VisualElement() { name = "Content" };
            instance.content.style.flexGrow = 1;

            instance.icon.style.width = instance.icon.style.height = 12;
            instance.icon.style.minHeight = instance.icon.style.minWidth = 12;
            instance.icon.style.marginRight = 6;
            ve.style.alignItems = Align.Center;

            instance.dropdown.Children().ToList()[0].style.marginLeft = 0;
            instance.dropdown.SetMargins(0);
            instance.dropdown.SetPaddings(0);

            bag.TryGetAttributeValue("Icon-Path", out string path);
            var img = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            instance.icon.style.backgroundImage = img;

            if (img == null)
                instance.icon.style.display = DisplayStyle.None;

            ve.Add(instance.icon);


            instance.dropdown.style.marginRight = 4;
            instance.dropdown.style.flexShrink = 1;
            instance.dropdown.style.flexGrow = 1;
            //instance.dropdown.parent.style.flexWrap = Wrap.Wrap;

            if (bag.TryGetAttributeValue("Text", out string label))
                instance.dropdown.label = label;

            ve.Add(instance.dropdown);

            return instance;
        }
    }

    public new class UxmlTraits : BindableElement.UxmlTraits
    {
        private readonly UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription { name = "Text", defaultValue = "ClassDropDown" };
        private readonly UxmlStringAttributeDescription m_icon = new UxmlStringAttributeDescription { name = "Icon-Path", defaultValue = "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/Logo.png" };
        //private readonly UxmlStringAttributeDescription m_type = new UxmlStringAttributeDescription { name = "DropDown Type", defaultValue = "Type" };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            ClassFoldout instance = (ClassFoldout)ve;

            instance.dropdown.label = m_Label.GetValueFromBag(bag, cc);

            var path = m_icon.GetValueFromBag(bag, cc);
            var img = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            instance.icon.style.backgroundImage = img;
        }
    }


    public VisualElement icon = new VisualElement() { name = "Icon" };
    public ClassDropDown dropdown = new ClassDropDown() { name = "ClassDropDown" };
    public VisualElement content = new VisualElement();

    object data = null;

    public new string text
    {
        get => dropdown?.label;
        set
        {
            if(dropdown != null)
                dropdown.label = value; 
        }
    }

    public object Data => data;
    public Action OnSelectChoice;

    public Type Type
    {
        get => dropdown.Type;
        set => dropdown.Type = value;
    }

    public ClassFoldout()
    {
    }

    public ClassFoldout(Type type) : this()
    {
        this.dropdown.Type = type;
    }

}

/*
public class  ClassFoldout : VisualElement
{
    public ClassFoldout() 
    {
        
    }
}*/