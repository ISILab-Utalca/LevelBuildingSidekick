using ISILab.Commons.Utility.Editor;
using ISILab.LBS.AI.Categorization;
using ISILab.LBS.VisualElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace ISILab.LBS.AI.Categorization.Editor
{
    public class MapElitePresetEditorWindow : EditorWindow
    {
        MAPElitesPreset mapElitesPresset;

        string path = string.Empty;

        bool preexistant = true;

        TextField pathField;
        TextField nameField;
        VisualElement content;
        Button save;

        public static void OpenWindow(MAPElitesPreset presset)
        {
            var wnd = GetWindow<MapElitePresetEditorWindow>();
            Texture icon = Resources.Load<Texture>("Icons/Gear.png"); // FIX: It's not finding the icon.
            wnd.titleContent = new GUIContent("MAP Elites Preset Editor", icon);
            wnd.minSize = new Vector2(800, 400);
            wnd.LoadPreset(presset);
        }

        public virtual void CreateGUI()
        {
            var visualTree = DirectoryTools.GetAssetByName<VisualTreeAsset>("MapElitesPresetEditorWindow");
            visualTree.CloneTree(rootVisualElement);


            pathField = rootVisualElement.Q<TextField>(name: "Path");
            nameField = rootVisualElement.Q<TextField>(name: "Name");
            content = rootVisualElement.Q<VisualElement>(name: "Content");
            save = rootVisualElement.Q<Button>(name: "Save");

            nameField.RegisterValueChangedCallback(
                evt =>
                {
                    mapElitesPresset.name = evt.newValue;
                });

            pathField.RegisterValueChangedCallback(
                evt =>
                {
                    path = evt.newValue;
                });

            save.clicked += Save;

        }

        private void LoadPreset(MAPElitesPreset presset)
        {
            mapElitesPresset = presset;

            if (mapElitesPresset == null)
            {
                mapElitesPresset = CreateInstance<MAPElitesPreset>();
                mapElitesPresset.name = "New Preset";
                preexistant = false;
            }

            if (preexistant)
            {
                var p = AssetDatabase.GetAssetPath(mapElitesPresset);
                var index = p.LastIndexOf('/');
                path = p.Substring(0, index);
            }
            else
            {
                path = "Assets/ISI Lab/LBS/Presets/Resources";
            }

            var mp = new MAPElitesPresetVE(mapElitesPresset);
            content.Add(mp);

            nameField.value = mapElitesPresset.name;
            pathField.value = path;
        }

        private void Save()
        {

            if (preexistant)
            {
                var p1 = AssetDatabase.GetAssetPath(mapElitesPresset);
                var p2 = path + "/" + mapElitesPresset.name + ".asset";
                if (!p1.Equals(p2))
                    AssetDatabase.RenameAsset(p1, p2);
            }
            else
            {
                var p = path + "/" + mapElitesPresset.name + ".asset";
                AssetDatabase.CreateAsset(mapElitesPresset, p);
            }

            EditorUtility.SetDirty(mapElitesPresset);
            AssetDatabase.SaveAssets();

            Debug.Log(path);
        }
    }
}