using ISILab.LBS.Editor.Windows;
using UnityEngine;
using UnityEngine.UIElements;
using LBS.Components;

using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using ISI_Lab.LBS.Plugin.MapTools.Generators3D;
using ISILab.Commons;
using UnityEditor;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Generators;
using Object = UnityEngine.Object;
using UnityEditor.UIElements;

namespace ISILab.LBS.VisualElements.Editor
{
    [UxmlElement]
    public partial class PopulationAssistantButtonResult : VisualElement
    {
        #region UXMLFACTORY
        [UxmlElementAttribute]
        public new class UxmlFactory { }
        #endregion

        #region VIEW ELEMENTS

        private VisualElement background;
        // display 2 decimals only
        private Label scoreLabel;
        // image displaying 3 dots and eventually the generated data
        private VisualElement loadingIcon;
        private VisualElement customImage;
        //Texture for generated data
        public Texture2D backgroundTexture;
        //Button to show highlights
        public VisualElement selectBorder;
        public VisualElement inactiveBorder;
        public Button selectButton;
        public ToolbarMenu toolBar;
        #endregion

        #region FIELDS
        // result
        private object data;
        // value/score of the generated result
        private string score;
        //Can this be selected?
        private bool canHighlight;
        private bool selected;
        #endregion

        #region EVENTS
        public Action OnExecute;
        public Action OnImageChange;
        //Checks if the button is selected or not.
        public Action OnButtonDeselected;
        public Action OnButtonSelected;
        //Right click buttons
        public Action OnApplySuggestion;
        public Action OnSaveSuggestion;

        #endregion

        #region PROPERTIES
        public object Data { 
            get => data;
            set => data = value;
        }
        public string Score
        {
            get => score;
            set => score = value;
        }
        #endregion

        #region CONSTRUCTORS
        public PopulationAssistantButtonResult()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("PopulationAssistantButtonResult");
            visualTree.CloneTree(this);

            background = this.Q<VisualElement>("Background");

            loadingIcon = this.Q<VisualElement>("LoadingImage");
            customImage = this.Q<VisualElement>("CustomImage");

            scoreLabel = this.Q<Label>("ScoreValue");

            selectBorder = this.Q<VisualElement>("SelectBorder");
            selectBorder.style.opacity = 0.0f;
            selectButton = this.Q<Button>("SelectButton");

            inactiveBorder = this.Q<VisualElement>("InactiveBorder");

            //Decoratives to check if the button is highlighted or not
            OnButtonDeselected += () =>
            {
                selected = false;
                selectBorder.visible = false;
                inactiveBorder.visible = true;
                selectBorder.style.opacity = 0.5f;
            };

            OnButtonSelected += () =>
            {
                selected = true;
                selectBorder.visible = true;
                inactiveBorder.visible = false;
                selectBorder.style.opacity = 1f;
            };

            OnImageChange += () =>
            {
                loadingIcon.visible = customImage.style.backgroundImage != null ? false : true;
                canHighlight = customImage.style.backgroundImage != null ? true : false;
            };
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            RegisterCallback<MouseEnterEvent>(OnMouseEnter);

            //Right click stuff

            ContextualMenuManipulator m = new ContextualMenuManipulator(ResultManipulator);
            m.target = this;
        }

        void ResultManipulator(ContextualMenuPopulateEvent evt)
        {
            // Apply Suggestion
            evt.menu.AppendAction("Apply", action =>
            {
                if (Data != null) OnApplySuggestion?.Invoke();
            }
            );
            // Save Suggestion
            evt.menu.AppendAction("Pin", action =>
            {
                if (Data != null) OnSaveSuggestion?.Invoke();
            }
            );
        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            if ((canHighlight)&&(!selected))
            {
                selectBorder.style.opacity = 0.5f;
                selectBorder.visible = true;
                inactiveBorder.visible = false;
            }
        }

        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            if(!selected)
            {
                selectBorder.visible = false;
                inactiveBorder.visible = true;
            }
        }
        #endregion

        public Texture2D GetTexture()
        {
            if (data is IDrawable)
            {
                return (data as IDrawable).ToTexture();
            }
            return default;
        }

        public void SetTexture(Texture2D texture)
        {
            if (customImage == null) { Debug.Log("No image");  return; }
            backgroundTexture = texture;
            customImage.style.backgroundImage = texture;
            OnImageChange?.Invoke();
        }

        public void SetColor(Color color)
        {
            background.style.backgroundColor = color;
        }

        public void UpdateLabel()
        {
            if (scoreLabel == null) return;
            scoreLabel.text = Score;
        }
    }
}