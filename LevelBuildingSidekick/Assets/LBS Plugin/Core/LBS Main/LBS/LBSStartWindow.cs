using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using LevelBuildingSidekick.Graph;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace LevelBuildingSidekick
{
    public class LBSStartWindow : EditorWindow
    {
        #region Selection
        Button newLvlBtn;
        Button loadLvlBtn;
        Button infoBtn;

        VisualElement newLvlPanel;
        VisualElement loadLvlPanel;
        VisualElement infoPanel;
        #endregion

        #region NewLevel
        TextField newLvlNameField;
        Vector3Field newLvlSizeField;
        Button openNewLvlBtn;
        DropdownField lvlRepListDD;

        #endregion

        #region LoadLevel
        DropdownField loadLvlSelectionDD;
        Button openLoadLvlBtn;
        #endregion

        [MenuItem("LBS/Welcome window...", priority = 0)]
        public static void ShowWindow()
        {
            var window = GetWindow<LBSStartWindow>();
            window.titleContent = new GUIContent("Level Building Sidekick");
            //var btn1 = buscar boton;
            Debug.Log(window.position);
            window.position = window.position;
            var controller = new LBSController();
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("StartWindow");
            visualTree.CloneTree(root);

            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("StartWindow");
            root.styleSheets.Add(styleSheet);


            InitSelection();
            InitNewLevel();
            InitLoadLevel();
        }

        void InitSelection()
        {
            newLvlBtn = rootVisualElement.Q<Button>(name: "NewLvlBtn");
            loadLvlBtn = rootVisualElement.Q<Button>(name: "LoadLvlBtn");
            infoBtn = rootVisualElement.Q<Button>(name: "InfoBtn");

            newLvlPanel = rootVisualElement.Q<VisualElement>(name: "NewLevel");
            loadLvlPanel = rootVisualElement.Q<VisualElement>(name: "LoadLevel");
            infoPanel = rootVisualElement.Q<VisualElement>(name: "Info");

            TurnOnNewLevel();
            newLvlBtn.clicked += TurnOnNewLevel;
            loadLvlBtn.clicked += TurnOnLoadLevel;
            infoBtn.clicked += TurnOnInfo;
        }

        void InitNewLevel()
        {
            newLvlNameField = rootVisualElement.Q<TextField>(name: "LvlNameField");
            newLvlNameField.value = "New Level Name";

            newLvlSizeField = rootVisualElement.Q<Vector3Field>(name: "LvlSizeField");
            newLvlSizeField.value = new Vector3(512,2,512);

            var representations = Utility.Reflection.FindDerivedTypes(typeof(LBSRepesentationData));
            lvlRepListDD = rootVisualElement.Q<DropdownField>(name: "LvlRepListDD");
            lvlRepListDD.choices = representations.Select(r => r.ToString()).ToList();
            
            openNewLvlBtn = rootVisualElement.Q<Button>(name: "OpenNewLvlBtn");
            openNewLvlBtn.clicked += OpenNewLevel;

        }

        void InitLoadLevel()
        {
            loadLvlSelectionDD = rootVisualElement.Q<DropdownField>(name: "LoadLvlSelectionDD");
            openLoadLvlBtn = rootVisualElement.Q<Button>(name: "OpenLoadLvlBtn");

            var jsonFiles = Utility.JSONDataManager.GetJSONFiles(Application.dataPath + "/LBSLevels");
            //Debug.Log(jsonFiles.Count);
            loadLvlSelectionDD.choices = jsonFiles;

            openLoadLvlBtn.clicked += LoadLevel;

        }

        void InitInfo()
        {

        }

        void TurnOnNewLevel()
        {
            newLvlPanel.visible = true;
            loadLvlPanel.visible = false;
            infoPanel.visible = false;
        }

        void TurnOnLoadLevel()
        {
            newLvlPanel.visible = false;
            loadLvlPanel.visible = true;
            infoPanel.visible = false;
        }

        void TurnOnInfo()
        {
            newLvlPanel.visible = false;
            loadLvlPanel.visible = false;
            infoPanel.visible = true;
        }

        void OpenNewLevel()
        {
            LBSController.CurrentLevel = LBSController.CreateLevel(newLvlNameField.value, Vector3.zero);
            LBSGraphWindow.OpenWindow();
            this.Close();
        }

        void LoadLevel()
        {
            LBSController.CurrentLevel = Utility.JSONDataManager.LoadData<LevelData>("LBSLevels", loadLvlSelectionDD.value);
            LBSGraphWindow.OpenWindow();
            this.Close();
        }
    }
}

