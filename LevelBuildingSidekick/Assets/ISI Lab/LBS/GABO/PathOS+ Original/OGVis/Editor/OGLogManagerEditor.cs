using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/*
OGLogManagerEditor.cs
OGLogManagerEditor (c) Nine Penguins (Samantha Stahlke) 2019
*/

[CustomEditor(typeof(OGLogManager))]
public class OGLogManagerEditor : Editor
{
    //Core object references.
    private OGLogManager manager;
    private SerializedObject serial;

    //Stylization.
    private const int pathDisplayLength = 32;
    private GUIStyle errorStyle = new GUIStyle();

    private string logDirectoryDisplay;
    private string defaultDialogDirectory;

    private SerializedProperty enableLogging;
    private SerializedProperty logFilePrefix;
    private SerializedProperty sampleRate;

    //Colors
    private Color bgColor, btnColor, btnColorLight, btnColorDark;
    private void OnEnable()
    {
        //Background color
        bgColor = GUI.backgroundColor;
        btnColor = new Color32(200, 203, 224, 255);
        btnColorLight = new Color32(229, 231, 241, 255);
        btnColorDark = new Color32(136, 143, 191, 255);

        manager = (OGLogManager)target;
        serial = new SerializedObject(manager);

        enableLogging = serial.FindProperty("enableLogging");
        logFilePrefix = serial.FindProperty("logFilePrefix");
        sampleRate = serial.FindProperty("sampleRate");

        PathOS.UI.TruncateStringHead(manager.logDirectory,
            ref logDirectoryDisplay, pathDisplayLength);

        errorStyle.normal.textColor = Color.red;

        //Need to chop off "/Assets" - 7 characters.
        defaultDialogDirectory = Application.dataPath.Substring(0,
            Application.dataPath.Length - 7);
    }

    public override void OnInspectorGUI()
    {
        serial.Update();

        EditorGUILayout.PropertyField(enableLogging);

        if (!enableLogging.boolValue)
        {
            EditorGUILayout.HelpBox("WARNING: LOGGING NOT ENABLED", MessageType.Error);
            serial.ApplyModifiedProperties();
            return;
        }

        EditorGUILayout.LabelField("Log Directory: ", logDirectoryDisplay);

        GUI.backgroundColor = btnColorLight;
        if (GUILayout.Button("Browse..."))
        {
            string defaultDirectory = (manager.LogDirectoryValid()) ?
                manager.logDirectory : defaultDialogDirectory;

            string selectedPath = EditorUtility.OpenFolderPanel("Select Folder...",
                defaultDirectory, "");

            if (selectedPath != "")
            {
                manager.logDirectory = selectedPath;

                EditorUtility.SetDirty(manager);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

            PathOS.UI.TruncateStringHead(manager.logDirectory,
                ref logDirectoryDisplay, pathDisplayLength);
        }
        GUI.backgroundColor = bgColor;

        if (!manager.LogDirectoryValid())
        {
            EditorGUILayout.HelpBox("WARNING: CHOOSE A VALID DIRECTORY OUTSIDE THE ASSETS FOLDER", MessageType.Error);
        }

        EditorGUILayout.PropertyField(logFilePrefix);
        EditorGUILayout.PropertyField(sampleRate);

        serial.ApplyModifiedProperties();
    }
}
