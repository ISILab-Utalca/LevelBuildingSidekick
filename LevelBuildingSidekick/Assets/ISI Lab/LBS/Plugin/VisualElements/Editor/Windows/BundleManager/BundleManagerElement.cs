using ISILab.Commons.Utility.Editor;
using LBS.Bundles;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISI_Lab.LBS.Plugin.VisualElements.Editor.Windows.BundleManager
{
    public class BundleManagerElement : VisualElement
    {
        private Bundle _bundleRef;
        private ListView _listRef;
        private readonly Label _bundleName;
        private bool _isMasterBundle;

        public BundleManagerElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BundleManagerElement");
            visualTree.CloneTree(this);
            
            _bundleName = this.Q<Label>("BundleName");
            
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
        }

        public void SetRefs(Bundle bundle, ListView list, bool masterBundle)
        {
            _bundleRef = bundle;
            _bundleName.text = _bundleRef.name;
            
            _listRef = list;
            _isMasterBundle = masterBundle;
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
                if (_bundleRef.Parent() != null)
                {
                    _bundleRef.Parent().RemoveChild(_bundleRef);   
                }
            }
        }
    }
}
