using ISILab.Commons.Utility.Editor;
using ISILab.LBS.Internal;
using ISILab.LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static ISILab.LBS.Settings.LBSSettings;

namespace ISILab.LBS.Settings
{
    // Register a SettingsProvider using IMGUI for the drawing framework:
    static class LBS_SettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider ToolsSettingProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("LBS/Tools", SettingsScope.Project)
            {
                label = "Tools",
                guiHandler = (searchContext) =>
                {
                    var settings = Instance;

                    //layer
                    EditorGUILayout.LabelField("Layers", EditorStyles.boldLabel);
                    settings.general.baseLayerName = EditorGUILayout.TextField("Default Layer Name", settings.general.baseLayerName);
                    EditorGUILayout.Separator();

                    // Header Camera
                    EditorGUILayout.Space();
                    EditorStyles.boldLabel.fontSize = 14;
                    EditorGUILayout.LabelField("Zoom", EditorStyles.boldLabel);

                    // XX
                    settings.general.OnChangeZoomValue?.Invoke(settings.general.zoomMin, settings.general.zoomMax);
                    settings.general.zoomMin = EditorGUILayout.FloatField("Min", settings.general.zoomMin);
                    settings.general.zoomMax = EditorGUILayout.FloatField("Max", settings.general.zoomMax);

                    

                    // Header teselation
                    EditorGUILayout.Space();
                    EditorStyles.boldLabel.fontSize = 14;
                    EditorGUILayout.LabelField("Teselation", EditorStyles.boldLabel);

                    // Bundles folder
                    EditorGUILayout.BeginHorizontal();
                    settings.general.TileSize = EditorGUILayout.Vector2Field("Tile size", settings.general.TileSize, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.EndHorizontal();
                },
                keywords = new HashSet<string>(new[] { "LBS", "Tools" })
            };
            return provider;
        }


        [SettingsProvider]
        public static SettingsProvider GeneralSettingProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("LBS/General", SettingsScope.Project)
            {
                label = "General",
                guiHandler = (searchContext) =>
                {
                    var settings = Instance;

                    // header Controller paths
                    EditorGUILayout.Space();
                    EditorStyles.boldLabel.fontSize = 14;
                    EditorGUILayout.LabelField("Controller Paths", EditorStyles.boldLabel);

                    // Settings path
                    EditorGUILayout.BeginHorizontal();
                    settings.paths.settingsPath = EditorGUILayout.TextField("Settings path", settings.paths.settingsPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    if (GUILayout.Button("Find", GUILayout.MaxWidth(60)))
                    {
                        var so = DirectoryTools.GetScriptable<LBSSettings>();
                        var path = AssetDatabase.GetAssetPath(so);
                        settings.paths.settingsPath = path;
                        EditorUtility.SetDirty(settings);
                    }
                    EditorGUILayout.EndHorizontal();

                    // Storage path
                    EditorGUILayout.BeginHorizontal();
                    settings.paths.storagePath = EditorGUILayout.TextField("Storage path", settings.paths.storagePath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    if (GUILayout.Button("Find", GUILayout.MaxWidth(60)))
                    {
                        var so = DirectoryTools.GetScriptable<LBSAssetsStorage>();
                        var path = AssetDatabase.GetAssetPath(so);
                        settings.paths.storagePath = path;
                        EditorUtility.SetDirty(settings);
                    }
                    EditorGUILayout.EndHorizontal();

                    // Find All button
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Find All", GUILayout.MaxWidth(120)))
                    {
                        var so = DirectoryTools.GetScriptable<LBSSettings>();
                        var path = AssetDatabase.GetAssetPath(so);
                        settings.paths.settingsPath = path;

                        var so2 = DirectoryTools.GetScriptable<LBSAssetsStorage>();
                        var path2 = AssetDatabase.GetAssetPath(so2);
                        settings.paths.storagePath = path2;
                        EditorUtility.SetDirty(settings);
                    }
                    EditorGUILayout.EndHorizontal();

                    // header Controller paths
                    EditorGUILayout.Space();
                    EditorStyles.boldLabel.fontSize = 14;
                    EditorGUILayout.LabelField("Extra Paths", EditorStyles.boldLabel);

                    // Storage path
                    EditorGUILayout.BeginHorizontal();
                    settings.paths.iconPath = EditorGUILayout.TextField("Icons paths", settings.paths.iconPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.EndHorizontal();

                    // Set Default button
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Set default", GUILayout.MaxWidth(120)))
                    {
                        var newSettings = new LBSSettings();
                        settings.paths.iconPath = newSettings.paths.iconPath;
                        EditorUtility.SetDirty(settings);
                    }
                    EditorGUILayout.EndHorizontal();

                    // Header foplders
                    EditorGUILayout.Space();
                    EditorStyles.boldLabel.fontSize = 14;
                    EditorGUILayout.LabelField("Assets folders", EditorStyles.boldLabel);

                    // Bundles folder
                    EditorGUILayout.BeginHorizontal();
                    settings.paths.bundleFolderPath = EditorGUILayout.TextField("Bundles folder",
                        settings.paths.bundleFolderPath,
                        EditorStyles.textField,
                        GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.EndHorizontal();

                    // Tag folder
                    EditorGUILayout.BeginHorizontal();
                    settings.paths.tagFolderPath = EditorGUILayout.TextField("Tags folder",
                        settings.paths.tagFolderPath,
                        EditorStyles.textField,
                        GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.BeginHorizontal();
                    settings.paths.meshFolderPath = EditorGUILayout.TextField("Generated Mesh Folder",
                        settings.paths.meshFolderPath,
                        EditorStyles.textField,
                        GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.EndHorizontal();

                    // Set Default button
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Set default", GUILayout.MaxWidth(120)))
                    {
                        var newSettings = new LBSSettings();
                        settings.paths.bundleFolderPath = newSettings.paths.bundleFolderPath;
                        settings.paths.tagFolderPath = newSettings.paths.tagFolderPath;
                        EditorUtility.SetDirty(settings);
                    }
                    EditorGUILayout.EndHorizontal();


                    // Header foplders
                    EditorGUILayout.Space();
                    EditorStyles.boldLabel.fontSize = 14;
                    EditorGUILayout.LabelField("Pressets folders", EditorStyles.boldLabel);

                    // Bundles pressets folder
                    EditorGUILayout.BeginHorizontal();
                    settings.paths.bundlesPresetFolderPath = EditorGUILayout.TextField("Bundles pressets", settings.paths.bundlesPresetFolderPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.EndHorizontal();

                    // Generator3D presset folder
                    EditorGUILayout.BeginHorizontal();
                    settings.paths.Generator3DPresetFolderPath = EditorGUILayout.TextField("Generator3D presset", settings.paths.Generator3DPresetFolderPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.EndHorizontal();

                    // Assisstants presset folder
                    EditorGUILayout.BeginHorizontal();
                    settings.paths.assistantPresetFolderPath = EditorGUILayout.TextField("Assisstants pressets", settings.paths.assistantPresetFolderPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.EndHorizontal();

                    // Layer presset folder
                    EditorGUILayout.BeginHorizontal();
                    settings.paths.layerPressetFolderPath = EditorGUILayout.TextField("Layer presset", settings.paths.layerPressetFolderPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.EndHorizontal();

                    // Set Default button
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Set default", GUILayout.MaxWidth(120)))
                    {
                        var newSettings = new LBSSettings();
                        settings.paths.bundlesPresetFolderPath = newSettings.paths.bundlesPresetFolderPath;
                        settings.paths.Generator3DPresetFolderPath = newSettings.paths.Generator3DPresetFolderPath;
                        settings.paths.assistantPresetFolderPath = newSettings.paths.assistantPresetFolderPath;
                        settings.paths.layerPressetFolderPath = newSettings.paths.layerPressetFolderPath;
                        EditorUtility.SetDirty(settings);
                    }
                    EditorGUILayout.EndHorizontal();


                },
                keywords = new HashSet<string>(new[] { "LBS", "General" })

            };
            return provider;
        }


        [SettingsProvider]
        public static SettingsProvider InterfaceSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("LBS/Interface", SettingsScope.Project)
            {
                label = "Interface",
                guiHandler = (searchContext) =>
                {
                    var settings = Instance;

                    // Header Tools
                    EditorGUILayout.Space();
                    EditorStyles.boldLabel.fontSize = 14;
                    EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);

                    // Toolkit Color
                    EditorGUILayout.BeginHorizontal();
                    settings.view.newToolkitSelected = EditorGUILayout.ColorField("Toolkit selected", settings.view.newToolkitSelected);
                    EditorGUILayout.EndHorizontal();

                    // Header Inspectors
                    EditorGUILayout.Space();
                    EditorStyles.boldLabel.fontSize = 14;
                    EditorGUILayout.LabelField("Inspectors", EditorStyles.boldLabel);

                    // Bundles Color
                    EditorGUILayout.BeginHorizontal();
                    settings.view.bundlesColor = EditorGUILayout.ColorField("Bundles color", settings.view.bundlesColor);
                    EditorGUILayout.EndHorizontal();

                    // Tags Color
                    EditorGUILayout.BeginHorizontal();
                    settings.view.tagsColor = EditorGUILayout.ColorField("Tags color", settings.view.tagsColor);
                    EditorGUILayout.EndHorizontal();

                    // Behaviours Color
                    EditorGUILayout.BeginHorizontal();
                    settings.view.behavioursColor = EditorGUILayout.ColorField("Behaviours color", settings.view.behavioursColor);
                    EditorGUILayout.EndHorizontal();

                    // Assistants Color
                    EditorGUILayout.BeginHorizontal();
                    settings.view.assistantColor = EditorGUILayout.ColorField("Assistants color", settings.view.assistantColor);
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.Space();
                    EditorStyles.boldLabel.fontSize = 14;
                    EditorGUILayout.LabelField("Notifications", EditorStyles.boldLabel);
                    
                    // Notification Color
                    settings.view.errorColor = EditorGUILayout.ColorField("Error Color", settings.view.errorColor);
                    settings.view.warningColor = EditorGUILayout.ColorField("Warning Color", settings.view.warningColor);
                    settings.view.okColor = EditorGUILayout.ColorField("Default Color", settings.view.okColor);
                    settings.view.successColor = EditorGUILayout.ColorField("Success Color", settings.view.successColor);
                    

                    // Set Default button
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Set default", GUILayout.MaxWidth(120)))
                    {
                        var newSettings = new LBSSettings();

                        settings.view.newToolkitSelected = newSettings.view.newToolkitSelected;

                        settings.view.bundlesColor = newSettings.view.bundlesColor;
                        settings.view.tagsColor = newSettings.view.tagsColor;
                        settings.view.behavioursColor = newSettings.view.behavioursColor;
                        settings.view.assistantColor = newSettings.view.assistantColor;
                        EditorUtility.SetDirty(settings);
                    }
                    EditorGUILayout.EndHorizontal();
                },

                keywords = new HashSet<string>(new[] { "LBS", "Layers" })

            };
            return provider;
        }
    }
}