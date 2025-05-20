using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;
using ISILab.Commons.Utility;
using ISILab.LBS.Settings;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS;
using ISILab.LBS.Assistants;
using ISILab.LBS.Behaviours;
using ISILab.LBS.Editor;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.VisualElements.Editor;
using ISILab.LBS.VisualElements;
using ISILab.LBS.Manipulators;
using ISILab.LBS.Template;
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
            ActivateTool(selectTool,layer,layer.Modules);
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
            separators.Add(separator);
        }

        private void ClearSeparators()
        {
            foreach (var separator in separators)
            {
                separator.style.display = DisplayStyle.None;
            }
            separators.Clear();
        }

        public void ActivateTool(LBSTool tool, LBSLayer layer, object behaviour)
        {
            LBSTool existingTool = null;
            ToolButton existingButton = null;
            if (tool?.Manipulator != null && tools.TryGetValue(tool.Manipulator.GetType(), out (LBSTool, ToolButton) tuple))
            {
                existingTool = tuple.Item1;
                existingButton = tuple.Item2;
            }

            if (existingTool is not null && existingButton is not null)
            {
                existingButton.style.display = DisplayStyle.Flex;
                existingTool.Init(layer, behaviour);
                
                // Remove previous events
                existingTool.OnStart -= (_) => { OnStartAction?.Invoke(existingTool.Manipulator.Layer); };
                existingTool.OnEnd -= (_) => { OnEndAction?.Invoke(existingTool.Manipulator.Layer); };
                
                // add new ones
                existingTool.OnStart += (_) => { OnStartAction?.Invoke(layer); };
                existingTool.OnEnd += (_) => { OnEndAction?.Invoke(layer); };
            }
            else
            {
                AddTool(tool);
            }

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
            // No longer readding the buttons instead hide them when not usedcontent.Clear();
        }
        
        #endregion


    }
}