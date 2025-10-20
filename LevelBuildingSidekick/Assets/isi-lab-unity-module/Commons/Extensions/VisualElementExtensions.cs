using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


// Extensions are always static classes
namespace ISILab.Extensions 
{
    public static class VisualElementExtensions
    {
        public static void SetDisplay(this VisualElement element, bool value)
        {
            if (element == null) return;
            element.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;
           // element.style.display = DisplayStyle.None;
            /* previous way to show the corresponding ortogonal part of the connection tile 
            in the SchemaTileView uxml
            
                
            */
        }
        
        public static bool GetDisplay(this VisualElement element)
        {
            return element.style.display == DisplayStyle.Flex;
        }


        public static void SetBackgroundColor(this VisualElement element, Color color){
            element.style.backgroundColor = color;
        }

        public static void SetMargins(this VisualElement element, int value)
        {
            element.style.marginBottom = value;
            element.style.marginLeft = value;
            element.style.marginRight = value;
            element.style.marginTop = value;
        }

        public static void SetPaddings(this VisualElement element, int value)
        {
            element.style.paddingBottom = value;
            element.style.paddingLeft = value;
            element.style.paddingRight = value;
            element.style.paddingTop = value;
        }

        public static void SetBorder(this VisualElement element, Color color, float value = 0f)
        {
            element.style.borderBottomColor = color;
            element.style.borderLeftColor = color;
            element.style.borderRightColor = color;
            element.style.borderTopColor = color;

            if (value >= 0)
            {
                element.style.borderBottomWidth = value;
                element.style.borderLeftWidth = value;
                element.style.borderRightWidth = value;
                element.style.borderTopWidth = value;
            }
        }

        public static void SetBorderRadius(this VisualElement element, float value)
        {
            element.style.borderBottomLeftRadius = value;
            element.style.borderBottomRightRadius = value;
            element.style.borderTopLeftRadius = value;
            element.style.borderTopRightRadius = value;
        }
    }
}