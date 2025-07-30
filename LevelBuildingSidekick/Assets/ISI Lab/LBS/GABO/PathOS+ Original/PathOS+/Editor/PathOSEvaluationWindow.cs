using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using PathOS;

/*
PathOSEvaluationWindow.cs 
(Atiya Nova) 2021
 */

public enum Severity
{
    NONE = 0,
    LOW = 1,
    MED = 2,
    HIGH = 3,
}

public enum Category
{
    NONE = 0,
    POS = 1,
    NEG = 2,
}

//When you finally get time, please clean this up
public class UserComment
{
    public string description;
    public string selectionName;
    public bool categoryFoldout;
    public Severity severity;
    public Category category;
    public GameObject selection;
    public EntityType entityType;

    [SerializeField]
    public int selectionID;

    public UserComment()
    {
        description = "";
        categoryFoldout = false;
        severity = Severity.NONE;
        category = Category.NONE;
        selection = null;
        selectionName = "";
        selectionID = 0;
        entityType = EntityType.ET_NONE;
    }

    public UserComment(string description, bool categoryFoldout, Severity severity, Category category, GameObject selection, string selectionName, EntityType entityType)
    {
        this.description = description;
        this.categoryFoldout = categoryFoldout;
        this.severity = severity;
        this.category = category;
        this.selection = selection;
        this.selectionName = selectionName;
        selectionID = selection.GetInstanceID();
        this.entityType = entityType;
    }
}


public class EvaluationHelperFunctions : MonoBehaviour
{
    public static EntityType IndexToEntity(int index)
    {

        switch (index)
        {
            case 0:
                return EntityType.ET_NONE;
            case 1:
                return EntityType.ET_GOAL_OPTIONAL;
            case 2:
                return EntityType.ET_GOAL_MANDATORY;
            case 3:
                return EntityType.ET_GOAL_COMPLETION;
            case 4:
                return EntityType.ET_RESOURCE_ACHIEVEMENT;
            case 5:
                return EntityType.ET_RESOURCE_PRESERVATION_LOW;
            case 6:
                return EntityType.ET_RESOURCE_PRESERVATION_MED;
            case 7:
                return EntityType.ET_RESOURCE_PRESERVATION_HIGH;
            case 8:
                return EntityType.ET_HAZARD_ENEMY_LOW;
            case 9:
                return EntityType.ET_HAZARD_ENEMY_MED;
            case 10:
                return EntityType.ET_HAZARD_ENEMY_HIGH;
            case 11:
                return EntityType.ET_HAZARD_ENEMY_BOSS;
            case 12:
                return EntityType.ET_HAZARD_ENVIRONMENT;
            case 13:
                return EntityType.ET_POI;
            case 14:
                return EntityType.ET_POI_NPC;
            default:
                return EntityType.ET_NONE;
        }
    }
    public static int EntityToIndex(EntityType type)
    {
        switch (type)
        {
            case EntityType.ET_NONE:
                return 0;
            case EntityType.ET_GOAL_OPTIONAL:
                return 1;
            case EntityType.ET_GOAL_MANDATORY:
                return 2;
            case EntityType.ET_GOAL_COMPLETION:
                return 3;
            case EntityType.ET_RESOURCE_ACHIEVEMENT:
                return 4;
            case EntityType.ET_RESOURCE_PRESERVATION_LOW:
                return 5;
            case EntityType.ET_RESOURCE_PRESERVATION_MED:
                return 6;
            case EntityType.ET_RESOURCE_PRESERVATION_HIGH:
                return 7;
            case EntityType.ET_HAZARD_ENEMY_LOW:
                return 8;
            case EntityType.ET_HAZARD_ENEMY_MED:
                return 9;
            case EntityType.ET_HAZARD_ENEMY_HIGH:
                return 10;
            case EntityType.ET_HAZARD_ENEMY_BOSS:
                return 11;
            case EntityType.ET_HAZARD_ENVIRONMENT:
                return 12;
            case EntityType.ET_POI:
                return 13;
            case EntityType.ET_POI_NPC:
                return 14;
            default:
                return 0;
        }
    }



    public static string SeverityToString(Severity name)
    {
        switch (name)
        {
            case Severity.NONE:
                return "NA";
            case Severity.LOW:
                return "LOW";
            case Severity.MED:
                return "MED";
            case Severity.HIGH:
                return "HIGH";
            default:
                return "NA";
        }
    }

    public static EntityType StringToEntityType(string name)
    {
        switch (name)
        {
            case "NONE":
                return EntityType.ET_NONE;
            case "OPTIONAL GOAL":
                return EntityType.ET_GOAL_OPTIONAL;
            case "MANDATORY GOAL":
                return EntityType.ET_GOAL_MANDATORY;
            case "COMPLETION GOAL":
                return EntityType.ET_GOAL_COMPLETION;
            case "ACHIEVEMENT":
                return EntityType.ET_RESOURCE_ACHIEVEMENT;
            case "PRESERVATION LOW":
                return EntityType.ET_RESOURCE_PRESERVATION_LOW;
            case "PRESERVATION MED":
                return EntityType.ET_RESOURCE_PRESERVATION_MED;
            case "PRESERVATION HIGH":
                return EntityType.ET_RESOURCE_PRESERVATION_HIGH;
            case "LOW ENEMY":
                return EntityType.ET_HAZARD_ENEMY_LOW;
            case "MED ENEMY":
                return EntityType.ET_HAZARD_ENEMY_MED;
            case "HIGH ENEMY":
                return EntityType.ET_HAZARD_ENEMY_HIGH;
            case "BOSS":
                return EntityType.ET_HAZARD_ENEMY_BOSS;
            case "ENVIRONMENT HAZARD":
                return EntityType.ET_HAZARD_ENVIRONMENT;
            case "POI":
                return EntityType.ET_POI;
            case "NPC POI":
                return EntityType.ET_POI_NPC;
            default:
                return EntityType.ET_NONE;
        }
    }


    public static String EntityTypeToString(EntityType name)
    {
        switch (name)
        {
            case EntityType.ET_NONE:
                return "NONE";
            case EntityType.ET_GOAL_OPTIONAL:
                return "OPTIONAL GOAL";
            case EntityType.ET_GOAL_MANDATORY:
                return "MANDATORY GOAL";
            case EntityType.ET_GOAL_COMPLETION:
                return "COMPLETION GOAL";
            case EntityType.ET_RESOURCE_ACHIEVEMENT:
                return "ACHIEVEMENT";
            case EntityType.ET_RESOURCE_PRESERVATION_LOW:
                return "PRESERVATION LOW";
            case EntityType.ET_RESOURCE_PRESERVATION_MED:
                return "PRESERVATION MED";
            case EntityType.ET_RESOURCE_PRESERVATION_HIGH:
                return "PRESERVATION HIGH";
            case EntityType.ET_HAZARD_ENEMY_LOW:
                return "LOW ENEMY";
            case EntityType.ET_HAZARD_ENEMY_MED:
                return "MED ENEMY";
            case EntityType.ET_HAZARD_ENEMY_HIGH:
                return "HIGH ENEMY";
            case EntityType.ET_HAZARD_ENEMY_BOSS:
                return "BOSS";
            case EntityType.ET_HAZARD_ENVIRONMENT:
                return "ENVIRONMENT HAZARD";
            case EntityType.ET_POI:
                return "POI";
            case EntityType.ET_POI_NPC:
                return "NPC POI";
            default:
                return "NONE";
        }
    }

    public static Severity StringToSeverity(string name)
    {
        switch (name)
        {
            case "NA":
                return Severity.NONE;
            case "LOW":
                return Severity.LOW;
            case "MED":
                return Severity.MED;
            case "HIGH":
                return Severity.HIGH;
            default:
                return Severity.NONE;
        }
    }
    public static string CategoryToString(Category name)
    {
        switch (name)
        {
            case Category.NONE:
                return "NA";
            case Category.POS:
                return "POS";
            case Category.NEG:
                return "NEG";
            default:
                return "NA";
        }
    }
    public static Category StringToCategory(string name)
    {
        switch (name)
        {
            case "NA":
                return Category.NONE;
            case "POS":
                return Category.POS;
            case "NEG":
                return Category.POS;
            default:
                return Category.NONE;
        }
    }
}

class ExpertEvaluation 
{ 
    //TODO: Spread things out in here to clean it up
    public List<UserComment> userComments = new List<UserComment>();
    private GUIStyle foldoutStyle = GUIStyle.none, buttonStyle = GUIStyle.none, labelStyle = GUIStyle.none;

    private readonly string[] severityNames = new string[] { "NA", "LOW", "MED", "HIGH" };
    private readonly string[] entityNames = new string[] { "NONE", "OPTIONAL GOAL", "MANDATORY GOAL", "COMPLETION GOAL", "ACHIEVEMENT", "PRESERVATION LOW",
    "PRESERVATION MED", "PRESERVATION HIGH", "LOW ENEMY", "MED ENEMY", "HIGH ENEMY", "BOSS", "ENVIRONMENT HAZARD", "POI", "NPC POI"};
    private readonly string[] categoryNames = new string[] { "NA", "POS", "NEG" };
    private Color[] severityColorsPos = new Color[] { Color.white, new Color32(175, 239, 169, 255), new Color32(86, 222, 74,255), new Color32(43, 172, 32,255) };
    private Color[] severityColorsNeg = new Color[] { Color.white, new Color32(232, 201, 100, 255), new Color32(232, 142, 100,255), new Color32(248, 114, 126, 255) };
    private Color[] categoryColors = new Color[] { Color.white, Color.green, new Color32(248, 114, 126, 255) };
    private Color entityColor = new Color32(60, 145, 255, 120);

    private Texture2D undefined, optional, mandatory, final, collectible, enemy_hazard, poi, npc, enemy_low, enemy_med, enemy_high, enemy_boss, health_low, health_med, health_high;

    public void LoadIcons()
    {
        undefined = Resources.Load<Texture2D>("entity_null");
        optional = Resources.Load<Texture2D>("goal_optional");
        mandatory = Resources.Load<Texture2D>("goal_mandatory");
        final = Resources.Load<Texture2D>("goal_completion");
        collectible = Resources.Load<Texture2D>("resource_achievement");
        enemy_low = Resources.Load<Texture2D>("hazard_enemy_low");
        enemy_med = Resources.Load<Texture2D>("hazard_enemy_medium");
        enemy_high = Resources.Load<Texture2D>("hazard_enemy_high");
        enemy_boss = Resources.Load<Texture2D>("hazard_enemy_boss");
        enemy_hazard = Resources.Load<Texture2D>("hazard_environment");
        health_low = Resources.Load<Texture2D>("resource_preservation_low");
        health_med = Resources.Load<Texture2D>("resource_preservation_med");
        health_high = Resources.Load<Texture2D>("resource_preservation_high");
        poi = Resources.Load<Texture2D>("poi_environment");
        npc = Resources.Load<Texture2D>("poi_npc");

    }
    public void SaveData()
    {
        string saveName;
        Scene scene = SceneManager.GetActiveScene();

        saveName = scene.name + " heuristicAmount";

        int counter = userComments.Count;
        PlayerPrefs.SetInt(saveName, counter);

        for (int i = 0; i < userComments.Count; i++)
        {
            saveName = scene.name + " heuristicsInputs " + i;

            PlayerPrefs.SetString(saveName, userComments[i].description);

            saveName = scene.name + " selectionName " + i;

            PlayerPrefs.SetString(saveName, userComments[i].selectionName);

            saveName = scene.name + " heuristicsSeverities " + i;

            PlayerPrefs.SetInt(saveName, (int)userComments[i].severity);

            saveName = scene.name + " heuristicsCategories " + i;

            PlayerPrefs.SetInt(saveName, (int)userComments[i].category);

            saveName = scene.name + " selectionID " + i;

            PlayerPrefs.SetInt(saveName, userComments[i].selectionID);

            saveName = scene.name + " entityType " + i;

            PlayerPrefs.SetInt(saveName, (int)userComments[i].entityType);

            saveName = scene.name + " categoryFoldout " + i;

            PlayerPrefs.SetInt(saveName, userComments[i].categoryFoldout ? 1 : 0);

        }
    }

    public void LoadData()
    {
        string saveName;
        Scene scene = SceneManager.GetActiveScene();
        int counter = 0;

        userComments.Clear();

        saveName = scene.name + " heuristicAmount";

        if (PlayerPrefs.HasKey(saveName))
            counter = PlayerPrefs.GetInt(saveName);

        for (int i = 0; i < counter; i++)
        {
            userComments.Add(new UserComment());

            saveName = scene.name + " heuristicsInputs " + i;
            if (PlayerPrefs.HasKey(saveName))
                userComments[i].description = PlayerPrefs.GetString(saveName);


            saveName = scene.name + " selectionName " + i;
            if (PlayerPrefs.HasKey(saveName))
                userComments[i].selectionName = PlayerPrefs.GetString(saveName);

            if (userComments[i].selectionName != "") userComments[i].selection = GameObject.Find(userComments[i].selectionName);

            saveName = scene.name + " heuristicsSeverities " + i;
            if (PlayerPrefs.HasKey(saveName))
                userComments[i].severity = (Severity)PlayerPrefs.GetInt(saveName);

            saveName = scene.name + " heuristicsCategories " + i;

            if (PlayerPrefs.HasKey(saveName))
                userComments[i].category = (Category)PlayerPrefs.GetInt(saveName);

            saveName = scene.name + " selectionID " + i;

            userComments[i].selectionID = PlayerPrefs.GetInt(saveName);

            //if (userComments[i].selectionID != 0)
              //  userComments[i].selection = //EditorUtility.InstanceIDToObject(userComments[i].selectionID) as GameObject;

            saveName = scene.name + " entityType " + i;

            userComments[i].entityType = (EntityType)PlayerPrefs.GetInt(saveName);

            saveName = scene.name + " categoryFoldout " + i;
            userComments[i].categoryFoldout = PlayerPrefs.GetInt(saveName) == 1 ? true : false;
        }

    }

    public void DrawComments(Rect layout)
    {
        EditorGUILayout.Space();

        foldoutStyle = EditorStyles.foldout;
        foldoutStyle.fontSize = 14;

        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 15;

        labelStyle.fontStyle = FontStyle.Italic;        
        labelStyle.fontSize = 15;

        EditorGUILayout.BeginVertical("Box");

        if (userComments.Count <= 0)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("   There are currently no comments.", labelStyle);
            EditorGUILayout.EndHorizontal();
        }

        for (int i = 0; i < userComments.Count; i++)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical("Button");
            foldoutStyle.fontStyle = FontStyle.Italic;

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(GetIcon(userComments[i].entityType), GUILayout.Width(17), GUILayout.Height(17));

            userComments[i].categoryFoldout = EditorGUILayout.Foldout(userComments[i].categoryFoldout, "Comment #" + (i+1), foldoutStyle);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("X", GUILayout.Width(17), GUILayout.Height(15)))
            {
                userComments.RemoveAt(i);
                i--;
                SaveData();
                continue;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            if (!userComments[i].categoryFoldout)
            {
                EditorGUILayout.EndVertical();
                continue;
            }

            EditorGUI.indentLevel++;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorStyles.label.wordWrap = true;
            userComments[i].description = EditorGUILayout.TextArea(userComments[i].description, GUILayout.Width(layout.width - 175));

            GUI.backgroundColor = categoryColors[((int)userComments[i].category)];
            userComments[i].category = (Category)EditorGUILayout.Popup((int)userComments[i].category, categoryNames);

            if (userComments[i].category != Category.POS) GUI.backgroundColor = severityColorsNeg[((int)userComments[i].severity)];
            else GUI.backgroundColor = severityColorsPos[((int)userComments[i].severity)];
            userComments[i].severity = (Severity)EditorGUILayout.Popup((int)userComments[i].severity, severityNames);
            GUI.backgroundColor = severityColorsPos[0];

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();

            userComments[i].selection = EditorGUILayout.ObjectField("", userComments[i].selection, typeof(GameObject), true)
                as GameObject;

            if (userComments[i].selection == null) userComments[i].selectionName = "";

            if (userComments[i].entityType != EntityType.ET_NONE) GUI.backgroundColor = entityColor;
            userComments[i].entityType = EvaluationHelperFunctions.IndexToEntity(EditorGUILayout.Popup(EvaluationHelperFunctions.EntityToIndex(userComments[i].entityType), entityNames));
            GUI.backgroundColor = severityColorsPos[0];

            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                if (userComments[i].selection != null) userComments[i].selectionID = userComments[i].selection.GetInstanceID(); 
                SaveData();
            }

            EditorGUILayout.Space(5);

            EditorGUILayout.EndVertical();

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("+", buttonStyle, GUILayout.Width(100)))
        {
            userComments.Add(new UserComment());
            SaveData();
        }
        if (GUILayout.Button("-", buttonStyle, GUILayout.Width(100)))
        {
            if (userComments.Count > 0) 
            {
                userComments.RemoveAt(userComments.Count - 1);
                SaveData();
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        EditorGUILayout.EndVertical();

        foldoutStyle.fontSize = 12;

    }

    public void DeleteAll()
    {
        userComments.Clear();

        SaveData();
    }

    public void ImportInputs(string filename)
    {
        StreamReader reader = new StreamReader(filename);

        string line = "";
        string[] lineContents;

        int inputCounter = 0;
        int lineNumber = 0;

        userComments.Clear();

        while ((line = reader.ReadLine()) != null)
        {
            lineContents = line.Split(',');

            if (lineContents.Length < 1)
            {
                Debug.Log("Error! Unable to read line");
                continue;
            }

            lineNumber++;

            if (lineNumber <= 7)
            {
                continue;
            }

            userComments.Add(new UserComment());

            string newDescription = lineContents[1].Replace("  ", "\n").Replace("/", ",");
            userComments[inputCounter].description = newDescription;

            userComments[inputCounter].severity = EvaluationHelperFunctions.StringToSeverity(lineContents[2]);

            userComments[inputCounter].category = EvaluationHelperFunctions.StringToCategory(lineContents[3]);

            if (lineContents[4] == "No GameObject")
            {
                userComments[inputCounter].selection = null;
                userComments[inputCounter].selectionName = "";
                userComments[inputCounter].selectionID = 0;
            }
            else
            {
                userComments[inputCounter].selectionID = int.Parse(lineContents[5]);
                userComments[inputCounter].selection = GameObject.Find(lineContents[4]); //EditorUtility.InstanceIDToObject(userComments[inputCounter].selectionID) as GameObject; 
                userComments[inputCounter].selectionName = userComments[inputCounter].selection.name;
            }

            userComments[inputCounter].entityType = EvaluationHelperFunctions.StringToEntityType(lineContents[6]);

            inputCounter++;
        }

        reader.Close();

        SaveData();
    }

    //Exports the heuristics
    public void ExportHeuristics(string filename)
    {
        StreamWriter writer = new StreamWriter(filename);

        List<string> headerComponents = new List<string>();
        List<string> noneComponents = new List<string>();
        List<string> lowComponents = new List<string>();
        List<string> medComponents = new List<string>();
        List<string> highComponents = new List<string>();

        headerComponents.Add("Severity,");
        noneComponents.Add("None,");
        lowComponents.Add("Low,");
        medComponents.Add("Med,");
        highComponents.Add("High,");

        for (int i = 0; i < userComments.Count; i++)
        {
            string entityString = EvaluationHelperFunctions.EntityTypeToString(userComments[i].entityType) + ",";

            if (headerComponents.Contains(entityString))
            {
                int index = headerComponents.IndexOf(EvaluationHelperFunctions.EntityTypeToString(userComments[i].entityType) + ",");
                
                if (userComments[i].severity == Severity.LOW)
                {
                    lowComponents[index-1] += " #" + (i + 1);
                }
                else if (userComments[i].severity == Severity.MED)
                {
                    medComponents[index-1] += " #" + (i + 1);
                }
                else if (userComments[i].severity == Severity.HIGH)
                {
                    highComponents[index-1] += " #" + (i + 1);
                }
                else
                {
                    noneComponents[index-1] += " #" + (i + 1);
                }
            }    
            else
            {
                headerComponents.Add(entityString);

                lowComponents.Add(",");
                medComponents.Add(",");
                highComponents.Add(",");
                noneComponents.Add(",");

                if (userComments[i].severity == Severity.LOW)
                {
                    lowComponents[lowComponents.Count-2] += "#" + (i + 1);
                }
                else if (userComments[i].severity == Severity.MED)
                {
                    medComponents[medComponents.Count - 2] += "#" + (i + 1);
                }
                else if (userComments[i].severity == Severity.HIGH)
                {
                    highComponents[highComponents.Count - 2] += "#" + (i + 1);
                }
                else
                {
                    noneComponents[noneComponents.Count - 2] += "#" + (i + 1);
                }
            }
        }

        string header = "";
        string none = "";
        string low = "";
        string med = "";
        string high = "";

        for (int i = 0; i < headerComponents.Count; i++)
        {
            header += headerComponents[i];
            none += noneComponents[i];
            low += lowComponents[i];
            med += medComponents[i];
            high += highComponents[i];
        }

        writer.WriteLine(header);
        writer.WriteLine(none);
        writer.WriteLine(low);
        writer.WriteLine(med);
        writer.WriteLine(high);
        writer.WriteLine("");

        writer.WriteLine("#, Description, Severity, Category, GameObject, Object ID, Entity Type");
        string description, severity, category, number, gameObjectName, ID, entity;

        for (int i = 0; i < userComments.Count; i++)
        {
            number = (i + 1).ToString();
            description = userComments[i].description.Replace("\r", "").Replace("\n", "  ").Replace(",", "/");

            severity = EvaluationHelperFunctions.SeverityToString(userComments[i].severity);

            category = EvaluationHelperFunctions.CategoryToString(userComments[i].category);

            if (userComments[i].selection != null)
            {
                gameObjectName = userComments[i].selection.name;
                ID = userComments[i].selectionID.ToString();
            }
            else
            {
                gameObjectName = "No GameObject";
                ID = "NA";
            }

            entity = entityNames[EvaluationHelperFunctions.EntityToIndex(userComments[i].entityType)];

            writer.WriteLine(number + ',' + description + ',' + severity + ',' + category + ',' + gameObjectName + ',' + ID + ',' + entity);
        }

        writer.Close();
        
        SaveData();
    }


    public void AddNewComment(UserComment comment)
    {
        userComments.Add(comment);
        SaveData();
    }

    private Texture2D GetIcon(EntityType type)
    {
        switch (type)
        {
            case EntityType.ET_GOAL_OPTIONAL:
                return optional;
            case EntityType.ET_GOAL_MANDATORY:
                return mandatory;
            case EntityType.ET_GOAL_COMPLETION:
                return final;
            case EntityType.ET_RESOURCE_ACHIEVEMENT:
                return collectible;
            case EntityType.ET_RESOURCE_PRESERVATION_LOW:
                return health_low;
            case EntityType.ET_RESOURCE_PRESERVATION_MED:
                return health_med;
            case EntityType.ET_RESOURCE_PRESERVATION_HIGH:
                return health_high;
            case EntityType.ET_HAZARD_ENEMY_LOW:
                return enemy_low;
            case EntityType.ET_HAZARD_ENEMY_MED:
                return enemy_med;
            case EntityType.ET_HAZARD_ENEMY_HIGH:
                return enemy_high;
            case EntityType.ET_HAZARD_ENEMY_BOSS:
                return enemy_boss;
            case EntityType.ET_HAZARD_ENVIRONMENT:
                return enemy_hazard;
            case EntityType.ET_POI:
                return poi;
            case EntityType.ET_POI_NPC:
                return npc;
            default:
                return undefined; 
        }
    }
}

[ExecuteInEditMode]
public class PathOSEvaluationWindow : EditorWindow
{
    private Color bgColor, btnColor;
    private PathOSManager managerReference;
    ExpertEvaluation comments = new ExpertEvaluation();
    private ScreenshotManager screenshot;
    private GUIStyle headerStyle = new GUIStyle();
    private GameObject selection = null;
    static bool popupAlreadyOpen = false;
    private string expertEvaluation = "Expert Evaluation", deleteAll = "DELETE ALL", import = "IMPORT", export = "EXPORT";
    private const string editorPrefsID = "PathOSEvaluationWindow";
    private static bool isCurrentlyOpen = false; //this is so jank, please fix this
    private static bool reloadData = false;
    private static string sceneName;
    private Color themeColor = Color.black;

    [SerializeField]
    private bool hasManager;

    [SerializeField]
    private int managerID;

    public static PathOSEvaluationWindow instance;

    private void OnEnable()
    {
        //Sets instance
        instance = this;

        //Set the scene name
        sceneName = SceneManager.GetActiveScene().name;

        //Background color
        comments.LoadIcons();
        comments.LoadData();
        bgColor = GUI.backgroundColor;
        btnColor = new Color32(200, 203, 224, 255);

        SceneView.duringSceneGui += this.OnSceneGUI;

        //Load saved settings.
        string prefsData = EditorPrefs.GetString(editorPrefsID, JsonUtility.ToJson(this, false));
        JsonUtility.FromJsonOverwrite(prefsData, this);

        //Saving manager reference
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

        if (PlayerPrefs.GetInt("IsThemeBlack") != 1)
        {
            themeColor = Color.white;
        }
        else
        {
            themeColor = Color.black;
        }
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;

        string prefsData = JsonUtility.ToJson(this, false);
        EditorPrefs.SetString(editorPrefsID, prefsData);

        PlayerPrefs.SetInt("IsThemeBlack", (themeColor == Color.black ? 1 : 0));

        comments.LoadData();
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;

        string prefsData = JsonUtility.ToJson(this, false);
        EditorPrefs.SetString(editorPrefsID, prefsData);

        PlayerPrefs.SetInt("IsThemeBlack", (themeColor == Color.black ? 1 : 0));

        comments.LoadData();
    }

    public void OnWindowOpen(Rect layout)
    {
        //Reloading data based on scene we're in
        if (sceneName != SceneManager.GetActiveScene().name)
        {
            sceneName = SceneManager.GetActiveScene().name;
            reloadData = true;
        }

        //In case the scene has changed, or a popup was added
        if (reloadData)
        {
            comments.LoadData();
            reloadData = false;
        }

        EditorGUILayout.BeginVertical("Box");

        GUILayout.BeginHorizontal();
        if (managerReference == null)
        {
            EditorGUILayout.HelpBox("MANAGER REFERENCE REQUIRED FOR ENTITY TAGGING", MessageType.Error);
        }
        else
        {
            EditorGUILayout.HelpBox("Right click objects in the scene to create a comment with its associated game object and entity type (if any)", MessageType.Info);
        }

        GUILayout.BeginVertical();
        GUI.backgroundColor = btnColor;
        if (GUILayout.Button("Light Mode"))
        {
            themeColor = Color.white;
        }
        if (GUILayout.Button("Dark Mode"))
        {
            themeColor = Color.black;
        }
        GUI.backgroundColor = bgColor;
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();


        EditorGUILayout.Space(15);

        GUI.backgroundColor = btnColor;
        headerStyle.fontSize = 20;
        headerStyle.normal.textColor = themeColor;
        
        EditorGUILayout.LabelField(expertEvaluation, headerStyle);

        EditorGUILayout.Space(10);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button(deleteAll))
        {
            comments.DeleteAll();
        }

        if (GUILayout.Button(import))
        {
            string importPath = EditorUtility.OpenFilePanel("Import Evaluation", "ASSETS\\EvaluationFiles", "csv");

            if (importPath.Length != 0)
            {
                comments.ImportInputs(importPath);
            }
        }

        if (GUILayout.Button(export))
        {
            string exportPath = EditorUtility.OpenFilePanel("Export Evaluation", "Assets\\EvaluationFiles", "csv");

            if (exportPath.Length != 0)
            {
                comments.ExportHeuristics(exportPath);
                if (screenshot != null) screenshot.TakeScreenshotEvaluation(Path.GetDirectoryName(exportPath) + Path.DirectorySeparatorChar, Path.GetFileName(exportPath));
            }
        }

        GUI.backgroundColor = bgColor;
        GUILayout.EndHorizontal();
        comments.DrawComments(layout);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Map View", headerStyle);
        GUILayout.FlexibleSpace();
        GUI.backgroundColor = btnColor;
        if (screenshot != null && GUILayout.Button("REFRESH"))
        {
            screenshot.GetNewScreenshot();
        }
        GUI.backgroundColor = bgColor;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(15);

        if (screenshot == null)
        {
            EditorGUILayout.HelpBox("SCREENSHOT REFERENCE REQUIRED FOR EXPORTING MAP SCREENSHOTS", MessageType.Error);
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        Texture2D currentScreenshot = screenshot.GetScreenshot();
        float targetWidth = layout.width,
            targetHeight = layout.width * currentScreenshot.height / currentScreenshot.width;
        GUILayout.Label(currentScreenshot, GUILayout.Width(targetWidth), GUILayout.Height(targetHeight));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    void OnSceneGUI(SceneView sceneView)
    {
        GrabManagerReference();

        if (popupAlreadyOpen || sceneView == null || !isCurrentlyOpen || EditorApplication.isPlaying) return;

        //Selection update.
        if (EditorWindow.mouseOverWindow != null && EditorWindow.mouseOverWindow.ToString() == " (UnityEditor.SceneView)")
        {
            Event e = Event.current;

            if (e != null && e.type == EventType.MouseUp && e.button == 1 && !popupAlreadyOpen)
            {
                selection = HandleUtility.PickGameObject(Event.current.mousePosition, true);

                if (selection != null)
                {
                    popupAlreadyOpen = true;
                    OpenPopup(selection, GetMarkup(selection));
                }
            }
        }
        else
        {
            selection = null;
        }
    }

    public void SetManagerReference(PathOSManager reference)
    {
        managerReference = reference;
    }

    public void SetScreenshotReference(ScreenshotManager reference)
    {
        screenshot = reference;
    }

    //Please clean this up
    public void AddComment(UserComment comment)
    {
        popupAlreadyOpen = false;
        reloadData = true;
        comments.AddNewComment(comment);
    }

    public void ClosePopup()
    {
        popupAlreadyOpen = false;
    }
    public void SetCurrentlyOpen(bool temp)
    {
        isCurrentlyOpen = temp;
    }

    private void OpenPopup(GameObject selection, EntityType entityType)
    {
        Popup window = new Popup();
        window.selection = selection;
        window.entityType = entityType;
        window.position = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 400, 150);
        window.ShowPopup();
    }

    private EntityType GetMarkup(GameObject selection)
    {
        if (managerReference == null)
        {
            return EntityType.ET_NONE;
        }

        for (int i = 0; i < managerReference.levelEntities.Count; i++)
        {
            //Looks into the pathos manager to figure out if that object has a tag
            if (managerReference.levelEntities[i].objectRef == selection)
            {
                return managerReference.levelEntities[i].entityType;
            }
        }

        return EntityType.ET_NONE;
    }

    private void GrabManagerReference()
    {
        if (hasManager && null == managerReference)
            managerReference = EditorUtility.InstanceIDToObject(managerID) as PathOSManager;
    }
}

//Really messy, rushed implementation. Please clean this up
public class Popup : EditorWindow
{
    private string description = "";
    Severity severity = Severity.NONE;
    Category category = Category.NONE;

    private readonly string[] severityNames = new string[] { "NA", "LOW", "MED", "HIGH" };
    private readonly string[] categoryNames = new string[] { "NA", "POS", "NEG" };
    private readonly string[] entityNames = new string[] { "NONE", "OPTIONAL GOAL", "MANDATORY GOAL", "COMPLETION GOAL", "ACHIEVEMENT", "PRESERVATION LOW",
    "PRESERVATION MED", "PRESERVATION HIGH", "LOW ENEMY", "MED ENEMY", "HIGH ENEMY", "BOSS", "ENVIRONMENT HAZARD", "POI", "NPC POI"};

    private Color[] severityColorsPos = new Color[] { Color.white, new Color32(175, 239, 169, 255), new Color32(86, 222, 74, 255), new Color32(43, 172, 32, 255) };
    private Color[] severityColorsNeg = new Color[] { Color.white, new Color32(232, 201, 100, 255), new Color32(232, 142, 100, 255), new Color32(248, 114, 126, 255) };
    private Color[] categoryColors = new Color[] { Color.white, Color.green, new Color32(248, 114, 126, 255) };
    private Color entityColor = new Color32(60, 145, 255, 120);

    private GUIStyle labelStyle = GUIStyle.none;
    public GameObject selection;
    public EntityType entityType; 

    private void OnDestroy()
    {
        PathOSEvaluationWindow.instance.ClosePopup();
    }

    private void OnDisable()
    {
        PathOSEvaluationWindow.instance.ClosePopup();
    }

    void OnGUI()
    {
        labelStyle.fontSize = 15;
        labelStyle.fontStyle = FontStyle.Italic;

        EditorGUI.indentLevel++;

        EditorGUILayout.BeginVertical("Box");

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("New Comment", labelStyle);

        if (GUILayout.Button("X", GUILayout.Width(17), GUILayout.Height(15)))
        {
            PathOSEvaluationWindow.instance.ClosePopup();
            this.Close();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        EditorStyles.label.wordWrap = true;
        description = EditorGUILayout.TextArea(description, GUILayout.Width(Screen.width * 0.6f));

        GUI.backgroundColor = categoryColors[((int)category)];
        category = (Category)EditorGUILayout.Popup((int)category, categoryNames);

        if (category != Category.POS) GUI.backgroundColor = severityColorsNeg[((int)severity)];
        else GUI.backgroundColor = severityColorsPos[((int)severity)];
        
        severity = (Severity)EditorGUILayout.Popup((int)severity, severityNames);
        GUI.backgroundColor = severityColorsPos[0];
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.Space(2);
        EditorGUILayout.BeginHorizontal();

        selection = EditorGUILayout.ObjectField("", selection, typeof(GameObject), true, GUILayout.Width(Screen.width * 0.6f))
            as GameObject;

        //entityType = (EntityType)EditorGUILayout.Popup(EntityToIndex(entityType), entityNames);
        if (entityType != EntityType.ET_NONE) GUI.backgroundColor = entityColor;
        entityType = EvaluationHelperFunctions.IndexToEntity(EditorGUILayout.Popup(EvaluationHelperFunctions.EntityToIndex(entityType), entityNames));
        GUI.backgroundColor = severityColorsPos[0];

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        EditorGUILayout.EndVertical();

       if (GUILayout.Button("Add Comment"))
        {
            PathOSEvaluationWindow.instance.AddComment(new UserComment(description, false, severity, category, selection, selection.name, entityType));
            this.Close();
        }
    }
}