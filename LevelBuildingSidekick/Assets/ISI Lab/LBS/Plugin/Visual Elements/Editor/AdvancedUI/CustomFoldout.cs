using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomFoldout : Foldout
{
    public new class UxmlFactory : UxmlFactory<CustomFoldout, UxmlTraits>
    {
        public override VisualElement Create(IUxmlAttributes bag, CreationContext cc)
        {
            var instance = base.Create(bag, cc) as CustomFoldout;
            var ve = instance.Q<VisualElement>(name: "unity-checkmark").parent;

            instance.icon = new VisualElement() { name = "Icon" };
            instance.icon.style.width = instance.icon.style.height = 12;
            instance.icon.style.marginRight = 6;

            bag.TryGetAttributeValue("Icon-Path", out string path);
            var img = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            instance.icon.style.backgroundImage = img;

            //ve.Add(instance.icon);

            ve.Insert(1, instance.icon);
            ve.style.alignItems = Align.Center;



            return instance;
        }
    }

    public new class UxmlTraits : BindableElement.UxmlTraits
    {
        private readonly UxmlStringAttributeDescription m_icon = new UxmlStringAttributeDescription { name = "Icon-Path", defaultValue = "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/Logo.png" };
        private readonly UxmlStringAttributeDescription m_text = new UxmlStringAttributeDescription { name = "Text", defaultValue = "Foldout" };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);

            CustomFoldout instance = (CustomFoldout)ve;

            var path = m_icon.GetValueFromBag(bag, cc);
            var img = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            instance.icon.style.backgroundImage = img;

            instance.text = m_text.GetValueFromBag(bag, cc);
        }
    }

    public VisualElement icon = new VisualElement() { name = "Icon" };


}
