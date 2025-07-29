﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using PathOS;
using Malee.Editor;

/*
PathOSMainInspector.cs 
PathOSMainInspector (c) Nine Penguins (Samantha Stahlke) 2018
*/

[CustomEditor(typeof(PathOSManager))]
public class PathOSMainInspector : Editor
{
    private PathOSManager manager;
    private SerializedObject serial;

    private GUIStyle foldoutStyle = GUIStyle.none;

    /* Basic Properties */
    private SerializedProperty limitSimulationTime;
    private SerializedProperty maxSimulationTime;
    private SerializedProperty endOnCompletionGoal;
    private SerializedProperty endSimulationOnDeath;
    private SerializedProperty showLevelMarkup;
    private GUIContent completionLabel, deathLabel;

    /* Level Markup */
    private static bool showMarkup = false;
    private GameObject selection = null;

    /* Level Entity List */
    private static bool showList = false, showIgnored = false;
    private bool warnedEntityNull = false;
    private ReorderableList entityListReorderable, ignoredListReorderable;
    private SerializedProperty heuristicWeights;

    /* Heuristic Weight Matrix */
    private static bool showWeights = false;
    private PathOS.Heuristic weightMatrixRowID;
    private int entityPopupId = 0;
    private string[] entityLabelList;
    private PathOS.EntityType weightMatrixColumnID;
    private int heuristicPopupId = 0;
    private string[] heuristicLabelList;

    private Dictionary<Heuristic, int> heuristicIndices;
    private Dictionary<EntityType, int> entypeIndices;

    private Dictionary<(Heuristic, EntityType), float> weightLookup;

    private bool transposeWeightMatrix;
    private string ignoredLabel = "Add the Mesh Renderers of GameObjects you'd like the Level Markup to ignore";


    //Utility class for level markup toggles.
    private class MarkupToggle
    {
        public static GUIStyle style;

        public EntityType entityType;
        public bool isClear;

        public Texture2D icon;
        public Texture2D cursor;
        private GUIContent content;

        public string label;

        public bool active;

        public MarkupToggle(EntityType entityType, string label,
            Texture2D icon, Texture2D cursor, bool isClear = false)
        {
            this.entityType = entityType;
            this.label = label;
            this.icon = icon;
            this.cursor = cursor;
            this.isClear = isClear;

            content = new GUIContent(icon);
        }

        public bool Layout()
        {
            //Toggle style - should be the same as small Editor buttons.
            //Check is placed here to avoid Unity not having initialized/
            //properly referenced the EditorStyles on load.
            if (null == style)
            {
                style = EditorStyles.miniButton;
                style.fixedHeight = 32.0f;
                style.fixedWidth = 32.0f;
            }

            style = EditorStyles.miniButton;
            style.fixedHeight = 32.0f;
            style.fixedWidth = 32.0f;

            GUILayout.BeginHorizontal();

            active = GUILayout.Toggle(active, content, style);
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label(label);
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            return active;
        }
    }

    private List<MarkupToggle> markupToggles = new List<MarkupToggle>();
    private MarkupToggle activeToggle = null;

    private void OnEnable()
    {
        //Grab references to the serialized version of the manager.
        manager = (PathOSManager)target;
        serial = new SerializedObject(manager);

        //Grab properties.
        limitSimulationTime = serial.FindProperty("limitSimulationTime");
        maxSimulationTime = serial.FindProperty("maxSimulationTime");
        endOnCompletionGoal = serial.FindProperty("endOnCompletionGoal");
        endSimulationOnDeath = serial.FindProperty("endSimulationOnDeath");
        showLevelMarkup = serial.FindProperty("showLevelMarkup");

        completionLabel = new GUIContent("Final Goal Triggers End");
        deathLabel = new GUIContent("Death Triggers End");

        heuristicWeights = serial.FindProperty("heuristicWeights");

        entityListReorderable = new ReorderableList(serial.FindProperty("levelEntities"));
        entityListReorderable.elementNameProperty = "Level Entities";


        ignoredListReorderable = new ReorderableList(serial.FindProperty("ignoredEntities"));
        ignoredListReorderable.elementNameProperty = "Ignored Entities";

        //Build weight matrix.
        heuristicIndices = new Dictionary<Heuristic, int>();
        entypeIndices = new Dictionary<EntityType, int>();

        int index = 0;

        foreach(Heuristic heuristic in System.Enum.GetValues(typeof(Heuristic)))
        {
            heuristicIndices.Add(heuristic, index);
            ++index;
        }

        index = 0;

        foreach(EntityType entype in System.Enum.GetValues(typeof(EntityType)))
        {
            entypeIndices.Add(entype, index);
            ++index;
        }

        weightLookup = new Dictionary<(Heuristic, EntityType), float>();

        manager.ResizeWeightMatrix();
        BuildWeightDictionary();

        entityLabelList = new string[UI.entityLabels.Count];
        for(int i = 0; i < UI.entityLabels.Count; ++i)
        {
            entityLabelList[i] = UI.entityLabels.Values[i];
        }

        heuristicLabelList = new string[UI.heuristicLabels.Count];
        for(int i = 0; i < UI.heuristicLabels.Count; ++i)
        {
            heuristicLabelList[i] = UI.heuristicLabels.Values[i];
        }


        //Set up toggles for level markup.
        foreach (EntityType entype in System.Enum.GetValues(typeof(EntityType)))
        {
            markupToggles.Add(new MarkupToggle(entype,
                manager.entityLabelLookup[entype],
                Resources.Load<Texture2D>(manager.entityGizmoLookup[entype]),
                Resources.Load<Texture2D>("cursor_" + manager.entityGizmoLookup[entype])));
        }

        markupToggles.Add(new MarkupToggle(EntityType.ET_NONE,
            "Clear Markup (remove from entity list)",
            Resources.Load<Texture2D>("delete"),
            Resources.Load<Texture2D>("cursor_delete"),
            true));

        warnedEntityNull = false;
    }

    public override void OnInspectorGUI()
    {
        serial.Update();

        //Placed here since Unity seems to have issues with having these 
        //styles initialized on enable sometimes.
        foldoutStyle = EditorStyles.foldout;
        foldoutStyle.fontStyle = FontStyle.Bold;

        //Show basic properties.
        EditorGUILayout.PropertyField(limitSimulationTime);
        if (limitSimulationTime.boolValue) EditorGUILayout.PropertyField(maxSimulationTime);
        EditorGUILayout.PropertyField(endOnCompletionGoal, completionLabel);
        EditorGUILayout.PropertyField(endSimulationOnDeath, deathLabel);
        EditorGUILayout.PropertyField(showLevelMarkup);

        //Level markup panel.
        showMarkup = EditorGUILayout.Foldout(
            showMarkup, "Level Markup", foldoutStyle);

        if (showMarkup)
        {
            for(int i = 0; i < markupToggles.Count; ++i)
            {
                if(markupToggles[i].Layout())
                    ActivateToggle(markupToggles[i]);
            }
        }

        //Stop using the markup tool if escape is pressed.
        if (Event.current.type == EventType.KeyDown
           && Event.current.keyCode == KeyCode.Escape)
        {
            ActivateToggle(null);
            Repaint();
        }


        //Ignored List
        showIgnored = EditorGUILayout.Foldout(
            showIgnored, "Ignored Entity List", foldoutStyle);

        if (showIgnored)
        {
            EditorGUILayout.LabelField(ignoredLabel);
            ignoredListReorderable.DoLayoutList();
        }

        //Entity list.
        showList = EditorGUILayout.Foldout(
            showList, "Level Entity List", foldoutStyle);


        EditorGUI.BeginChangeCheck();

        if (showList)
            entityListReorderable.DoLayoutList();
        
        if (EditorGUI.EndChangeCheck())
            warnedEntityNull = false;

        if (!warnedEntityNull)
        {
            for (int i = 0; i < manager.levelEntities.Count; ++i)
            {
                if (null == manager.levelEntities[i].objectRef)
                {
                    NPDebug.LogWarning("One or more level entities in the scene " +
                        "is null and will be ignored during play.");

                    warnedEntityNull = true;
                    break;
                }
            }
        }

        //Heuristic weight matrix.
        showWeights = EditorGUILayout.Foldout(
            showWeights, "Motive Weights", foldoutStyle);

        //Heuristic weight matrix.
        if (showWeights)
        {
            //Sync displayed values with values stored in manager.
            RefreshWeightDictionary();

            string transposeButtonText = "View by Entity Type";

            if (transposeWeightMatrix)
                transposeButtonText = "View by Motive";

            if (GUILayout.Button(transposeButtonText))
                transposeWeightMatrix = !transposeWeightMatrix;

            if (transposeWeightMatrix)
            {
                entityPopupId = EditorGUILayout.Popup("Entity type: ", entityPopupId, entityLabelList);
                weightMatrixColumnID = UI.entityLookup[entityLabelList[entityPopupId]];
            }
            else
            {
                heuristicPopupId = EditorGUILayout.Popup("Motive: ", heuristicPopupId, heuristicLabelList);
                weightMatrixRowID = UI.heuristicLookup[heuristicLabelList[heuristicPopupId]];
            }

            Heuristic curHeuristic;
            EntityType curEntityType;

            System.Type indexType = (transposeWeightMatrix) ? typeof(Heuristic) : typeof(EntityType);

            foreach (var index in System.Enum.GetValues(indexType))
            {
                curHeuristic = (transposeWeightMatrix) ? (Heuristic)(index) : weightMatrixRowID;
                curEntityType = (transposeWeightMatrix) ? weightMatrixColumnID : (EntityType)(index);

                string label = (transposeWeightMatrix) ? 
                    PathOS.UI.heuristicLabels[curHeuristic] : 
                    PathOS.UI.entityLabels[curEntityType];

                if (!weightLookup.ContainsKey((curHeuristic, curEntityType)))
                    continue;

                EditorGUI.BeginChangeCheck();

                float newWeight = EditorGUILayout.FloatField(label, weightLookup[(curHeuristic, curEntityType)]);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(manager, "Change Motive Weight");

                    weightLookup[(curHeuristic, curEntityType)] = newWeight;

                    manager.heuristicWeights[heuristicIndices[curHeuristic]].
                    weights[entypeIndices[curEntityType]].weight =
                    weightLookup[(curHeuristic, curEntityType)];
                }

            }

            if (GUILayout.Button("Refresh Matrix"))
            {
                manager.ResizeWeightMatrix();
                BuildWeightDictionary();
            }

            if (GUILayout.Button("Export Weights..."))
            {
                string exportPath = EditorUtility.SaveFilePanel("Export Weights...", Application.dataPath, "heuristic-weights", "csv");
                manager.ExportWeights(exportPath);
            }

            if (GUILayout.Button("Import Weights..."))
            {
                string importPath = EditorUtility.OpenFilePanel("Import Weights...", Application.dataPath, "csv");

                Undo.RecordObject(manager, "Import Motive Weights");
                Undo.RecordObject(manager, "Import Evaluation");

                if (manager.ImportWeights(importPath))
                {
                    EditorUtility.SetDirty(manager);
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            }
        }

        serial.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }

    private void OnSceneGUI()
    {
        if (activeToggle != null)
        {
            //Use the current markup icon as the cursor.
            Cursor.SetCursor(activeToggle.cursor, Vector2.zero, CursorMode.Auto);
            EditorGUIUtility.AddCursorRect(new Rect(0.0f, 0.0f, 10000.0f, 10000.0f), MouseCursor.CustomCursor);

            //Selection update.
            if (EditorWindow.mouseOverWindow != null &&
                EditorWindow.mouseOverWindow.ToString() == " (UnityEditor.SceneView)")
            {
                if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.MouseDrag)
                    selection = HandleUtility.PickGameObject(Event.current.mousePosition, true, manager.ignoredEntities.ToArray());
            }
            else
                selection = null;

            //Mark up the current selection.
            if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
            {
                if (Event.current.button == 0)
                {
                    Event.current.Use();

                    int passiveControlId = GUIUtility.GetControlID(FocusType.Passive);
                    GUIUtility.hotControl = passiveControlId;

                    if (selection != null)
                    {
                        Undo.RecordObject(manager, "Edit Level Markup");
                        EditorUtility.SetDirty(manager);
                        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

                        int selectedID = selection.GetInstanceID();

                        bool addNewEntry = !activeToggle.isClear;

                        for (int i = 0; i < manager.levelEntities.Count; ++i)
                        {
                            if (manager.levelEntities[i].objectRef.GetInstanceID() == selectedID)
                            {
                                if (activeToggle.isClear)
                                    manager.levelEntities.RemoveAt(i);
                                else
                                    manager.levelEntities[i].entityType = activeToggle.entityType;

                                addNewEntry = false;
                                break;
                            }
                        }

                        if (addNewEntry)
                            manager.levelEntities.Add(new LevelEntity(selection, activeToggle.entityType));
                    }
                }
            }

            //Stop using the markup tool if escape key is pressed.
            else if (Event.current.type == EventType.KeyDown
           && Event.current.keyCode == KeyCode.Escape)
            {
                ActivateToggle(null);
                Repaint();
            }
        }

        manager.curMouseover = selection;
    }

    private void BuildWeightDictionary()
    {
        weightLookup.Clear();

        for(int i = 0; i < manager.heuristicWeights.Count; ++i)
        {
            for(int j = 0; j < manager.heuristicWeights[i].weights.Count; ++j)
            {
                weightLookup.Add((manager.heuristicWeights[i].heuristic,
                    manager.heuristicWeights[i].weights[j].entype),
                    manager.heuristicWeights[i].weights[j].weight);
            }
        }
    }

    private void RefreshWeightDictionary()
    {
        for (int i = 0; i < manager.heuristicWeights.Count; ++i)
        {
            for (int j = 0; j < manager.heuristicWeights[i].weights.Count; ++j)
            {
                weightLookup[(manager.heuristicWeights[i].heuristic,
                    manager.heuristicWeights[i].weights[j].entype)] =
                    manager.heuristicWeights[i].weights[j].weight;
            }
        }
    }

    private void ActivateToggle(MarkupToggle toggle)
    {
        if(activeToggle != null)
            activeToggle.active = false;

        activeToggle = toggle;

        if(null == activeToggle)
        {
            selection = null;
            GUIUtility.hotControl = 0;
        }
        else
        {
            activeToggle.active = true;
            manager.showLevelMarkup = true;
        }
    }
}
