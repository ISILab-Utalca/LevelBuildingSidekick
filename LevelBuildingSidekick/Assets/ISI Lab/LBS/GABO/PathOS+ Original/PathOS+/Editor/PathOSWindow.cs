using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

/*
PathOSWindow.cs 
(Atiya Nova) 2021
 */

public class PathOSWindow : EditorWindow
{
    enum Tabs {
        Setup = 0,
        Agent = 1,
        Resources = 2,
        Batching = 3,
        Profiles = 4,
        Manager = 5,
        Visualization = 6,
        ExpertEvaluation = 7
    };

    string[] tabLabels = { "SETUP", "Agent", "Resource Values", "Batching", "Profiles", "Manager", "Visualization", "Expert Evaluation" };
    int tabSelection = 0;

    private PathOSProfileWindow profileWindow;
    private PathOSAgentBatchingWindow batchingWindow;
    private PathOSAgentWindow agentWindow;
    private static PathOSEvaluationWindow evaluationWindow;
    private static PathOSManagerWindow managerWindow;

    private GameObject proxyScreenshot;
    private ScreenshotManager screenshotReference;
    private PathOSManager managerReference;
    private PathOSAgent agentReference;

    private Vector2 scrollPos = Vector2.zero;
    private bool disableCamera = true;
    private Color bgColor, btnColorDark, redColor, greenColor;

    private GUIStyle foldoutStyle = GUIStyle.none;

    [SerializeField]
    private bool hasScreenshot, hasManager, hasAgent;

    [SerializeField]
    private int screenshotID, managerID, agentID;

    //Screenshot display settings
    private string lblNavigationFoldout = "NAVIGATION";
    private static bool showNavigation = true;

    private string editorPrefsID, sceneName;


    [MenuItem("Window/PathOS+")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PathOSWindow), false, "PathOS+");
    }

    private void OnEnable()
    {
        //Set the scene name
        sceneName = SceneManager.GetActiveScene().name;

        //Background color
        bgColor = GUI.backgroundColor;
        btnColorDark = new Color32(158, 164, 211, 255);
        redColor = new Color32(255, 60, 71, 150);
        greenColor = new Color32(60, 179, 113, 140);

        //initializes the different windows
        //profileWindow = new PathOSProfileWindow();
        //batchingWindow = new PathOSAgentBatchingWindow();
        //agentWindow = new PathOSAgentWindow();
        //managerWindow = new PathOSManagerWindow();
        //evaluationWindow = new PathOSEvaluationWindow();

        profileWindow = (PathOSProfileWindow)ScriptableObject.CreateInstance(typeof(PathOSProfileWindow)); 
        batchingWindow = (PathOSAgentBatchingWindow)ScriptableObject.CreateInstance(typeof(PathOSAgentBatchingWindow));
        agentWindow = (PathOSAgentWindow)ScriptableObject.CreateInstance(typeof(PathOSAgentWindow)); 
        managerWindow = (PathOSManagerWindow)ScriptableObject.CreateInstance(typeof(PathOSManagerWindow)); 
        evaluationWindow = (PathOSEvaluationWindow)ScriptableObject.CreateInstance(typeof(PathOSEvaluationWindow)); 

        //Re-establish references, if they have been nullified.
        if (hasScreenshot)
        {
            if (screenshotReference != null)
                screenshotID = screenshotReference.GetInstanceID();
            else
                screenshotReference = EditorUtility.InstanceIDToObject(screenshotID) as ScreenshotManager;
        }

        hasScreenshot = screenshotReference != null;

        //manager reference
        if (hasManager)
        {
            if (managerReference != null)
            {
                managerID = managerReference.GetInstanceID();
            }
            else
                managerReference = EditorUtility.InstanceIDToObject(managerID) as PathOSManager;
        }

        hasManager = managerReference != null;

        //Agent reference
        if (hasAgent)
        {
            if (agentReference != null)
                agentID = agentReference.GetInstanceID();
            else
                agentReference = EditorUtility.InstanceIDToObject(agentID) as PathOSAgent;
        }

        hasAgent = agentReference != null;
    }

    private void OnDestroy()
    {
        if (profileWindow) DestroyImmediate(profileWindow);
        if (batchingWindow) DestroyImmediate(batchingWindow);
        if (agentWindow) DestroyImmediate(agentWindow);
        if (managerWindow) DestroyImmediate(managerWindow);
        if (evaluationWindow) DestroyImmediate(evaluationWindow);
    }

    private void OnDisable()
    {
        if (profileWindow) DestroyImmediate(profileWindow);
        if (batchingWindow) DestroyImmediate(batchingWindow);
        if (agentWindow) DestroyImmediate(agentWindow);
        if (managerWindow) DestroyImmediate(managerWindow);
        if (evaluationWindow) DestroyImmediate(evaluationWindow);
    }

    //gizmo stuff from here https://stackoverflow.com/questions/37267021/unity-editor-script-visible-hidden-gizmos
    public void OnGUI()
    {
        //Sets style variables
        foldoutStyle = EditorStyles.foldout;
        foldoutStyle.fontStyle = FontStyle.Bold;

        EditorGUILayout.Space();
        scrollPos = GUILayout.BeginScrollView(scrollPos, true, true);

        GUI.backgroundColor = btnColorDark;
        EditorGUILayout.BeginVertical("Box");

        //Establishes references
        GrabAgentReference();
        GrabManagerReference();
        GrabScreenshotReference();

        agentWindow.SetAgentReference(agentReference);
        evaluationWindow.SetManagerReference(managerReference);
        managerWindow.SetManagerReference(managerReference);
        evaluationWindow.SetScreenshotReference(screenshotReference);

        //Shows tab navigation
        showNavigation = EditorGUILayout.Foldout(showNavigation, lblNavigationFoldout, foldoutStyle);
        GUI.backgroundColor = bgColor;

        if (showNavigation)
        {
            // The tabs to alternate between specific menus
            GUI.backgroundColor = btnColorDark;
            GUILayout.BeginHorizontal();
            tabSelection = GUILayout.SelectionGrid(tabSelection, tabLabels, 4);
            //PathOSEvaluationWindow.instance.SetCurrentlyOpen((tabSelection == (int)Tabs.ExpertEvaluation ? true : false));
             evaluationWindow.SetCurrentlyOpen((tabSelection==(int)Tabs.ExpertEvaluation ?  true : false));
            GUILayout.EndHorizontal();
            GUI.backgroundColor = bgColor;

        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);


        switch (tabSelection)
        {
            case (int)Tabs.Setup:
                OpenSetup();
                break;
            case (int)Tabs.Agent:
                agentWindow.OnWindowOpen();
                break;
            case (int)Tabs.Resources:
                OnResourcesOpen();
                break;
            case (int)Tabs.Batching:
                batchingWindow.OnWindowOpen();
                break;
            case (int)Tabs.Profiles:
                profileWindow.OnWindowOpen();
                break;
            case (int)Tabs.Manager:
                managerWindow.OnWindowOpen();
                break;
            case (int)Tabs.Visualization:
                managerWindow.OnVisualizationOpen();
                break;
            case (int)Tabs.ExpertEvaluation:
                //PathOSEvaluationWindow.instance.OnWindowOpen();
                evaluationWindow.OnWindowOpen();
                break;
        }
        GUILayout.EndScrollView();
    }

    private void Update()
    {
        //Temporary solution
        batchingWindow.UpdateBatching();
    }

    // GABO: Set references from outside the class
    public void SetAgentReference(PathOSAgent agent) { agentReference = agent; hasAgent = true; }
    public void SetManagerReference(PathOSManager manager) { managerReference = manager; hasManager = true; }
    public void SetScreenshotCameraReference(ScreenshotManager camera) { screenshotReference = camera; hasScreenshot = true; }

    private void OnResourcesOpen()
    {
        managerWindow.OnResourceOpen();
        agentWindow.OnResourceOpen();
    }

    private void OpenSetup()
    {
        EditorGUILayout.BeginVertical("Box");

        EditorGUILayout.LabelField("PathOS+ Agent", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        agentReference = EditorGUILayout.ObjectField("Agent Reference: ", agentReference, typeof(PathOSAgent), true)
        as PathOSAgent;

        if (EditorGUI.EndChangeCheck())
        {
            hasAgent = agentReference != null;

            if (hasAgent)
            {
                agentID = agentReference.GetInstanceID();
            }
        }

        if (agentReference == null)
        {
            GUI.backgroundColor = redColor;
            if (GUILayout.Button("Create Agent"))
            {
                GameObject proxyAgent;
                proxyAgent = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PathOS+/Prefabs/PathOS Agent.prefab") as GameObject;
                Instantiate(proxyAgent);
                agentReference = proxyAgent.GetComponent<PathOSAgent>();
            }
            GUI.backgroundColor = bgColor;
        }
        else
        {
            GUI.backgroundColor = greenColor;
            if (GUILayout.Button("Edit Agent"))
            {
                tabSelection = (int)Tabs.Agent;
            }
            GUI.backgroundColor = bgColor;
        }

        EditorGUILayout.Space(2);

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);


        //////////////////////////////////////////////////////////////////////////////////////

        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("PathOS+ Manager", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        managerReference = EditorGUILayout.ObjectField("Manager Reference: ", managerReference, typeof(PathOSManager), true)
        as PathOSManager;


        if (EditorGUI.EndChangeCheck())
        {
            hasManager = managerReference != null;

            if (hasManager)
            {
                managerID = managerReference.GetInstanceID();
            }
        }

        if (managerReference == null)
        {
            GUI.backgroundColor = redColor;
            if (GUILayout.Button("Create Manager"))
            {
                GameObject proxyManager;
                proxyManager = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PathOS+/Prefabs/PathOS Manager.prefab") as GameObject;
                Instantiate(proxyManager);
                managerReference = proxyManager.GetComponent<PathOSManager>();
            }
            GUI.backgroundColor = bgColor;
        }
        else
        {
            GUI.backgroundColor = greenColor;
            if (GUILayout.Button("Edit Manager"))
            {
                tabSelection = (int)Tabs.Manager;
            }
            GUI.backgroundColor = bgColor;
        }

        EditorGUILayout.Space(2);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);

        //////////////////////////////////////////////////////////////////////////////////////

        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Screenshot Options", EditorStyles.boldLabel);
         GrabScreenshotReference();

        EditorGUI.BeginChangeCheck();

        screenshotReference = EditorGUILayout.ObjectField("Screenshot Reference: ", screenshotReference, typeof(ScreenshotManager), true)
             as ScreenshotManager;

        //Update agent ID if the user has selected a new object reference.
        if (EditorGUI.EndChangeCheck())
         {
             hasScreenshot = screenshotReference != null;

             if (hasScreenshot)
             {
                 screenshotID = screenshotReference.GetInstanceID();
             }
         }

         //or instantiate the screenshot camera if it does not exist
         if (screenshotReference == null)
         {
            GUI.backgroundColor = redColor;
            if (GUILayout.Button("Instantiate Screenshot Camera"))
             {
                 proxyScreenshot = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/PathOS+/Prefabs/ScreenshotCamera.prefab") as GameObject;
                 Instantiate(proxyScreenshot);
                 screenshotReference = proxyScreenshot.GetComponent<ScreenshotManager>();
             }
            GUI.backgroundColor = bgColor;
        }

        //only draws the screenshot elements if the variable is not null
        if (screenshotReference != null)
         {
             screenshotReference.SetFolderName(EditorGUILayout.TextField("Folder Name: ", screenshotReference.GetFolderName()));
             screenshotReference.SetFilename(EditorGUILayout.TextField("Filename: ", screenshotReference.GetFilename()));
             disableCamera = EditorGUILayout.Toggle("Disable During Runtime", disableCamera);

            GUI.backgroundColor = greenColor;
            if (GUILayout.Button("Take Screenshot"))
            {
                screenshotReference.TakeScreenshotGeneral();
            }
            GUI.backgroundColor = bgColor;

            if (EditorApplication.isPlaying && disableCamera)
            {
                screenshotReference.gameObject.SetActive(false);
            }
            else if (EditorApplication.isPlaying && !disableCamera)
            {
                screenshotReference.gameObject.SetActive(true);
            }
         }

        EditorGUILayout.Space(2);
        EditorGUILayout.EndVertical();

    }

    private void GrabScreenshotReference()
    {
        if (hasScreenshot && null == screenshotReference)
            screenshotReference = EditorUtility.InstanceIDToObject(screenshotID) as ScreenshotManager;
    }
    private void GrabManagerReference()
    {
        if (hasManager && null == managerReference)
            managerReference = EditorUtility.InstanceIDToObject(managerID) as PathOSManager;
    }
    private void GrabAgentReference()
    {
        if (hasAgent && null == agentReference)
            agentReference = EditorUtility.InstanceIDToObject(agentID) as PathOSAgent;
    }
}
