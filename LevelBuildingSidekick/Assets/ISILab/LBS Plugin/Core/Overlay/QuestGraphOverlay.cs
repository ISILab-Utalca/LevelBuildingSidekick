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
using UnityEditor.Experimental.GraphView;
using LBS.Manipulators;
using LBS.VisualElements;

namespace LBS.Overlays
{
    // (??) OLD
    [Overlay(typeof(QuestWindow), ID, "Tools", "GraphOverlayUSS", defaultDisplay = true)]
    public class QuestGraphOverlay : Overlay // esta corresponderia a todas las funciones bases del derivados del graphview.
    {
        private const string ID = "QuestGraphOverlayTools";
         
        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();

            var btnGroup = new ButtonGroup();
            {
                var allMode = new PresedBtn();
                {
                    var wnd = EditorWindow.GetWindow<QuestWindow>();
                    allMode.clicked += () => wnd.MainView.SetBasicManipulators();
                    allMode.text = "Handling mode";
                }
                btnGroup.Add(allMode);


                var b = new Box();
                b.style.minHeight = 10;
                btnGroup.Add(b);

                var delete = new PresedBtn();
                {
                    delete.clicked += () =>
                    {
                        var wnd = EditorWindow.GetWindow<QuestWindow>();
                        var c = wnd.GetController<LBSQuestGraphController>();
                        wnd.MainView.SetManipulator(new DeleteNodeManipulator(wnd,c));
                    };
                    delete.text = "Delete mode";
                }
                btnGroup.Add(delete);

                var addNode = new PresedBtn();
                {
                    addNode.clicked += () => {
                        var wnd = EditorWindow.GetWindow<QuestWindow>();
                        var c = wnd.GetController<LBSQuestGraphController>();
                        wnd.MainView.SetManipulator(new AddNodeManipulator(wnd,c)); 
                    };
                    addNode.text = "Add node mode";
                }
                btnGroup.Add(addNode);

                var connect = new PresedBtn();
                {
                    connect.clicked += () =>
                    {
                        var wnd = EditorWindow.GetWindow<QuestWindow>();
                        var c = wnd.GetController<LBSQuestGraphController>();
                        wnd.MainView.SetManipulator(new ConnectionManipulator(wnd,c));
                    };
                    connect.text = "Connect mode";
                }
                btnGroup.Add(connect);

                var expand = new PresedBtn();
                {
                    expand.clicked += () =>
                    {
                        var wnd = EditorWindow.GetWindow<QuestWindow>();
                        var c = wnd.GetController<LBSQuestGraphController>();
                        wnd.MainView.SetManipulator(new ExpandManipulator(wnd, c));
                    };
                    expand.text = "Expand mode";
                }
                btnGroup.Add(expand);

                var open = new PresedBtn();
                {
                    open.clicked += () =>
                    {
                        var wnd = EditorWindow.GetWindow<QuestWindow>();
                        var c = wnd.GetController<LBSQuestGraphController>();
                        wnd.MainView.SetManipulator(new OpenQuestNodeManipulator(wnd, c));
                    };
                    open.text = "Open Node";
                }
                btnGroup.Add(open);
            }
            btnGroup.Init();
            root.Add(btnGroup);

            return root;
        }
    }

}