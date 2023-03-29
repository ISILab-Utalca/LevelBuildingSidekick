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

            ve.style.flexWrap = Wrap.Wrap;

            instance.icon = new VisualElement() { name = "Icon" };
            instance.dropdown = new ClassDropDown() { name = "ClassDropDown" };

            instance.icon.style.width = instance.icon.style.height = 20;

            bag.TryGetAttributeValue("Icon-Path", out string path);
            var img = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            instance.icon.style.backgroundImage = img;

            if (img == null)
                instance.icon.style.display = DisplayStyle.None;

            ve.Add(instance.icon);
            instance.dropdown.style.paddingLeft = 4;
            instance.dropdown.style.flexGrow = instance.dropdown.style.flexShrink = 1;
            //instance.dropdown.parent.style.flexWrap = Wrap.Wrap;

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

            if (img == null)
                instance.icon.style.display = DisplayStyle.None;
        }
    }


    public VisualElement icon = new VisualElement() { name = "Icon" };
    public ClassDropDown dropdown = new ClassDropDown() { name = "ClassDropDown" };
    //public VisualElement content = new VisualElement();

    
    public new string text
    {
        get => dropdown?.label;
        set
        {
            if(dropdown != null)
                dropdown.label = value; 
        }
    }

    public ClassFoldout()
    {
        dropdown.RegisterValueChangedCallback(UpdateView);
    }

    public ClassFoldout(Type type) : this()
    {

    }

    private void UpdateView(ChangeEvent<string> e)
    {
        var type = Utility.Reflection.GetType(e.newValue);

        if(type == null)
        {
            throw new Exception("[ISI Lab] Class type not found");
        }

        var ves = Utility.Reflection.GetClassesWith<CustomVisualElementAttribute>().Where(t => t.Item2.Any(v => v.type == type));

        if(ves.Count() == 0)
        {
            throw new Exception("[ISI Lab] No class marked as CustomVisualElement found for type: " + type);
        }

        var ve = Activator.CreateInstance(ves.First().Item1, new object[] { dropdown.GetChoiceInstance() });

        if (!(ve is VisualElement))
        {
            throw new Exception("[ISI Lab] " + ve.GetType().GetType() +" is not a VisualElement ");
        }

        contentContainer.Add(ve as VisualElement);
    }

}
