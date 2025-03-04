using ISILab.Commons.Utility.Editor;
using LBS.Components.TileMap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using ISILab.Extensions;
using ISILab.LBS.Behaviours;
using UnityEditor.UIElements;
using Object = UnityEngine.Object;

namespace ISILab.LBS.VisualElements
{
    public class WorldEditBarView : GraphElement
    {
        #region VIEW FIELDS
        private static VisualTreeAsset view;
        
        private EnumField typeField;
        private ObjectField bundleField;
        private Button resetButton;
        private Button shuffleButton;

        // reference to the gameObject containing the gizmo
        public GameObject go;
        #endregion

        public WorldEditBarView(GameObject targetComponent)
        {
            if (view == null)
            {
                view = DirectoryTools.GetAssetByName<VisualTreeAsset>("WorldEditBarView");
            }
            view.CloneTree(this);
            
            typeField = this.Q<EnumField>("TypeField");
            bundleField = this.Q<ObjectField>("BundleField");
            resetButton = this.Q<Button>("ResetButton");
            shuffleButton = this.Q<Button>("ShuffleButton");
            
            typeField.RegisterValueChangedCallback(evt => OnTypeChanged(evt.newValue));
            bundleField.RegisterValueChangedCallback(evt => OnBundleChanged(evt.newValue));
            resetButton.clicked += OnResetClicked;
            shuffleButton.clicked += OnShuffleClicked;

            go = targetComponent;
            Debug.Log(go.name);
        }

        private void OnShuffleClicked()
        {
            throw new NotImplementedException();
        }

        private void OnResetClicked()
        {
            throw new NotImplementedException();
        }

        private void OnBundleChanged(Object evtNewValue)
        {
            throw new NotImplementedException();
        }

        private void OnTypeChanged(Enum evtNewValue)
        {
            throw new NotImplementedException();
        } 
    }
}