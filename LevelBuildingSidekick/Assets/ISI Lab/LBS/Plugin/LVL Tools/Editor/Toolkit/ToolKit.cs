using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.VisualElements;
using System.Linq;
using System;
using UnityEngine.UIElements;
using UnityEditor;

namespace LBS.VisualElements
{
    public class ToolKit : ButtonGroup
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<ToolKit, UxmlTraits> { }
        #endregion

        #region SINGLETON
        private static ToolKit instance;
        internal static ToolKit Instance 
        {
            get
            {
                return instance;
            }
        }
        #endregion

        #region FIELDS
        private List<LBSTool> currentTools = new List<LBSTool>();
        #endregion

        #region FIELDS VIEW
        private VisualElement content;
        #endregion

        #region EVENTS
        public event Action OnEndAction;
        public event Action OnStartAction;
        #endregion

        #region CONSTRUCTORS
        public ToolKit()
        {
            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("ToolKit");
            visualTree.CloneTree(this);

            this.content = this.Q<VisualElement>("Content");

            // Singleton
            if (instance != this)
                instance = this;
        }
        #endregion

        #region METHODS
        public void AddSubTools(LBSTool[] tool, int index = -1)
        {
            throw new NotImplementedException();
        }

        public void AddTool(LBSTool tool, int index = -1)
        {
            currentTools.Add(tool);

            var button = new ToolButton(tool);
            this.Add(button);
            //this.content.Add(button);

            tool.OnStart += OnStartAction;
            tool.OnEnd += OnEndAction;

        }

        public void AddSeparator(int height = 10)
        {
            var separator = new VisualElement();
            separator.style.height = height;
            this.Add(separator);
            //this.content.Add(separator);
        }

        public new void Clear()
        {
            currentTools.Clear();
            base.Clear();
            //this.content.Clear();
        }
        #endregion
    }
}

/*
public class ToolkitManager
{
    private List<LBSTool> currentTools = new List<LBSTool>();

    // VisualElement references
    private ButtonGroup toolPanel;
    private MainView view;
    private LBSInspectorPanel InspectorManager;

    // event
    public event Action OnEndSomeAction;

    public ToolkitManager(ref ButtonGroup toolPanel, ref MainView view, ref LBSInspectorPanel inspectorManager , ref List<LayerTemplate> templates)
    {
        this.toolPanel = toolPanel;
        this.view = view;
        this.InspectorManager = inspectorManager;
    }

    public void SetTools(object tools, ref LBSLayer layer, ref LBSBehaviour behaviour)
    {
        ClearTools();

        currentTools = tools as List<LBSTool>;
        foreach (var tool in currentTools)
        {
            tool.OnEnd += OnEndSomeAction;

            var btn = tool.Init(view, layer, behaviour);
            btn.style.flexGrow = 1;
            toolPanel.Add(btn);

            if (!string.IsNullOrEmpty(tool.inspector))
            {
                var insp = tool.InitInspector(view, layer, behaviour);
                insp.style.flexGrow = 1;
                btn.OnFocusEvent += () => {
                    InspectorManager.AddInspector(insp,0); 
                };
                btn.OnBlurEvent += () => {
                    InspectorManager.RemoveInspector(insp);
                };
            }
        }
    }

    public void ClearTools()
    {
        foreach (var tool in currentTools)
        {
            tool.OnEnd -= OnEndSomeAction;
        }

        toolPanel.Clear();
    }

    internal void OnSelectedLayerChange(LBSLayer layer)
    {
        ClearTools();
        //var manipulators = layer.Behaviours.SelectMany(b => b.);
       
    }
}
*/
