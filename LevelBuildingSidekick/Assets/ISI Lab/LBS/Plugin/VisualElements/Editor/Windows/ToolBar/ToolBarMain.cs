using ISILab.Commons.Utility.Editor;
using System;
using ISI_Lab.LBS.Plugin.VisualElements.Editor.Windows.BundleManager;
using ISILab.LBS.CustomComponents;
using UnityEditor;
using UnityEngine.UIElements;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Settings;
using UnityEngine;

namespace ISILab.LBS.VisualElements.Editor
{
    [UxmlElement]
    public partial class ToolBarMain : VisualElement
    {
        //public new class UxmlFactory : UxmlFactory<ToolBarMain, VisualElement.UxmlTraits> { }
        
        public LBSMainWindow MainWindow;
        public string defaultLabel = "Unsaved File *";
        
        public event Action<LoadedLevel> OnLoadLevel;
        public event Action<LoadedLevel> OnNewLevel;
        public event Action<LoadedLevel> OnSaveLevel;
        public event Action<LoadedLevel> OnLevelChange;
        public event Action<LBSSettings.Interface.InterfaceTheme> OnThemeChanged;
        
        public event Action OnProgressCompleted;
        public event Action OnProgressCancelled;
        
        #region  Visual Elements
            private LBSToolbarToggle keyMapToggle;
            private VisualElement taskInfo;
            private LBSCustomProgressBar taskProgressBar;
            private LBSToolbarButton taskStopButton;
            private LBSToolbarButton settingMenu;
        #endregion
        
        public ToolBarMain()
        {
            VisualTreeAsset visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("ToolBarMain");
            visualTree.CloneTree(this);

            // File menu option
            LBSToolbarMenu fileMenu = this.Q<LBSToolbarMenu>("ToolBarMenu");
            fileMenu.menu.AppendAction("New", NewLevel);
            fileMenu.menu.AppendAction("Load", LoadLevel);
            fileMenu.menu.AppendAction("Save", SaveLevel);
            fileMenu.menu.AppendAction("Save as", SaveAsLevel);

            //Button
            settingMenu = this.Q<LBSToolbarButton>("OptionButton");
            keyMapToggle = this.Q<LBSToolbarToggle>("KeyMapToggle");
            
            LBSToolbarButton bundManBtn = this.Q<LBSToolbarButton>("BundleManagerButton");
            bundManBtn.clickable.clicked += BundleManagerWindow.ShowWindow;

            // file name label
            var label = this.Q<Label>("IsSavedLabel"); 
            if(LBS.loadedLevel?.FileInfo!=null) { label.text = LBS.loadedLevel.FileInfo.Name; }
            else { label.text = defaultLabel; }

            LBSCustomEnumField ThemeSelector = this.Q<LBSCustomEnumField>("ThemeSelector");
            ThemeSelector.RegisterValueChangedCallback(_evt =>
            {
                OnThemeChanged?.Invoke((LBSSettings.Interface.InterfaceTheme)_evt.newValue);
            });
            
            OnSaveLevel += (level) => { label.text = LBS.loadedLevel?.FileInfo?.Name; };
            OnLevelChange += (level) => { label.text = LBS.loadedLevel?.FileInfo != null ? LBS.loadedLevel.FileInfo.Name +" *" : defaultLabel; };
            
            
            taskInfo = this.Q<VisualElement>("TaskInfo");
            taskProgressBar = this.Q<LBSCustomProgressBar>("TaskProgressBar");
            taskStopButton = this.Q<LBSToolbarButton>("TaskStop");
            
            taskStopButton.clicked += () =>
            {
                EnableProcess(false);
                OnProgressCancelled?.Invoke();
            };
            
            taskInfo.style.display = DisplayStyle.None;
            
        }

        public void EnableProcess(bool enable, string assistantName = "Assistant")
        {
            var taskOverlay = MainWindow.rootVisualElement.Q<LBSWaitTaskOverlay>();
            taskOverlay.ShowRect = enable;
            
            taskProgressBar.ProgressTextLabel = assistantName;
            var percent = enable ? 0 : 1;
            SetProgressPercent(percent);
            taskInfo.style.display = enable ? DisplayStyle.Flex : DisplayStyle.None;
        }
        public void SetProgressPercent(float percent)
        {
            taskProgressBar.title = $"{Mathf.RoundToInt(percent * 100)}%";
            taskProgressBar.value = percent;    
        }
        
        public void Bind(LBSMainWindow _mainWindow)
        {
            MainWindow = _mainWindow;
            keyMapToggle.RegisterCallback<ClickEvent>(evt =>
            {
                Debug.Log("[Display Help]: Toggle KeyMap]");
                MainWindow.DisplayHelp();
            });
            
            OnNewLevel += (_loadedLevel) =>
            {
                LBS.loadedLevel = _loadedLevel;
                MainWindow.RefreshWindow();
            };
            
            settingMenu.RegisterCallback<ClickEvent>(OpenConfiguration);
        }
        
        
        public void UnBind(LBSMainWindow _mainWindow)
        {
            
        }

        public void NewLevel(DropdownMenuAction dma)
        {
            NewLevel();
        }
        public void NewLevel()
        {
            var answer = EditorUtility.DisplayDialog(
                   "Current File Not Saved",
                   "Progress in the current level will be lost. Proceed?",
                   "confirm",
                   "cancel");
            switch (answer)
            {
                case true:
                    var data = LBSController.CreateNewLevel("new file");
                    if (data != null)
                    {
                        OnNewLevel?.Invoke(data);
                     
                        LBSMainWindow.MessageNotify("New level created.");
                    }
                    return;
                case false:
                    return;
            }

            
        }

        public void LoadLevel(DropdownMenuAction dma)
        {
            LoadLevel();
        }
        public void LoadLevel()
        {
            var data = LBSController.LoadFile();
            if (data != null)
            {
                OnLoadLevel?.Invoke(data);
                LBSMainWindow.MessageNotify("The level has been loaded successfully.");
            }

        }

        public void LevelChange()
        {
            OnLevelChange?.Invoke(LBS.loadedLevel);
        }

        public void SaveLevel(DropdownMenuAction dma)
        {
            SaveLevel();
        }
        public void SaveLevel()
        {
            LBSController.SaveFile();
            OnSaveLevel?.Invoke(LBS.loadedLevel);
            AssetDatabase.Refresh();
        }

        public void SaveAsLevel(DropdownMenuAction dma)
        {
            if (LBSController.SaveFileAs()) { 
                OnSaveLevel?.Invoke(LBS.loadedLevel);
            }
            AssetDatabase.Refresh();
        }

        public static void OpenConfiguration(ClickEvent evt)
        {
            // Open the Project Settings window
            SettingsService.OpenProjectSettings("LBS");
        }
        
        public void CancelProgress()
        {
            OnProgressCancelled?.Invoke();
            OnProgressCancelled = null;
            EnableProcess(false);
        }
    }
}