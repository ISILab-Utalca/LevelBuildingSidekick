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

                EditorGUILayout.Space();
                EditorStyles.boldLabel.fontSize = 14;
                EditorGUILayout.LabelField("Zoom", EditorStyles.boldLabel);

                settings.general.OnChangeZoomValue(settings.general.zoomMin, settings.general.zoomMax);
                settings.general.zoomMin = EditorGUILayout.FloatField("Min",settings.general.zoomMin);
                settings.general.zoomMax = EditorGUILayout.FloatField("Max", settings.general.zoomMax);
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
                settings.settingsPath = EditorGUILayout.TextField("Settings path",settings.settingsPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                if(GUILayout.Button("Find", GUILayout.MaxWidth(60)))
                {
                    var so = Utility.DirectoryTools.GetScriptable<LBSSettings>();
                    var path = AssetDatabase.GetAssetPath(so);
                    settings.settingsPath = path;
                }
                EditorGUILayout.EndHorizontal();

                // Storage path
                EditorGUILayout.BeginHorizontal();
                settings.storagePath = EditorGUILayout.TextField("Storage path", settings.storagePath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                if (GUILayout.Button("Find", GUILayout.MaxWidth(60)))
                {
                    var so = Utility.DirectoryTools.GetScriptable<LBSAssetsStorage>();
                    var path = AssetDatabase.GetAssetPath(so);
                    settings.storagePath = path;
                }
                EditorGUILayout.EndHorizontal();

                // Pressets path
                EditorGUILayout.BeginHorizontal();
                settings.pressetsPath = EditorGUILayout.TextField("Presets path", settings.pressetsPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                if (GUILayout.Button("Find", GUILayout.MaxWidth(60)))
                {
                    var so = Utility.DirectoryTools.GetScriptable<LBSPresets>();
                    var path = AssetDatabase.GetAssetPath(so);
                    settings.pressetsPath = path;
                }
                EditorGUILayout.EndHorizontal();

                // Find All button
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Find All", GUILayout.MaxWidth(120)))
                {
                    var so = Utility.DirectoryTools.GetScriptable<LBSSettings>();
                    var path = AssetDatabase.GetAssetPath(so);
                    settings.settingsPath = path;

                    var so2 = Utility.DirectoryTools.GetScriptable<LBSAssetsStorage>();
                    var path2 = AssetDatabase.GetAssetPath(so2);
                    settings.storagePath = path2;
                }
                EditorGUILayout.EndHorizontal();

                // header Controller paths
                EditorGUILayout.Space();
                EditorStyles.boldLabel.fontSize = 14;
                EditorGUILayout.LabelField("Extra Paths", EditorStyles.boldLabel);

                // Storage path
                EditorGUILayout.BeginHorizontal();
                settings.iconPath = EditorGUILayout.TextField("Icons paths", settings.iconPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                EditorGUILayout.EndHorizontal();

                // Set Default button
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Set default", GUILayout.MaxWidth(120)))
                {
                    var newSettings = new LBSSettings();
                    settings.iconPath = newSettings.iconPath;
                }
                EditorGUILayout.EndHorizontal();

                // Header foplders
                EditorGUILayout.Space();
                EditorStyles.boldLabel.fontSize = 14;
                EditorGUILayout.LabelField("Assets folders", EditorStyles.boldLabel);

                // Bundles folder
                EditorGUILayout.BeginHorizontal();
                settings.bundleFolderPath = EditorGUILayout.TextField("Bundles folder", settings.bundleFolderPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                EditorGUILayout.EndHorizontal();

                // Tag folder
                EditorGUILayout.BeginHorizontal();
                settings.tagFolderPath = EditorGUILayout.TextField("Tags folder", settings.tagFolderPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                EditorGUILayout.EndHorizontal();

                // Set Default button
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Set default", GUILayout.MaxWidth(120)))
                {
                    var newSettings = new LBSSettings();
                    settings.bundleFolderPath = newSettings.bundleFolderPath;
                    settings.tagFolderPath = newSettings.tagFolderPath;
                }
                EditorGUILayout.EndHorizontal();


                // Header foplders
                EditorGUILayout.Space();
                EditorStyles.boldLabel.fontSize = 14;
                EditorGUILayout.LabelField("Pressets folders", EditorStyles.boldLabel);

                // Bundles pressets folder
                EditorGUILayout.BeginHorizontal();
                settings.bundlesPresetFolderPath = EditorGUILayout.TextField("Bundles pressets", settings.bundlesPresetFolderPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                EditorGUILayout.EndHorizontal();

                // Generator3D presset folder
                EditorGUILayout.BeginHorizontal();
                settings.Generator3DPresetFolderPath = EditorGUILayout.TextField("Generator3D presset", settings.Generator3DPresetFolderPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                EditorGUILayout.EndHorizontal();

                // Assisstants presset folder
                EditorGUILayout.BeginHorizontal();
                settings.assistantPresetFolderPath = EditorGUILayout.TextField("Assisstants pressets", settings.assistantPresetFolderPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                EditorGUILayout.EndHorizontal();

                // Layer presset folder
                EditorGUILayout.BeginHorizontal();
                settings.layerPressetFolderPath = EditorGUILayout.TextField("Layer presset", settings.layerPressetFolderPath, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                EditorGUILayout.EndHorizontal();

                // Set Default button
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Set default", GUILayout.MaxWidth(120)))
                {
                    var newSettings = new LBSSettings();
                    settings.bundlesPresetFolderPath = newSettings.bundlesPresetFolderPath;
                    settings.Generator3DPresetFolderPath = newSettings.Generator3DPresetFolderPath;
                    settings.assistantPresetFolderPath = newSettings.assistantPresetFolderPath;
                    settings.layerPressetFolderPath = newSettings.layerPressetFolderPath;
                }
                EditorGUILayout.EndHorizontal();

                EditorUtility.SetDirty(settings);
            },
            keywords = new HashSet<string>(new[] { "LBS", "General" })

        };
        return provider;
    }

    [SettingsProvider]
    public static SettingsProvider ModulesSettingsProvider()
    {
        // First parameter is the path in the Settings window.
        // Second parameter is the scope of this setting: it only appears in the Project Settings window.
        var provider = new SettingsProvider("LBS/Modules", SettingsScope.Project)
        {
            label = "Modules",
            guiHandler = (searchContext) =>
            {
                var settings = LBSSettings.Instance;

                // Header teselation
                EditorGUILayout.Space();
                EditorStyles.boldLabel.fontSize = 14;
                EditorGUILayout.LabelField("Teselation", EditorStyles.boldLabel);

                // Bundles folder
                EditorGUILayout.BeginHorizontal();
                settings.TileSize = EditorGUILayout.Vector2Field("Tile size", settings.TileSize, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                EditorGUILayout.EndHorizontal();


                // Header graph
                EditorGUILayout.Space();
                EditorStyles.boldLabel.fontSize = 14;
                EditorGUILayout.LabelField("Graph", EditorStyles.boldLabel);
                // IMPLEMENTAR (!!!)

            },
            keywords = new HashSet<string>(new[] { "LBS", "Modules", "Teselation", "Graph" })

        };
        return provider;
    }

    [SettingsProvider]
    public static SettingsProvider AssisstantsSettingsProvider()
    {
        // First parameter is the path in the Settings window.
        // Second parameter is the scope of this setting: it only appears in the Project Settings window.
        var provider = new SettingsProvider("LBS/Assistants", SettingsScope.Project)
        {
            label = "Assistants",
            guiHandler = (searchContext) =>
            {
                var settings = LBSSettings.Instance;

                // Header base optimizer
                EditorGUILayout.Space();
                EditorStyles.boldLabel.fontSize = 14;
                EditorGUILayout.LabelField("Base optimizer", EditorStyles.boldLabel);
                // IMPLEMENTAR (!!!)

                // Header map elite
                EditorGUILayout.Space();
                EditorStyles.boldLabel.fontSize = 14;
                EditorGUILayout.LabelField("Map elite", EditorStyles.boldLabel);
                // IMPLEMENTAR (!!!)

                // Header WFC
                EditorGUILayout.Space();
                EditorStyles.boldLabel.fontSize = 14;
                EditorGUILayout.LabelField("Wave Function Collapse", EditorStyles.boldLabel);
                // IMPLEMENTAR (!!!)
            },

            keywords = new HashSet<string>(new[] { "LBS", "Assistants" })

        };
        return provider;
    }

    [SettingsProvider]
    public static SettingsProvider LayersSettingsProvider()
    {
        // First parameter is the path in the Settings window.
        // Second parameter is the scope of this setting: it only appears in the Project Settings window.
        var provider = new SettingsProvider("LBS/Layers", SettingsScope.Project)
        {
            label = "Layers",
            guiHandler = (searchContext) =>
            {
                var settings = LBSSettings.Instance;

            },

            keywords = new HashSet<string>(new[] { "LBS", "Layers" })

        };
        return provider;
    }

    [SettingsProvider]
    public static SettingsProvider Generators3DSettingsProvider()
    {
        // First parameter is the path in the Settings window.
        // Second parameter is the scope of this setting: it only appears in the Project Settings window.
        var provider = new SettingsProvider("LBS/3D generators", SettingsScope.Project)
        {
            label = "3D Generators",
            guiHandler = (searchContext) =>
            {
                var settings = LBSSettings.Instance;

            },

            keywords = new HashSet<string>(new[] { "LBS", "Generators", "3D" })

        };
        return provider;
    }
}