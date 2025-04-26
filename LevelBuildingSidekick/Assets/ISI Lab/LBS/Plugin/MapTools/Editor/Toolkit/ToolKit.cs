using UnityEngine;
using UnityEngine.UIElements;

using System.Collections.Generic;
using System.Linq;
using System;

using ISILab.LBS.Settings;
using LBS.Components;
using ISILab.Commons.Utility;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.VisualElements;
using ISILab.LBS.Manipulators;
using ISILab.LBS;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Editor;
using ISILab.LBS.Editor.Windows;

namespace LBS.VisualElements
{
    [UxmlElement]
    public partial class ToolKit : VisualElement
    {
        #region FACTORY
        //public new class UxmlFactory : UxmlFactory<ToolKit, UxmlTraits> { }
        
        UxmlColorAttributeDescription m_BaseColor = new UxmlColorAttributeDescription
        {
            name = "base-color",
            defaultValue = new Color(72f / 255f, 72f / 255f, 72f / 255f)
        };

        UxmlColorAttributeDescription m_SelectedColor = new UxmlColorAttributeDescription
        {
            name = "selected-color",
    
            defaultValue = new Color(215f / 255f, 127f / 255f, 45f / 255f)
        };

        UxmlIntAttributeDescription m_Index = new UxmlIntAttributeDescription
        {
            name = "index",
            defaultValue = 0
        };

        public IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get
            {
                yield return new UxmlChildElementDescription(typeof(VisualElement));
            }
        }

        public void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            Init(ve, bag, cc);
            var btn = ve as ToolKit;

            btn.BaseColor = m_BaseColor.GetValueFromBag(bag, cc);
            btn.Index = m_Index.GetValueFromBag(bag, cc);
        }
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
        private List<(LBSTool,ToolButton)> tools = new();
        private (LBSTool,ToolButton) current;

        private bool Initialized;
        
        private Color baseColor = new Color(72f / 255f, 72f / 255f, 72f / 255f);
        private int index = 0;
        private int choiceCount = 0;
        #endregion

        #region FIELDS VIEW
        private VisualElement content;
        #endregion

        #region PROPERTIES
        public Color BaseColor
        {
            get => baseColor;
            set => baseColor = value;
        }

        public int Index
        {
            get => index;
            set => index = value;
        }

        public int ChoiceCount
        {
            get => choiceCount;
            set => choiceCount = value;
        }
        #endregion

        #region EVENTS
        public event Action<LBSLayer> OnEndAction;
        public event Action<LBSLayer> OnStartAction;
        #endregion

        #region CONSTRUCTORS
        public ToolKit()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("ToolKit");
            visualTree.CloneTree(this);

            this.content = this.Q<VisualElement>("Content");

            // Singleton
            if (instance != this)
                instance = this;
        }
        #endregion

        #region METHODS
        public void Init(LBSLayer layer)
        {
            InitGeneralTools(layer);
            this.AddSeparator();

            InitBehavioursTools(layer);
            this.AddSeparator();

            InitAssistantsTools(layer);
        }
        
        private void InitGeneralTools(LBSLayer layer)
        {
            var t1 = TryGetTool("Select");
            if (t1 == null)
            {
                var icon = Resources.Load<Texture2D>("Icons/Select");
                var selectTool = new Select();
                t1 = new LBSTool(icon, "Select", "Selection",  selectTool);
            }
            t1.Init(layer, this);
            t1.OnSelect += () =>
            {
                LBSInspectorPanel.ShowInspector("Current data");
            };
            AddTool(t1);

        }

        LBSTool TryGetTool(string toolName)
        {
            foreach (var element in tools)
            {
                if(element.Item1 == null) continue;
                if (element.Item1.Name == toolName) return element.Item1;
            }
            return null;
        }

        public void InitBehavioursTools(LBSLayer layer)
        {
            if (layer==null) return;
            foreach (var behaviour in layer.Behaviours)
            {
                var type = behaviour.GetType();
                var customEditors = Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                    .Where(t => t.Item2.Any(v => v.type == type)).ToList();

                if (!customEditors.Any())
                    return;

                var customEditor = customEditors.First().Item1;
                var i = customEditor.GetInterface(nameof(IToolProvider));

                if (i == null) continue;
                
                var matchingEditors = LBSInspectorPanel.Instance.behaviours.CustomEditors
                    .Where(e => e.Target is LBSBehaviour lb && e.GetType() == customEditor && lb.OwnerLayer == layer)
                    .ToList();

               // foreach (var editor in matchingEditors)
               // {
                    var editor = matchingEditors.First();  
                    var lb = editor.Target as LBSBehaviour;
                    if (lb == null) continue;

                    Debug.Log($"Comparing OwnerLayer: {lb.OwnerLayer.ID} with Layer: {layer.ID}");

                    if (Equals(lb.OwnerLayer, layer))
                    {
                        Debug.Log($"MATCH: {lb.OwnerLayer.ID} belongs to layer {layer.ID}");
                        // Do something with matching editor
                        var ve = editor as LBSCustomEditor;
                        ve?.SetInfo(behaviour);
                        if (ve is IToolProvider toolProvider)
                            toolProvider.SetTools(this);
                        
                    }
                //}
             
                /*
              var ve = LBSInspectorPanel.Instance.behaviours.CustomEditors.First( x => x.GetType() == customEditor);
                ve.SetInfo(behaviour);
                if (ve is IToolProvider toolProvider) 
                    toolProvider.SetTools(this);
                }*/
            }
        }

        private void InitAssistantsTools(LBSLayer layer)
        {
            foreach (var assist in layer.Assistants)
            {
                var type = assist.GetType();
                var customEditors = Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                    .Where(t => t.Item2.Any(v => v.type == type)).ToList();

                if (customEditors.Count() == 0)
                    return;

                var customEditor = customEditors.First().Item1;
                var i = customEditor.GetInterface(typeof(IToolProvider).Name);

                if (i != null)
                {
                    var ve = LBSInspectorPanel.Instance.assistants.CustomEditors.First(x => x.GetType() == customEditor);
                    ve.SetInfo(assist);
                    ((IToolProvider)ve).SetTools(this);
                }
            }
        }

        public void SetActive(int index)
        {
            this.index = index;

            if(current != default((LBSTool,ToolButton)))
                current.Item2.OnBlur();
            
            current = tools[index];

            current.Item2.OnFocus();

            var m = current.Item1.Manipulator;
            MainView.Instance.AddManipulator(m);
            
            m.OnManipulationNotification += () =>
            {
                LBSMainWindow.MessageManipulator(tools.ElementAtOrDefault(index).Item1.Description);
  
            };
            m.OnManipulationNotification?.Invoke();
        }

        public void SetActive(LBSManipulator manipulator)
        {
            foreach (var element in tools)
            {
                if (element.Item1.Manipulator == manipulator)
                {
                    SetActive(manipulator);
                }
            }
        }
        
        public void SetActiveWhithoutNotify(int index)
        {
            this.index = index;
            current = tools[index];
            current.Item2.OnFocusWithoutNotify();

            var m = current.Item1.Manipulator;
            MainView.Instance.AddManipulator(m);
            
            m.OnManipulationNotification += () =>
            {
                LBSMainWindow.MessageManipulator(tools.ElementAtOrDefault(index).Item1.Description);
  
            };
            m.OnManipulationNotification?.Invoke();
        }

        public void SetActive(string value)
        {
            var index = tools.FindIndex(t => t.Item2.tooltip.Equals(value));
            SetActive(index);
       //     if(tools.Count >= index && tools[index].Item1 != null) Debug.Log(tools[index].Item1.Name);
        }

        public void SetActive()
        {
            if (tools.Count <= 0)
                return;
        }

        public void AddTool(LBSTool tool, int index = -1)
        {
            var button = new ToolButton(tool);
            (LBSTool, ToolButton) t = new(tool, button);
            tool.BindButton(button);

            this.content.Add(button);
            tools.Add(t);

            var i = tools.Count - 1;
            button.AddGroupEvent(() =>
            {
                var index = i;
                SetActive(index);
            });
            button.SetColorGroup(baseColor, LBSSettings.Instance.view.toolkitSelected);
            
            // for tools that add
            if (tool.Manipulator.Remover != null)
            {
                // right clicking removes
                tool.Manipulator.OnManipulationRightClick += () =>
                {
                    foreach ((LBSTool toolItem, ToolButton button) tuple in tools)
                    {
                        if (tuple.toolItem.Manipulator == tool.Manipulator.Remover)
                        {
                            int index = tools.FindIndex(t => t.Item1.Manipulator == tool.Manipulator.Remover);
                            SetActive(index);
                            MainView.Instance.SetManipulator(tool.Manipulator.Remover, true);
                            break; 
                        }
                    }
                };
            }
            // for tools that remove
            else if (tool.Manipulator.Adder != null)
            {
                // once it was used via right click go back to its corresponding add tool
                tool.Manipulator.OnManipulationRightClickEnd += () =>
                {
                    foreach ((LBSTool toolItem, ToolButton button) tuple in tools)
                    {
                        if (tuple.toolItem.Manipulator == tool.Manipulator.Adder)
                        {
                            int index = tools.FindIndex(t => t.Item1.Manipulator == tool.Manipulator.Adder);
                            SetActive(index);
                            MainView.Instance.SetManipulator(tool.Manipulator.Adder, true);
                            break; 
                        }
                    }
                };
            }
            
            tool.OnStart += (l) => { OnStartAction?.Invoke(l); };
            tool.OnEnd += (l) => { OnEndAction?.Invoke(l); };
            

        }

        public void AddSeparator(int height = 10)
        {
            var separator = new VisualElement();
            separator.style.height = height;
            content.Add(separator);
        }

        public new void Clear()
        {
            if (tools.Count <= 0)
                return;

            current.Item2?.OnBlur();
            tools.Clear();
            content.Clear();
        }
        #endregion


    }
}
