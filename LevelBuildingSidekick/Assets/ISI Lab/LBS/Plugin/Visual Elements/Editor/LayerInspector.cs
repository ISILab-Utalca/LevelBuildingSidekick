using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.VisualElements
{
    public class LayerInspector : VisualElement
    {
        private readonly string path = "Assets/ISI Lab/Commons/Assets2D/Arrow.png";
        private readonly Texture2D arrow;

        //private List<Button> buttons = new List<Button>();
        //private List<VisualElement> arrows = new List<VisualElement>();

        public new class UxmlFactory : UxmlFactory<LayerInspector, UxmlTraits> { }

        public LayerInspector()
        {
            //var pre = Application.dataPath;
            arrow = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            //this.style.display = DisplayStyle.Flex;
            //SetInfo(new string[] { "Hola", "Mundo" });
        }

        public void SetInfo(string[] text)
        {
            this.Clear();

            foreach (var t in text)
            {
                var v = new Button();
                v.text = t;
                v.SetBorder(Color.red, 0);
                v.style.backgroundColor = new Color(0, 0, 0, 0);
                v.style.color = new Color(1,1,1,0.5f);

                this.Add(v);

                var a = new VisualElement();
                a.style.backgroundImage = arrow;
                a.style.width = 8;

                this.Add(a);
            }
        }
    }
}
