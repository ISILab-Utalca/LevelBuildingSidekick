using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System.Linq;
using System;
using LBS.Bundles;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using ISILab.LBS.Characteristics;
using ISILab.LBS;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class CharacteristicsPanel : VisualElement
    {
        #region FACTORY
      //  public new class UxmlFactory : UxmlFactory<CharacteristicsPanel, UxmlTraits> { }
        #endregion

        private VisualElement content;
        private ComplexDropdown search;
        private Foldout foldout;

        private Bundle target;

        public CharacteristicsPanel()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("CharacteristicsPanel");
            visualTree.CloneTree(this);

            content = this.Q<VisualElement>("Content");

            foldout = this.Q<Foldout>();
            foldout.RegisterCallback<ChangeEvent<bool>>(e =>
            {
                content.SetDisplay(e.newValue);
            });

            search = this.Q<ComplexDropdown>();
            search.Init(typeof(LBSCharacteristicAttribute));
            search.OnSelected = (e) =>
            {
                var x = e as LBSCharacteristic;
                target.AddCharacteristic(x);
                SetInfo(target);
                AssetDatabase.SaveAssets();
            };
        }

        public void SetInfo(Bundle target)
        {
            this.target = target;

            content.Clear();
            var characs = target.Characteristics;

            foreach (var characteristic in characs)
            {
                var view = new CharacteristicsBaseView();
                view.SetContent(characteristic);
                content.Add(view);
            }
        }
    }
}