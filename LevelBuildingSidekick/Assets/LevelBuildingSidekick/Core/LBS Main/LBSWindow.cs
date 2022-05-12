using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LBSWindow : EditorWindow
{
    [MenuItem("Level Building Sidekick/Open New")]
    public static void ShowWindow()
    {
        var window = GetWindow<LBSWindow>("Level Building Sidekick");
    }


}
