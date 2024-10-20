using ISILab.Commons.Utility.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using System.Linq;
using LBS.Bundles;
using ISILab.LBS.Characteristics;
using ISILab.LBS.Components;

namespace LBS.VisualElements
{
    public class PathOSTagPallete : VisualElement
    {
        #region FACTORY
        public new class UxmlFactory : UxmlFactory<PathOSTagPallete, VisualElement.UxmlTraits> { }
        #endregion

        private PathOSOptionView[] optionViews;
        private object[] options;
        private object selected;
        private Action<PathOSOptionView, object> onSetView;

        #region FIELS VIEW
        private VisualElement contentElements;
        private VisualElement contentEvents;
        private DropdownField dropdownGroup;
        private VisualElement icon;
        private Label nameLabel;
        private Button noElement;
        private Button noEvent;
        private Button addButton;
        private Button removeButton;
        #endregion

        #region EVENTS
        public event Action<ChangeEvent<string>> OnChangeGroup;
        public event Action<object> OnSelectOption;
        public event Action<object> OnRemoveOption;
        public event Action OnAddOption;
        public event Action OnRepaint;
        public event Func<object, string> OnSetTooltip;
        #endregion

        #region PROPERTIES
        public object Selected
        {
            get => selected;
            set => selected = value;
        }

        public object[] Options
        {
            get => options;
            set => options = value;
        }

        public bool ShowGroups
        {
            set => this.dropdownGroup.SetDisplay(value);
        }

        public bool ShowRemoveButton
        {
            set => this.removeButton.SetDisplay(value);
        }

        public bool ShowAddButton
        {
            set => this.addButton.SetDisplay(value);
        }
        #endregion


        #region CONSTRUCTORS
        public PathOSTagPallete()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("PathOSTagPallete");
            visualTree.CloneTree(this);

            // Content: Element Tags
            this.contentElements = this.Q<VisualElement>("ContentElementTags");

            // Content: Event Tags
            this.contentEvents = this.Q<VisualElement>("ContentEventTags");

            // Change Group
            this.dropdownGroup = this.Q<DropdownField>("DropdownGroup");
            dropdownGroup.RegisterCallback<ChangeEvent<string>>(evt => OnChangeGroup?.Invoke(evt));

            // NameLabel
            this.nameLabel = this.Q<Label>("NameLabel");

            // AddButton
            this.addButton = this.Q<Button>("AddButton");
            addButton.clicked += () => OnAddOption?.Invoke();

            // removeButton
            this.removeButton = this.Q<Button>("DeleteButton");
            removeButton.clicked += () => OnRemoveOption?.Invoke(selected);

            // NoElement
            this.noElement = this.Q<Button>("NoElement");

            // NoEvent
            this.noEvent = this.Q<Button>("NoEvent");

            // Icon
            this.icon = this.Q<VisualElement>("IconPallete");
        }
        #endregion

        #region METHODS
        private void OnInternalSelectOption(object obj)
        {

            foreach (var optV in optionViews)
            {
                optV.SetSelected(false);
            }
            selected = obj;
            OnSelectOption?.Invoke(obj);

        }

        public void SetOptions(object[] options, Action<PathOSOptionView, object> onSetView)
        {
            this.options = options;
            this.onSetView = onSetView;
        }

        public void SetIcon(Texture2D icon, Color color)
        {
            this.icon.style.backgroundImage = icon;
            this.icon.style.unityBackgroundImageTintColor = color;
        }

        public void SetName(string name)
        {
            this.nameLabel.text = name;
        }

        public void Repaint()
        {
            OnRepaint?.Invoke();

            // Limpiar contenedores
            contentElements.Clear();
            contentEvents.Clear();

            this.optionViews = new PathOSOptionView[options.Length];

            // Se agregan opciones a contenedores
            for (int i = 0; i < options.Length; i++)
            {
                var option = options[i];
                var view = new PathOSOptionView(option, OnInternalSelectOption, onSetView);
                view.tooltip = OnSetTooltip?.Invoke(option);
                optionViews[i] = view;

                // Se agrega a contenedor correspondiente segun PathOSTag asociado a la opcion (obtenido
                // desde su Bundle).
                Bundle castedOption = option as Bundle;
                if (castedOption != null)
                {
                    var t = castedOption.GetCharacteristics<LBSPathOSTagsCharacteristic>()[0].Value;
                    if (t.Category == PathOSTag.PathOSCategory.ElementTag)
                    {
                        contentElements.Add(view);
                    }
                    else if (t.Category == PathOSTag.PathOSCategory.EventTag)
                    {
                        contentEvents.Add(view);
                    }
                }
                // Si NO es convertible a bundle, arroja advertencia.
                else
                {
                    Debug.LogWarning("PathOSTagPallete.Repaint():" +
                        "Elemento no convertible a Bundle encontrado en opciones!");
                }
            }

            // Si un contenedor queda vacio, se le agrega su boton "vacio"
            // correspondiente. 
            if (contentElements.childCount == 0)
            {
                contentElements.Add(noElement);
            }
            if (contentEvents.childCount == 0)
            {
                contentEvents.Add(noEvent);
            }

            // Se restablece la PathOSOptionView seleccionada anteriormente
            if (selected != null)
            {
                var ov = optionViews.ToList().Find(o => o.target.Equals(selected));
                if (ov != null)
                    ov.SetSelected(true);
            }
        }
        #endregion
    }

}