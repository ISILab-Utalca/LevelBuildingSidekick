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

namespace ISILab.LBS.VisualElements.Editor
{
    [UxmlElement]
    public partial class ToolBarMain : VisualElement
    {
        //public new class UxmlFactory : UxmlFactory<ToolBarMain, VisualElement.UxmlTraits> { }
        
        public LBSMainWindow MainWindow;
        
        public event Action<LoadedLevel> OnLoadLevel;
        public event Action<LoadedLevel> OnNewLevel;
        
        public event Action<LBSSettings.Interface.InterfaceTheme> OnThemeChanged;
        
        
        public ToolBarMain()
        {
            VisualTreeAsset visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("ToolBarMain");
            visualTree.CloneTree(this);

            // File menu option
            ToolbarMenu fileMenu = this.Q<ToolbarMenu>("ToolBarMenu");
            fileMenu.menu.AppendAction("New", NewLevel);
            fileMenu.menu.AppendAction("Load", LoadLevel);
            fileMenu.menu.AppendAction("Save", SaveLevel);
            fileMenu.menu.AppendAction("Save as", SaveAsLevel);

            //Button
            ToolbarButton settingMenu = this.Q<ToolbarButton>("OptionButton");
            //settingMenu.clicked += () => OpenConfiguration();
            settingMenu.RegisterCallback<ClickEvent>(OpenConfiguration);

            // var keyMapBtn = this.Q<ToolbarButton>("KeyMapBtn");
            // keyMapBtn.clicked += () =>  LBSMainWindow.DisplayHelp();// { KeyMapWindow.ShowWindow(); };
            
            ToolbarToggle keyMapToggle = this.Q<ToolbarToggle>("KeyMapToggle");
            keyMapToggle.RegisterCallback<ClickEvent>(_ => LBSMainWindow.DisplayHelp()); //Such a awful Hack
            
            var bundManBtn = this.Q<ToolbarButton>("BundleManagerButton");
            bundManBtn.clickable.clicked += BundleManagerWindow.ShowWindow;

            // file name label
            var label = this.Q<Label>("IsSavedLabel"); // TODO: mark as unsaved when changes are made
            
            LBSCustomEnumField ThemeSelector = this.Q<LBSCustomEnumField>("ThemeSelector");
            ThemeSelector.RegisterValueChangedCallback(_evt =>
            {
                Debug.Log(_evt.currentTarget);
                
                OnThemeChanged?.Invoke((LBSSettings.Interface.InterfaceTheme)_evt.newValue);
            });
            
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

        public void SaveLevel(DropdownMenuAction dma)
        {
            LBSController.SaveFile();
            AssetDatabase.Refresh();
        }

        public void SaveAsLevel(DropdownMenuAction dma)
        {
            LBSController.SaveFileAs();
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