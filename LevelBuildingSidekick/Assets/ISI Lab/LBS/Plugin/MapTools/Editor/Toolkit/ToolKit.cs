using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using System.Linq;
using ISILab.Commons.Utility;
using ISILab.LBS.Settings;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Editor;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.VisualElements;
using ISILab.LBS.Manipulators;
using LBS.Components;

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
        #endregion
        
        #region FIELDS
        private Dictionary<Type, (LBSTool, ToolButton)> tools = new();
        private (LBSTool, ToolButton) current;
        private bool Initialized;
        private Color baseColor = new Color(72f / 255f, 72f / 255f, 72f / 255f);
        private int index = 0;
        private int choiceCount = 0;

        private VisualElement content;
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
        
        #region CONSTRUCTOR
        public ToolKit()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("ToolKit");
            visualTree.CloneTree(this);
            content = this.Q<VisualElement>("Content");

            if (instance != this)
                instance = this;
        }
        #endregion

        #region METHODS
        
        #region INITS
        public void Init(LBSLayer layer)
        {
            InitGeneralTools(layer);
            AddSeparator();

            InitBehavioursTools(layer);
            AddSeparator();

            InitAssistantsTools(layer);
        }
        
        private void InitGeneralTools(LBSLayer layer)
        {
            var entry = GetTool(typeof(Select));
            LBSTool toolInstance;
            
            if (entry.Key == null || entry.Value.Item1 == null)
            {
                toolInstance = new LBSTool(new Select());
                AddTool(toolInstance);
            }
            else
            {
                toolInstance = entry.Value.Item1;
            }
            
            toolInstance.Init(layer, this);
            toolInstance.OnSelect += LBSInspectorPanel.ActivateDataTab;
        }
        
        public void InitBehavioursTools(LBSLayer layer)
        {
            if (layer == null) return;
            
            var editorTypes = Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                .Where(t => t.Item2.Any(v => v.type != null))
                .Select(t => new
                {
                    EditorType = t.Item1,
                    BehaviorType = t.Item2.First(v => v.type != null).type // Safely select first non-null type
                })
                .Where(x => typeof(IToolProvider).IsAssignableFrom(x.EditorType)) // Only IToolProvider editors
                .ToList();

            // Cache matching editors for the layer
            var customEditors = Enumerable.OfType<LBSCustomEditor>(LBSInspectorPanel.Instance.behaviours.CustomEditors)
                .Where(e => e.Target is LBSBehaviour lb && Equals(lb.OwnerLayer, layer))
                .ToList();

            foreach (var behaviour in layer.Behaviours)
            {
                if (behaviour == null) continue;

                var behaviourType = behaviour.GetType();

                // Find the first editor type matching the behavior
                var editorType = editorTypes
                    .FirstOrDefault(x => x.BehaviorType == behaviourType)?.EditorType;

                if (editorType == null) continue;

                // Find the first matching editor instance
                var editor = customEditors
                    .FirstOrDefault(e => e.GetType() == editorType && e.Target == behaviour);

                if (editor == null) continue;

                // Update the editor and set tools
                editor.SetInfo(behaviour);
                if (editor is IToolProvider toolProvider)
                {
                    toolProvider.SetTools(this);
                }
            }
        }

        private void InitAssistantsTools(LBSLayer layer)
        {
            foreach (var assist in layer.Assistants)
            {
                var type = assist.GetType();
                var customEditors = Reflection.GetClassesWith<LBSCustomEditorAttribute>()
                    .Where(t => t.Item2.Any(v => v.type == type)).ToList();

                if (!customEditors.Any())
                    return;

                var customEditor = customEditors.First().Item1;
                var i = customEditor.GetInterface(nameof(IToolProvider));

                if (i != null)
                {
                    var ve = LBSInspectorPanel.Instance.assistants.CustomEditors.First(x => x.GetType() == customEditor);
                    ve.SetInfo(assist);
                    ((IToolProvider)ve).SetTools(this);
                }
            }
        }
        
        #endregion
        
        public object GetActiveManipulator()
        {
            return content;
        }
        private KeyValuePair<Type, (LBSTool, ToolButton)> GetTool(Type manipulatorType)
        {
            // Find the first matching tool in the dictionary with all null checks
            var foundTool = tools.FirstOrDefault(kvp =>
                kvp is { Key: not null, Value: { Item1: { Manipulator: not null } } } &&
                kvp.Value.Item1.Manipulator.GetType() == manipulatorType);

            return foundTool;
        }
        
        public void SetActive(Type manipulatorType)
        {
            // Ensure manipulatorType is not null
            if (manipulatorType == null)
            {
                Debug.LogWarning("Manipulator type is null.");
                return;
            }

            // Find the first matching tool in the dictionary
            var foundTool = GetTool(manipulatorType);

            if (foundTool.Key == null)
            {
                Debug.LogWarning($"Tool of type {manipulatorType.Name} not found.");
                return;
            }
        
            // If another tool was active, blur it
            current.Item2?.OnBlur();

            // Set the new current tool and focus it
            current = foundTool.Value;
            current.Item2?.OnFocus();

            // Activate its manipulator
            var manipulator = current.Item1.Manipulator;
            MainView.Instance.AddManipulator(manipulator);

            // Notify
            manipulator.OnManipulationNotification += () =>
            {
                LBSMainWindow.MessageManipulator(manipulator.Description);
            };
            manipulator.OnManipulationNotification?.Invoke();
            
        }
        
        public void AddSeparator(int height = 10)
        {
            var separator = new VisualElement
            {
                style =
                {
                    height = height
                }
            };
            content.Add(separator);
        }

        public void AddTool(LBSTool tool)
        {
            var button = new ToolButton(tool);
            tool.BindButton(button);
            content.Add(button);

            tools[tool.Manipulator.GetType()] = (tool, button);

            button.AddGroupEvent(() => SetActive(tool.Manipulator.GetType()));
            button.SetColorGroup(LBSSettings.Instance.view.toolkitNormal, LBSSettings.Instance.view.newToolkitSelected);
            
            SetUpAdderRemover(tool);

            tool.OnStart += (l) => { OnStartAction?.Invoke(l); };
            tool.OnEnd += (l) => { OnEndAction?.Invoke(l); };
        }

        private void SetUpAdderRemover(LBSTool tool)
        {
            // For tools that add
            if (tool.Manipulator.Remover != null)
            {
                // Right-clicking removes
                tool.Manipulator.OnManipulationRightClick += () =>
                {
                    // Use GetTool to find the matching Remover tool safely
                    var removerTool = GetTool(tool.Manipulator.Remover.GetType());

                    if (removerTool.Key != null)
                    {
                        // Set the tool as active and update the manipulator
                        SetActive(removerTool.Key);
                        MainView.Instance.SetManipulator(tool.Manipulator.Remover, true);
                    }
                };
            }
            // For tools that remove
            else if (tool.Manipulator.Adder != null)
            {
                // Once it was used via right-click, go back to its corresponding add tool
                tool.Manipulator.OnManipulationRightClickEnd += () =>
                {
                    // Use GetTool to find the matching Adder tool safely
                    var adderTool = GetTool(tool.Manipulator.Adder.GetType());

                    if (adderTool.Key != null)
                    {
                        // Set the tool as active and update the manipulator
                        SetActive(adderTool.Key);
                        MainView.Instance.SetManipulator(tool.Manipulator.Adder, true);
                    }
                };
            }
        }

        public new void Clear()
        {
            if (!tools.Any()) return;
            current.Item2?.OnBlur();
            tools.Clear();
            content.Clear();
        }
        
        #endregion
    }
}