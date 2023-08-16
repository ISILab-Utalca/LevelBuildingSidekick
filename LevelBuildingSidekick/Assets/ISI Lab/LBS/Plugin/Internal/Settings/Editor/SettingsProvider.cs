using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
                var settings = LBSSettings.Instance;

                // Header Camera
                EditorGUILayout.Space();
                EditorStyles.boldLabel.fontSize = 14;
                EditorGUILayout.LabelField("Zoom", EditorStyles.boldLabel);

                // XX
                settings.general.OnChangeZoomValue?.Invoke(settings.general.zoomMin, settings.general.zoomMax);
                settings.general.zoomMin = EditorGUILayout.FloatField("Min",settings.general.zoomMin);
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
                var settings = LBSSettings.Instance;

                // header Controller paths
                EditorGUILayout.Space();
                EditorStyles.boldLabel.fontSize = 14;
                EditorGUILayout.LabelField("Controller Paths", EditorStyles.boldLabel);

                // Settings path
                EditorGUILayout.BeginHorizontal();
                settings.paths.settingsPath = EditorGUILayout.TextField("Settings path", settings.paths.settingsPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                if (GUILayout.Button("Find", GUILayout.MaxWidth(60)))
                {
                    var so = Utility.DirectoryTools.GetScriptable<LBSSettings>();
                    var path = AssetDatabase.GetAssetPath(so);
                    settings.paths.settingsPath = path;
                }
                EditorGUILayout.EndHorizontal();

                // Storage path
                EditorGUILayout.BeginHorizontal();
                settings.paths.storagePath = EditorGUILayout.TextField("Storage path", settings.paths.storagePath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                if (GUILayout.Button("Find", GUILayout.MaxWidth(60)))
                {
                    var so = Utility.DirectoryTools.GetScriptable<LBSAssetsStorage>();
                    var path = AssetDatabase.GetAssetPath(so);
                    settings.paths.storagePath = path;
                }
                EditorGUILayout.EndHorizontal();

                // Pressets path
                EditorGUILayout.BeginHorizontal();
                settings.paths.pressetsPath = EditorGUILayout.TextField("Presets path", settings.paths.pressetsPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                if (GUILayout.Button("Find", GUILayout.MaxWidth(60)))
                {
                    var so = Utility.DirectoryTools.GetScriptable<LBSPresets>();
                    var path = AssetDatabase.GetAssetPath(so);
                    settings.paths.pressetsPath = path;
                }
                EditorGUILayout.EndHorizontal();

                // Find All button
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Find All", GUILayout.MaxWidth(120)))
                {
                    var so = Utility.DirectoryTools.GetScriptable<LBSSettings>();
                    var path = AssetDatabase.GetAssetPath(so);
                    settings.paths.settingsPath = path;

                    var so2 = Utility.DirectoryTools.GetScriptable<LBSAssetsStorage>();
                    var path2 = AssetDatabase.GetAssetPath(so2);
                    settings.paths.storagePath = path2;
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
                }
                EditorGUILayout.EndHorizontal();

                // Header foplders
                EditorGUILayout.Space();
                EditorStyles.boldLabel.fontSize = 14;
                EditorGUILayout.LabelField("Assets folders", EditorStyles.boldLabel);

                // Bundles folder
                EditorGUILayout.BeginHorizontal();
                settings.paths.bundleFolderPath = EditorGUILayout.TextField("Bundles folder", settings.paths.bundleFolderPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                EditorGUILayout.EndHorizontal();

                // Tag folder
                EditorGUILayout.BeginHorizontal();
                settings.paths.tagFolderPath = EditorGUILayout.TextField("Tags folder", settings.paths.tagFolderPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                EditorGUILayout.EndHorizontal();

                // Set Default button
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Set default", GUILayout.MaxWidth(120)))
                {
                    var newSettings = new LBSSettings();
                    settings.paths.bundleFolderPath = newSettings.paths.bundleFolderPath;
                    settings.paths.tagFolderPath = newSettings.paths.tagFolderPath;
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
                }
                EditorGUILayout.EndHorizontal();

                EditorUtility.SetDirty(settings);
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
                var settings = LBSSettings.Instance;

                // Header Camera
                EditorGUILayout.Space();
                EditorStyles.boldLabel.fontSize = 14;
                EditorGUILayout.LabelField("Zoom", EditorStyles.boldLabel);

                // Toolkit color
                EditorGUILayout.BeginHorizontal();
                settings.@interface.toolkitSelected = EditorGUILayout.ColorField("Toolkit selected",settings.@interface.toolkitSelected);
                EditorGUILayout.EndHorizontal();
            },

            keywords = new HashSet<string>(new[] { "LBS", "Layers" })

        };
        return provider;
    }
}