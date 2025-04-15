using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using OGVis;


/*
OGVisEditor.cs
OGVisEditor (c) Ominous Games 2018 Atiya Nova 2021

This class manages the Unity Inspector pane for vis customization.
*/

[ExecuteInEditMode]
[CustomEditor(typeof(OGLogVisualizer))]
public class OGVisEditor : Editor
{
    //Core object references.
    private OGLogVisualizer vis;
    private SerializedObject serial;

    private const int pathDisplayLength = 32;
    private GUIStyle errorStyle = new GUIStyle();

    private string logDirectoryDisplay;
    private string defaultDialogDirectory;

    private SerializedProperty propLogDirectory;

    private SerializedProperty propDisplayHeight;

    private GUIStyle expansionToggleStyle;
    private GUIContent expansionLabel = new GUIContent("...", "Show agent profile");
    private GUIContent noContent = new GUIContent("");

    //Heatmap.
    private SerializedProperty propHeatmapAlpha;
    private SerializedProperty propHeatmapTileSize;

    private SerializedProperty propHeatmapAggregate;
    private GUIContent toggleAggregateLabel = new GUIContent("Active Agents Only",
                "Only include data from agents\nchecked in the filtering tab");

    private SerializedProperty propHeatmapTimeSlice;
    private GUIContent toggleTimeSliceLabel = new GUIContent("Use Time Range",
                "Only include data within the range\nspecified in the filtering tab");

    private PathOSAgent agentReference;
    private List<PathOS.Heuristic> heuristics = new List<PathOS.Heuristic>();

    private SerializedProperty propShowIndividual;
    private SerializedProperty propShowIndividualInteractions;
    private GUIContent individualInteractionsLabel = new GUIContent("Individual Interactions",
                "Show visited entities along\nindividual agent paths");

    private Texture2D polylinetex;

    //Interaction display settings.
    private static bool showLabels = false;

    private SerializedProperty propShowEntities;
    private SerializedProperty propShowArrows;
    private SerializedProperty propEntityAggregate;
    private SerializedProperty propEntityTimeSlice;

    //Interaction display settings.
    private static int tabSelection = 0;
    string[] tabLabels = { "Heatmaps", "Individual Paths", "Entity Interactions" };

    //Time scrubbing display settings
    private string lblLoadFoldout = "File Loading Options";
    private static bool loadFoldout = false;

    //Colors
    private Color bgColor, btnColor, btnColorLight, btnColorDark, bgDark1, bgDark2, bgDark3;

    //Timestamp variables

    //Called when the inspector pane is initialized.
    private void OnEnable()
    {
        //Background color
        bgColor = GUI.backgroundColor;
        btnColor = new Color32(200, 203, 224, 180);
        btnColorLight = new Color32(229, 231, 241, 180);
        btnColorDark = new Color32(158, 164, 211, 180);
        bgDark1 = new Color32(184, 187, 199, 100);
        bgDark2 = new Color32(224, 225, 230, 120);
        bgDark3 = new Color32(224, 225, 230, 80);

        //Grab our sharp line texture - this looks nicer on screen than the default.
        polylinetex = Resources.Load("polylinetex") as Texture2D;

        //Grab a reference to the serialized representation of the visualization.
        vis = (OGLogVisualizer)target;
        serial = new SerializedObject(vis);

        //These serialized properties will let us skip some of the legwork in displaying
        //interactive widgets in the inspector.
        propLogDirectory = serial.FindProperty("logDirectory");

        propDisplayHeight = serial.FindProperty("displayHeight");

        propHeatmapAlpha = serial.FindProperty("heatmapAlpha");
        propHeatmapTileSize = serial.FindProperty("tileSize");
        propHeatmapAggregate = serial.FindProperty("heatmapAggregateActiveOnly");
        propHeatmapTimeSlice = serial.FindProperty("heatmapUseTimeSlice");

        propShowIndividual = serial.FindProperty("showIndividualPaths");
        propShowArrows = serial.FindProperty("showDirectionArrows");
        propShowIndividualInteractions = serial.FindProperty("showIndividualInteractions");

        propShowEntities = serial.FindProperty("showEntities");
        propEntityAggregate = serial.FindProperty("interactionAggregateActiveOnly");
        propEntityTimeSlice = serial.FindProperty("interactionUseTimeSlice");

        PathOS.UI.TruncateStringHead(vis.logDirectory,
            ref logDirectoryDisplay, pathDisplayLength);

        //Need to chop off "/Assets" - 7 characters.
        defaultDialogDirectory = Application.dataPath.Substring(0,
            Application.dataPath.Length - 7);

        errorStyle.normal.textColor = Color.red;

        heuristics.Clear();
    }

    //Draws the inspector pane itself.
    public override void OnInspectorGUI()
    {
        //Make sure our vis representation is up-to-date.
        serial.Update();

        GUI.backgroundColor = bgDark1;
        EditorGUILayout.BeginVertical("Box");

        //Collapsible display options pane.
        loadFoldout = EditorGUILayout.Foldout(loadFoldout, lblLoadFoldout);

        GUI.backgroundColor = bgColor;

        if (loadFoldout)
        {   
            EditorGUILayout.LabelField("Load Directory: ", logDirectoryDisplay);

            GUI.backgroundColor = btnColorLight;
            if (GUILayout.Button("Browse..."))
            {
                string defaultDirectory = (Directory.Exists(vis.logDirectory)) ?
                    vis.logDirectory : defaultDialogDirectory;

                string selectedPath = EditorUtility.OpenFolderPanel("Select Folder...",
                    defaultDirectory, "");

                if (selectedPath != "")
                {
                    vis.logDirectory = selectedPath;

                    EditorUtility.SetDirty(vis);
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }

                PathOS.UI.TruncateStringHead(vis.logDirectory,
                    ref logDirectoryDisplay, pathDisplayLength);
            }

            if (!Directory.Exists(vis.logDirectory))
            {
                EditorGUILayout.LabelField("Error! You must choose a " +
                    "valid folder on this computer.", errorStyle);
            }


            //Log file loading/management.
            if (GUILayout.Button("Add Files from " + logDirectoryDisplay + "/"))
            {
                //Add log files, if the provided directory is valid.
                vis.LoadLogs();
            }

            if (GUILayout.Button("Clear All Data"))
            {
                //Clear all logs and records from the current session.
                vis.ClearData();
                Debug.Log("Cleared all visualization data.");
            }
            GUI.backgroundColor = bgColor;
        }

        if (!vis.IsDataInitialized())
        {
            serial.ApplyModifiedProperties();
            EditorGUILayout.HelpBox("WARNING: NO FILES LOADED", MessageType.Error);
            SceneView.RepaintAll();
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(5);

        GUI.backgroundColor = btnColor;
        EditorGUILayout.BeginVertical("Box");
        GUI.backgroundColor = bgColor;
        //  EditorGUILayout.LabelField("Time Range", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        PathOS.EditorUI.FullMinMaxSlider("Time Range",
            ref vis.displayTimeRange.min,
            ref vis.displayTimeRange.max,
            vis.fullTimeRange.min,
            vis.fullTimeRange.max);

        if (EditorGUI.EndChangeCheck())
        {
            vis.ApplyDisplayRange();
            vis.ReclusterEvents();
            vis.ApplyHeatmapSettings();
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(5);

        // The tabs to alternate between specific menus

        GUI.backgroundColor = btnColorDark;
        EditorGUILayout.BeginVertical("Box");
        GUI.backgroundColor = bgColor;

        EditorGUILayout.LabelField("Visualization Settings", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        tabSelection = GUILayout.Toolbar(tabSelection, tabLabels);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = bgColor;

        //Switches between the tabs (temp solution, todo: clean this up when you get the time)
        switch (tabSelection)
        {
            case 0:

                EditorGUI.BeginChangeCheck();
                vis.showHeatmap = EditorGUILayout.Toggle("Show Heatmap", vis.showHeatmap);

                if (EditorGUI.EndChangeCheck())
                    vis.UpdateHeatmapVisibility();

                if (!vis.showHeatmap) break;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Heatmap Colours", GUILayout.Width(PathOS.UI.longLabelWidth));

                EditorGUILayout.LabelField("Low", GUILayout.Width(PathOS.UI.mediumLabelWidth));
                vis.heatmapGradient = EditorGUILayout.GradientField(vis.heatmapGradient);
                EditorGUILayout.LabelField("High", GUILayout.Width(PathOS.UI.mediumLabelWidth));

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(propHeatmapAlpha);
                EditorGUILayout.PropertyField(propHeatmapTileSize);
                EditorGUILayout.PropertyField(propHeatmapAggregate, toggleAggregateLabel);
                EditorGUILayout.PropertyField(propHeatmapTimeSlice, toggleTimeSliceLabel);

                if (EditorGUI.EndChangeCheck())
                {
                    vis.ApplyHeatmapSettings();
                }

                break;
            case 1:

                //Global path display settings.
                EditorGUILayout.PropertyField(propShowIndividual);

                if (!vis.showIndividualPaths) break;

                EditorGUILayout.PropertyField(propShowArrows);

                EditorGUILayout.PropertyField(propShowIndividualInteractions,
                    individualInteractionsLabel);

                if (vis.pLogs.Count > 0)
                    GUILayout.Label("Agent Colors:");

                //Filter options.
                //Enable/disable players, set path colour by player ID.
                foreach (PlayerLog pLog in vis.pLogs)
                {
                    GUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(pLog.playerID);
                    pLog.pathColor = EditorGUILayout.ColorField(pLog.pathColor);

                    GUILayout.EndHorizontal();
                }
                break;

            case 2:

                EditorGUILayout.PropertyField(propShowEntities);

                if (!vis.showEntities) break;

                showLabels = EditorGUILayout.Toggle("Display Interaction Labels", showLabels);

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Interaction Gradient", GUILayout.Width(PathOS.UI.longLabelWidth));

                EditorGUILayout.LabelField("Low", GUILayout.Width(PathOS.UI.mediumLabelWidth));
                vis.interactionGradient = EditorGUILayout.GradientField(vis.interactionGradient);
                EditorGUILayout.LabelField("High", GUILayout.Width(PathOS.UI.mediumLabelWidth));

                EditorGUILayout.EndHorizontal();

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propEntityAggregate, toggleAggregateLabel);
                EditorGUILayout.PropertyField(propEntityTimeSlice, toggleTimeSliceLabel);

                if (EditorGUI.EndChangeCheck())
                {
                    vis.ReclusterEvents();
                }

                break;
        }
         EditorGUILayout.EndVertical();


        EditorGUILayout.Space(5);

        GUI.backgroundColor = btnColor;
        EditorGUILayout.BeginVertical("Box");
        GUI.backgroundColor = bgColor;

        EditorGUILayout.LabelField("Filtering Settings", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(propDisplayHeight);

        if (EditorGUI.EndChangeCheck())
        {
            vis.ApplyDisplayHeight();
            vis.ReclusterEvents();
        }

        bool refreshFilter = false;
        bool oldFilter = false;

        expansionToggleStyle = EditorStyles.miniButton;
        expansionToggleStyle.fixedHeight = 16.0f;
        expansionToggleStyle.fixedWidth = 32.0f;

        if (vis.pLogs.Count > 0)
            GUILayout.Label("Filter Data by Player ID:");


        //Filter options.
        //Enable/disable players.
        foreach (PlayerLog pLog in vis.pLogs)
        {
            GUILayout.BeginHorizontal();

            pLog.pathColor = EditorGUILayout.ColorField(noContent,
                pLog.pathColor, false, false, false, GUILayout.Width(16.0f));

            oldFilter = pLog.visInclude;
            pLog.visInclude = GUILayout.Toggle(pLog.visInclude, "", GUILayout.Width(16.0f));
            pLog.playerID = EditorGUILayout.TextField(pLog.playerID);

            if (oldFilter != pLog.visInclude)
                refreshFilter = true;

            pLog.showDetail = GUILayout.Toggle(pLog.showDetail, expansionLabel, expansionToggleStyle);

            GUILayout.EndHorizontal();

            if (pLog.showDetail)
            {
                agentReference = EditorGUILayout.ObjectField("Agent to update: ",
                    agentReference, typeof(PathOSAgent), true) as PathOSAgent;

                if (GUILayout.Button("Copy motives to agent")
                    && agentReference != null)
                {
                    Undo.RecordObject(agentReference, "Copy motives to agent");

                    agentReference.experienceScale = pLog.experience;

                    foreach (PathOS.HeuristicScale scale in agentReference.heuristicScales)
                    {
                        if (pLog.heuristics.ContainsKey(scale.heuristic))
                            scale.scale = pLog.heuristics[scale.heuristic];
                    }
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Experience:", GUILayout.Width(84.0f));
                EditorGUILayout.LabelField(pLog.experience.ToString());
                EditorGUILayout.EndHorizontal();

                foreach (KeyValuePair<PathOS.Heuristic, float> stat in pLog.heuristics)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(PathOS.UI.heuristicLabels[stat.Key] + ":",
                        GUILayout.Width(84.0f));
                    EditorGUILayout.LabelField(stat.Value.ToString());
                    EditorGUILayout.EndHorizontal();
                }

            }
        }

        GUI.backgroundColor = btnColorLight;
        GUILayout.BeginHorizontal();
        //Shortcut to enable all PIDs in the vis.
        if (GUILayout.Button("Select All"))
        {
            foreach (PlayerLog pLog in vis.pLogs)
            {
                pLog.visInclude = true;
            }

            refreshFilter = true;
        }

        //Shortcut to exclude all PIDs from the vis.
        if (GUILayout.Button("Select None"))
        {
            foreach (PlayerLog pLog in vis.pLogs)
            {
                pLog.visInclude = false;
            }

            refreshFilter = true;
        }

        GUILayout.EndHorizontal();
        GUI.backgroundColor = bgColor;

        //If we've detected a change that requires re-aggregation, do so.
        if (refreshFilter)
        {
            if (vis.interactionAggregateActiveOnly)
                vis.ReclusterEvents();

            if (vis.heatmapAggregateActiveOnly)
                vis.UpdateHeatmap();
        }

        EditorGUILayout.EndVertical();

        serial.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }


    //Draws content in the scene context.
    private void OnSceneGUI()
    {
        //Don't draw anything if the script is disabled.
        if (!vis.enabled)
            return;


        //Draw individual player paths.
        
            foreach (PlayerLog pLog in vis.pLogs)
            {
                if (pLog.visInclude)
                {
                    if (pLog.pathPoints.Count > 0)
                    {

                    if (vis.showIndividualPaths)
                    {
                        //Draw path trace.
                        Vector3[] points = pLog.pathPoints.GetRange(
                            pLog.displayStartIndex,
                            pLog.displayEndIndex - pLog.displayStartIndex + 1)
                            .ToArray();

                        //Draw arrows at set intervals
                        Handles.color = pLog.pathColor;

                        //For timestamps
                        Vector3 mousePos = Event.current.mousePosition;
                        mousePos.y = SceneView.lastActiveSceneView.camera.pixelHeight - mousePos.y;
                        Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mousePos);

                        RaycastHit hit;
                        bool timestampFound = false;

                        Event e = Event.current;

                        for (int i = 0; i < points.Length; i++)
                        {
                            if (vis.showDirectionArrows && i % 10 == 0)
                            {
                                Handles.ArrowHandleCap(
                                     0,
                                     points[i],
                                     pLog.orientations[i].rot,
                                     2.5f,
                                     EventType.Repaint
                                 );
                            }

                            if (Physics.Raycast(ray, out hit) && !timestampFound)
                            {
                                if (Vector3.Distance(hit.point, points[i]) <= 2f)
                                {
                                    Handles.Label(points[i], "Time: " + pLog.positions[i].timestamp.ToString("F0") + " Health: " + pLog.healths[i].health, GUI.skin.textArea);
                                    timestampFound = true;
                                }
                            }
                        }

                        SceneView.RepaintAll();

                        Handles.DrawAAPolyLine(polylinetex, OGLogVisualizer.PATH_WIDTH, points);
                    }

                        //Individual interactions are only shown if aggregate interactions
                        //are hidden (to prevent overlap).
                        if (vis.showIndividualInteractions && !vis.showEntities)
                        {
                            for (int i = 0; i < pLog.interactionEvents.Count; ++i)
                            {
                                PlayerLog.InteractionEvent curEvent = pLog.interactionEvents[i];

                                if (curEvent.timestamp < vis.currentTimeRange.min)
                                    continue;
                                else if (curEvent.timestamp > vis.currentTimeRange.max)
                                    break;

                                Handles.color = Color.white;
                                Handles.DrawSolidDisc(curEvent.pos, Vector3.up, OGLogVisualizer.MIN_ENTITY_RADIUS);

                                if (showLabels)
                                Handles.Label(curEvent.pos, curEvent.objectName, GUI.skin.textArea);
                            }
                        }
                    }
                }


                //Draw aggregate entity interactions.
                DrawAggregateEntityInteractions();
            }
        
    }

    private void DrawAggregateEntityInteractions()
    {
        if (vis.showEntities)
        {
            foreach (KeyValuePair<string, OGLogVisualizer.AggregateInteraction> interaction
                in vis.aggregateInteractions)
            {
                Handles.color = interaction.Value.displayColor;
                Handles.DrawSolidDisc(interaction.Value.pos, Vector3.up,
                    interaction.Value.displaySize);
            }

            if (showLabels)
            {
                //Shows the labels for the aggregate data
                foreach (KeyValuePair<string, OGLogVisualizer.AggregateInteraction> interaction
                    in vis.aggregateInteractions)
                {
                    Handles.Label(interaction.Value.pos,
                        interaction.Value.displayName, GUI.skin.textArea);
                }
            }
        }
    }    
}