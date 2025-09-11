using ISILab.Commons.Utility.Editor;
using System;
using ISI_Lab.LBS.Plugin.VisualElements.Editor.Windows.BundleManager;
using ISILab.LBS.CustomComponents;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using ISILab.LBS.Editor.Windows;
using ISILab.LBS.Settings;
using UnityEngine;
using static UnityEngine.Analytics.IAnalytic;

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
            LBSToolbarButton settingMenu = this.Q<LBSToolbarButton>("OptionButton");
            //settingMenu.clicked += () => OpenConfiguration();
            settingMenu.RegisterCallback<ClickEvent>(OpenConfiguration);

            // var keyMapBtn = this.Q<ToolbarButton>("KeyMapBtn");
            // keyMapBtn.clicked += () =>  LBSMainWindow.DisplayHelp();// { KeyMapWindow.ShowWindow(); };
            
            LBSToolbarToggle keyMapToggle = this.Q<LBSToolbarToggle>("KeyMapToggle");
            keyMapToggle.RegisterCallback<ClickEvent>(_ => LBSMainWindow.DisplayHelp()); //Such a awful Hack
            
            LBSToolbarButton bundManBtn = this.Q<LBSToolbarButton>("BundleManagerButton");
            bundManBtn.clickable.clicked += BundleManagerWindow.ShowWindow;

            // file name label
            var label = this.Q<Label>("IsSavedLabel"); 
            if(LBS.loadedLevel?.FileInfo!=null) { label.text = LBS.loadedLevel.FileInfo.Name; }
            else { label.text = defaultLabel; }

                LBSCustomEnumField ThemeSelector = this.Q<LBSCustomEnumField>("ThemeSelector");
            ThemeSelector.RegisterValueChangedCallback(_evt =>
            {
                //Debug.Log(_evt.currentTarget);
                
                OnThemeChanged?.Invoke((LBSSettings.Interface.InterfaceTheme)_evt.newValue);
                
            });

            OnSaveLevel += (level) => { label.text = LBS.loadedLevel?.FileInfo?.Name; };
            OnLevelChange += (level) => { label.text = LBS.loadedLevel?.FileInfo != null ? LBS.loadedLevel.FileInfo.Name +" *" : defaultLabel; };
        }

        public void NewLevel(DropdownMenuAction dma)
        {
            var data = LBSController.CreateNewLevel("new file");
            OnNewLevel?.Invoke(data);
            LBSMainWindow.MessageNotify("New level created.");
        }

        public void LoadLevel(DropdownMenuAction dma)
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
            Debug.Log("saving");
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


        public void ChangeTheme(LBSSettings.Interface.InterfaceTheme _newTheme)
        {
            if (MainWindow == null) return;
            
            
            switch (_newTheme)
            {
               case  LBSSettings.Interface.InterfaceTheme.Light:
                   this.ClearClassList();
                   this.AddToClassList("light");
                   break;
               case  LBSSettings.Interface.InterfaceTheme.Dark:
                   this.ClearClassList();
                   this.AddToClassList("dark");
                   break;
               case LBSSettings.Interface.InterfaceTheme.Alt:
                   this.ClearClassList();
                   this.AddToClassList("alt");
                   break;
               default:
                   break;
            }
        }

    }
}