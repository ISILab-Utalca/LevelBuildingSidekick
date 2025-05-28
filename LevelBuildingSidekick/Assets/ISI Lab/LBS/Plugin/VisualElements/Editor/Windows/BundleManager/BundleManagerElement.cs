using System.Collections.Generic;
using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Internal;
using LBS.Bundles;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISI_Lab.LBS.Plugin.VisualElements.Editor.Windows.BundleManager
{
    public class BundleManagerElement : VisualElement
    {
        // External references
        private Bundle _bundleRef;
        private BundleCollection _bundleCollectionRef;
        private ListView _listRef;
        
        // Internal references
        private readonly Label _bundleName;
        private readonly IMGUIContainer _bundleIcon;
        private readonly IMGUIContainer _masterIcon;
        private readonly IMGUIContainer _warningIcon;
        
        // Properties
        private bool _isMasterBundle;

        public BundleManagerElement(int size, int gapSize)
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BundleManagerElement");
            visualTree.CloneTree(this);
            
            _bundleName = this.Q<Label>("BundleName");
            _bundleIcon = this.Q<IMGUIContainer>("BundleIcon");
            _masterIcon = this.Q<IMGUIContainer>("MasterIcon");
            _warningIcon = this.Q<IMGUIContainer>("WarningIcon");
            
            // Set DeleteBundle Button
            Button deleteButton = this.Q<Button>("DeleteButton");
            deleteButton.clickable.clicked += () =>
            {
                var answer = EditorUtility.DisplayDialogComplex(
                    "The bundle will be deleted",
                    "The bundle: " + _bundleName.text + " will be deleted, are you sure you want to continue?",
                    "Delete",
                    "Cancel",
                    "");
                switch (answer)
                {
                    case 0: //Delete
                        string path = AssetDatabase.GetAssetPath(_bundleRef);
                        RemoveFromList();
                        Debug.Log(AssetDatabase.DeleteAsset(path)
                            ? "File at " + path + " successfully deleted"
                            : "File failed to delete");
                        _listRef.RefreshItems();
                        return;
                    case 1: //Cancel
                        return;
                }
            };
            
            // Set size for visualElements
            this.Q<VisualElement>("Background_1").style.height = size - gapSize;
            this.Q<VisualElement>("DeleteButton").style.height = size - gapSize;
        }

        public void SetRefs(Bundle bundle, ListView list, bool masterBundle)
        {
            _bundleRef = bundle;
            _bundleCollectionRef = null;
            
            _bundleName.text = bundle == null ? "Empty Bundle" : _bundleRef.name;
            
            _listRef = list;
            _isMasterBundle = masterBundle;

            if (bundle.Icon != null)
            {
                _bundleIcon.style.backgroundImage = new StyleBackground(bundle.Icon);   
            }
        }
        public void SetRefs(BundleCollection bundle, ListView list)
        {
            _bundleRef = null;
            _bundleCollectionRef = bundle;
            
            _bundleName.text = bundle == null ? "Empty Bundle Collection" : _bundleCollectionRef.name;
            
            _listRef = list;
            _isMasterBundle = true;

            if (bundle.Icon != null)
            {
                _bundleIcon.style.backgroundImage = new StyleBackground(bundle.Icon);   
            }
        }

        private void RemoveFromList()
        {
            if (_isMasterBundle)
            {
                foreach (BundleManagerWindow.BundleContainer item in _listRef.itemsSource)
                {
                    if (item.GetMainBundle().Equals(_bundleRef))
                    {
                        _listRef.itemsSource.Remove(item);
                        break;
                    }
                }
            }
            else
            {
                _listRef.itemsSource.Remove(_bundleRef);
                List<Bundle> parents = _bundleRef.Parents();
                foreach (Bundle p in parents)
                {
                    p.RemoveChild(_bundleRef);
                }
            }
            LBSAssetsStorage.Instance.RemoveElement(_bundleRef);
        }

        public void SetIconDisplay(Icons icon, bool display)
        {
            DisplayStyle displayStyle = display ? DisplayStyle.Flex : DisplayStyle.None; 
            
            switch (icon)
            {
                case Icons.Bundle:
                    _bundleIcon.style.display = displayStyle;
                    break;
                case Icons.Master:
                    _masterIcon.style.display = displayStyle;
                    break;
                case Icons.Warning:
                    _warningIcon.style.display = displayStyle;
                    break;
            }
        }

        public enum Icons
        {
            Bundle, Master, Warning
        }
    }
}
