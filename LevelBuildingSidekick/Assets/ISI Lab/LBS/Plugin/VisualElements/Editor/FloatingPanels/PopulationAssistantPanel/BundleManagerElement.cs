using ISILab.Commons.Utility.Editor;
using LBS.Bundles;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace ISILab.LBS.VisualElements.Editor
{
    public class BundleManagerElement : VisualElement
    {
        private Bundle _bundleRef;
        private Label _bundleName;
        private Button _deleteButton;
        public BundleManagerElement()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("BundleManagerElement");
            visualTree.CloneTree(this);
            
            _bundleName = this.Q<Label>("BundleName");
            
            _deleteButton = this.Q<Button>("DeleteButton");
            _deleteButton.clickable.clicked += () =>
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
                        FileUtil.DeleteFileOrDirectory(AssetDatabase.GetAssetPath(_bundleRef));
                        return;
                    case 1: //Cancel
                        return;
                }
            };
        }

        public void SetBundleRef(Bundle bundle)
        {
            _bundleRef = bundle;
            _bundleName.text = _bundleRef.name;
        }
        /*
         * 
            string path;
            FileInfo fileInfo;
            LBSLevelData data;
            switch (answer)
            {
                case 0: // Save
                    SaveFile();
                    path = EditorUtility.OpenFilePanel("Load level data", "", "lbs");
                    fileInfo = new System.IO.FileInfo(path);
                    data = JSONDataManager.LoadData<LBSLevelData>(fileInfo.DirectoryName, fileInfo.Name);
                    CurrentLevel = LoadedLevel.CreateInstance(data, fileInfo.FullName);
                    CurrentLevel.data.Reload();
                    return CurrentLevel;

                case 1: // Discard
                    
                    LBSMainWindow.MessageNotify("Level discarded.");
                    
                    path = EditorUtility.OpenFilePanel("Load level data", "", "lbs");
                    if (path == "")
                        return null;
                    fileInfo = new System.IO.FileInfo(path);
                    data = JSONDataManager.LoadData<LBSLevelData>(fileInfo.DirectoryName, fileInfo.Name);
                    CurrentLevel = LoadedLevel.CreateInstance(data, fileInfo.FullName);
                    CurrentLevel.data.Reload();
                    OnLoadLevel?.Invoke(CurrentLevel.data);
                    return CurrentLevel;

                case 2: //do nothing
                default:
                    return null; 
            }
        */
    }
}
