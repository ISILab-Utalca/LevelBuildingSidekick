using UnityEngine;
using UnityEngine.UIElements;

using System;
using System.Linq;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.AI.Categorization;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;

namespace ISILab.LBS.VisualElements.Editor
{
    [UxmlElement]
    public partial class PopulationMapEntry : VisualElement
    {
        #region UXMLFACTORY
        [UxmlElementAttribute]
        public new class UxmlFactory { }
        #endregion

        #region VIEW ELEMENTS
        private VisualElement image;
        private VisualElement bar; // contains the progress bar and label
        private VisualElement placeholderImage;
        private VisualElement colorFillBar;

        private Label mapName;
        private Foldout entryFoldout;
        private Button buttonRemove;

        private ProgressBar progressBar; // contains the progress bar and label
        
        #endregion

        #region FIELDS
        private SavedMap entryMap;

        // result
        private object data;
        // value/score of the generated result
        private float score;

        #endregion

        #region EVENTS
        // public Action OnExecute;
        #endregion

        #region PROPERTIES
        public SavedMap EntryMap
        {
            get => entryMap;
            set => entryMap = value;
        }
        public string Name
        {
            get => mapName.text;
            set => mapName.text = value;
        }
        public object Data
        {
            get => data;
            set => data = value;
        }
        public float Score
        {
            get => score;
            set => score = value;
        }

        #endregion


        #region EVENTS
        public Action RemoveMapEntry;
        #endregion

        #region CONSTRUCTORS
        public PopulationMapEntry()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("PopulationMapEntry");
            visualTree.CloneTree(this);
            
            entryFoldout = this.Q<Foldout>("Foldout");
            entryFoldout.RegisterCallback<ClickEvent>(evt =>
            {
                entryFoldout.value = !entryFoldout.value;
                var mode = entryFoldout.value;
                bar.style.display = mode ? DisplayStyle.Flex : DisplayStyle.None;
                entryFoldout.style.backgroundImage = mode ? 
                    new StyleBackground(Resources.Load<VectorImage>("Icons/Arrows/Icon=ArrowUp")) :
                    new StyleBackground(Resources.Load<VectorImage>("Icons/Arrows/Icon=ArrowDown"));
                

            });
            
            image = this.Q<VisualElement>("Image");
            placeholderImage = this.Q<VisualElement>("PlaceHolderImage");
            mapName = this.Q<Label>("MapName");
            buttonRemove = this.Q<Button>("RemoveButton");
            buttonRemove.clicked += () =>  RemoveMapEntry?.Invoke();
   
            
            bar = this.Q<VisualElement>("Bar");
            progressBar = this.Q<ProgressBar>("ProgressBar");
            FindColorFillVisualElement(progressBar as VisualElement);
            
            colorFillBar.style.backgroundColor = new StyleColor(new Color(0f, 1f, 0.68f, 0.8f));

        }
        #endregion

        private void FindColorFillVisualElement(VisualElement element)
        {
            while (true)
            {
                if (element.Children().Count() > 1)
                {
                    for (int i = 0; i < element.Children().Count(); i++)
                    {
                        if (i == 0) colorFillBar = element.Children().ElementAt(i);
                    }
                }
                else if (element.Children().Any())
                {
                    element = element.Children().First() as VisualElement;
                    continue;
                }

                break;
            }
        }

        private void SetColor(float score)
        {
            // normalize value 
            var percentage = (score)*100f;
            progressBar.value = percentage;
            
            //interpolate color based on score value
            var color = new Color(score, score, score);
            colorFillBar.style.backgroundColor = color; 
            
        }

        private void SetMapImage(Texture2D texture)
        {
            placeholderImage.style.display = DisplayStyle.None;
            image.style.backgroundImage = texture;
        }

        public void SetData(SavedMap mapEntry)
        {
            entryMap = mapEntry;
            //Name
            Name = mapEntry.Name;
            Score = mapEntry.Score;
            Data = mapEntry.Map;
            // Based on the data

            SetMapImage(mapEntry.Image);
            SetColor(Score);

        }
    }
}