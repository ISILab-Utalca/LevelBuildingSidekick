using LBS.Transformers;
using LBS.Windows;
using LBS;
using LBS.Graph;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace LBS.Overlays
{
    [Overlay(typeof(LBSGraphRCWindow), ID, "Tools", "GraphOverlayUSS", defaultDisplay = true)]
    public class GraphOverlay : Overlay // esta corresponderia a todas las funciones bases del derivados del graphview.
    {
        private const string ID = "GraphOverlayTools";
         

        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();

            var btnGroup = new ButtonGroup();
            {

                var allMode = new PresedBtn();
                {
                    var wnd = EditorWindow.GetWindow<LBSGraphRCWindow>();
                    //allMode.clicked += wnd.MainView.SetBasicManipulators;
                    allMode.text = "All mode";
                }
                btnGroup.Add(allMode);


                var select = new PresedBtn();
                {
                    select.clicked += () => Debug.Log("[Select implementar]"); //wnd.MainView.SetManipulator(wnd.MainView.rectagleSelector); // mm no me gusta (!) 
                    select.text = "Select mode";
                }
                btnGroup.Add(select);

                var delete = new PresedBtn();
                {
                    delete.clicked += () => Debug.Log("[Delete implementar]");
                    delete.text = "Delete mode";
                }
                btnGroup.Add(delete);

                var addNode = new PresedBtn();
                {
                    addNode.clicked += () =>
                    {
                        Debug.Log("[Add node implementar]");
                    };
                    addNode.text = "Add node mode";
                }
                btnGroup.Add(addNode);
            }
            btnGroup.Init();
            root.Add(btnGroup);


            var generateSchema = new PresedBtn();
            {
                generateSchema.clicked += () =>
                {
                    var controller = (this.containerWindow as LBSGraphRCWindow).GetController<LBSGraphRCController>();
                    controller.GenerateSchema();
                    LBSSchemaWindow.OpenWindow();
                };
                generateSchema.text = "Generate Schema";
            }
            root.Add(generateSchema);


            return root;
        }
    }

    public class PresedBtn : Button, IGrupable
    {
        public new class UxmlFactory : UxmlFactory<PresedBtn, UxmlTraits> { }

        public Color selected = new Color(0.35f, 0.35f, 0.35f, 1f);
        public Color unselected = new Color(0.27f, 0.38f, 0.49f, 1f);
        public Texture2D Icon;

        public void AddEvent(Action action)
        {
            this.clicked += action;
        }

        public void SetActive(bool value)
        {
            this.style.backgroundColor = !value ? selected : unselected;
        }
    }

    public interface IGrupable
    {
        public void AddEvent(Action action);

        public void SetActive(bool value);
    }

    public class ButtonGroup : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<ButtonGroup, UxmlTraits> { }

        public bool allowSwitchOff = false;
        private List<IGrupable> group = new List<IGrupable>();
        private IGrupable current;

        public ButtonGroup()
        {
            Init();
        }

        public void Init()
        {
            group = this.Query<VisualElement>().ToList().Where(ve => ve is IGrupable).Select(ve => ve as IGrupable).ToList();
            group.ForEach(b => b.AddEvent(() => Active(b)));

            if (!allowSwitchOff && group.Count > 0)
            {
                current = group[0];
                Active(current);
            }
        }

        private void Active(IGrupable active)
        {
            if (allowSwitchOff)
            {
                if (current == active)
                {
                    current = null;
                    active.SetActive(false);
                }
                else
                {
                    group.ForEach(b => b.SetActive(false));
                    current = active;
                    active.SetActive(true);
                }

            }
            else
            {
                if (current == active)
                    return;

                group.ForEach(b => b.SetActive(false));
                current = active;
                active.SetActive(true);
            }
        }

        public void Remove(IGrupable btn)
        {
            group.Remove(btn);
        }

        public void AddMember(IGrupable btn)
        {
            group.Add(btn);
        }

    }
}