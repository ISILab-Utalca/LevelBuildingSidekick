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
        public Button selectButton;

        public Texture2D backgroundTexture;
        private Texture2D defaultImage;

        #endregion

        #region FIELDS
        // result
        private object data;
        // value/score of the generated result
        private string score;
        //Can this be selected?
        private bool canHighlight;
        
        #endregion

        #region EVENTS
        public Action OnExecute;
        public Action OnImageChange;
        public Action OnButtonClicked;
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
            scoreLabel = this.Q<Label>("ScoreValue");
            selectButton = this.Q<Button>("SelectButton");
            //selectButton.clicked += ButtonClicked;
            selectButton.visible = false;

            loadingIcon = this.Q<VisualElement>("LoadingImage");
            customImage = this.Q<VisualElement>("CustomImage");
            OnImageChange += () =>
            {

                loadingIcon.visible = customImage.style.backgroundImage != null ? false : true;
                canHighlight = customImage.style.backgroundImage != null ? true : false;
            };

            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            RegisterCallback<MouseEnterEvent>(OnMouseEnter);

            
        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            if (canHighlight)
            {
                selectButton.visible = true;
            }
        }

        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            selectButton.visible = false;
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