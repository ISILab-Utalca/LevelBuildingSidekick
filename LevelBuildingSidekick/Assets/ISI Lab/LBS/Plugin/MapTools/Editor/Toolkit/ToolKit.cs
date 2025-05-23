using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using System.Linq;
using ISILab.LBS.Settings;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Assistants;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Editor;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.VisualElements;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Modules;
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
        private Color baseColor = new(72f / 255f, 72f / 255f, 72f / 255f);
        private int index;
        private int choiceCount;

        private VisualElement content;
        private List<VisualElement> separators = new();

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

            if (!Equals(instance, this))
                instance = this;
    
        }
        #endregion

        #region METHODS
        
        public void InitGeneralTools(LBSLayer layer)
        { 
            LBSTool selectTool = new LBSTool(new Select());
            ActivateTool(selectTool,layer);
            selectTool.Init(layer, this);
            selectTool.OnSelect += LBSInspectorPanel.ActivateDataTab;
        }
        
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
        
        private void ClearSeparators()
        {
            foreach (var separator in separators)
            {
                separator.style.display = DisplayStyle.None;
            }
            separators.Clear();
        }

        public void ActivateTool(LBSTool tool, LBSLayer layer, object provider = null)
        {
            if(tool == null) return;
            
            AddTool(tool);
            tool.Init(layer, provider);

        }

        private void AddTool(LBSTool tool)
        {
            var button = new ToolButton(tool);
            tool.BindButton(button);
            content.Add(button);
            tools[tool.Manipulator.GetType()] = (tool, button);

            button.AddGroupEvent(() => SetActive(tool.Manipulator.GetType()));
            button.SetColorGroup(LBSSettings.Instance.view.toolkitNormal, LBSSettings.Instance.view.newToolkitSelected);
            
            SetUpAdderRemover(tool);

            tool.OnStart += (_) => { OnStartAction?.Invoke(tool.Manipulator.Layer); };
            tool.OnEnd += (_) => { OnEndAction?.Invoke(tool.Manipulator.Layer); };
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
        
        public void SetTarget(LBSCustomEditor editor)
        {
            if (editor is IToolProvider toolProvider)
            {
                toolProvider.SetTools(this);
            }
        }
        
        public new void Clear()
        {
            if (!tools.Any()) return;
            current.Item2?.OnBlur();
            foreach ((LBSTool, ToolButton) toolPair in tools.Values)
            {
                toolPair.Item2.style.display = DisplayStyle.None;
            }
            ClearSeparators();
        }
        
        #endregion


        public void SetSeparators()
        {
            ClearSeparators();
            if (tools == null || tools.Count == 0)
                return;
            
            Dictionary<Type, List<ToolButton>> groupedButtons = new();

            foreach ((LBSTool tool, ToolButton button) in tools.Values)
            {
                if (button == null || button.style.display == DisplayStyle.None)
                    continue;

                Type type = tool?.Manipulator?.ObjectType;
                if(type is null) continue;
                
                if (!groupedButtons.ContainsKey(type))
                    groupedButtons[type] = new List<ToolButton>();

                groupedButtons[type].Add(button);
            }

            // presets in desired order!
            List<Type> presentTypes = new()
            {
                typeof(VisualElement),
                typeof(LBSModule),
                typeof(LBSBehaviour),
                typeof(LBSAssistant)
            };
            
            List<ToolButton> lastButtonPerType = new();
            for (int i = 0; i < presentTypes.Count - 1; i++)
            {
                if (groupedButtons.TryGetValue(presentTypes[i], out var buttons) && buttons.Count > 0)
                {
                    lastButtonPerType.Add(buttons.Last());
                }
            }
            
            foreach (var button in lastButtonPerType)
            {
                InsertSeparatorAfter(button);
            }
            
            MarkDirtyRepaint();
        }
        
        private void InsertSeparatorAfter(VisualElement element)
        {
            var separator = new VisualElement();
            separator.style.height = 1;
            separator.style.marginTop = 4;
            separator.style.marginBottom = 4;
            separator.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            separator.style.flexGrow = 1;

            var parent = element.parent;
            if (parent == null) return;

            int index = parent.IndexOf(element);
            if (index >= 0)
            {
                parent.Insert(index + 1, separator);
            }
            
            separators.Add(separator);
        }
    }
}