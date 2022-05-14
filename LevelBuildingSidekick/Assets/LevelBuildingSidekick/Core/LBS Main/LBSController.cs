using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LBSController
{
    static EditorWindow view;
    LBSData data;

    [MenuItem("Level Building Sidekick/Open New")]
    public static void ShowWindow()
    {
        view = EditorWindow.GetWindow<EditorWindow>("Level Building Sidekick");
        view.Show();
    }
}
