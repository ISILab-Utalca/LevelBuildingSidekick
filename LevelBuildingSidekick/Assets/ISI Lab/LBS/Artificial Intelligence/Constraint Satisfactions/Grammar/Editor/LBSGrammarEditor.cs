using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LBSGrammar))]
public class LBSGrammarEditor : Editor
{
    string path = "";
    bool foldout;
    List<bool> actionFoldouts = new List<bool>();

    public override void OnInspectorGUI()
    {
        var grammar = (LBSGrammar)target;
        //GUILayout.Label("<b>Import Grammar</b>");
        foldout = EditorGUILayout.Foldout(foldout, "Actions");
        if (foldout)
        {
            if(actionFoldouts.Count != grammar.ActionCount)
            {
                for (int i = 0; i < grammar.ActionCount; i++)
                {
                    actionFoldouts.Add(false);
                }
            }
            for (int i = 0; i < grammar.ActionCount; i++)
            {
                var action = grammar.GetAction(i);
                actionFoldouts[i] = EditorGUILayout.Foldout(actionFoldouts[i], action.GrammarElement.ID);
                if(actionFoldouts[i])
                {
                    for(int j = 0; j < action.TargetCount; j++)
                    {
                        action.SetTarget(j, EditorGUILayout.TextField("Target " + j + ": ", action.GetTarget(j)));
                    }
                    var s = EditorGUILayout.TextField("New Target: ", "");
                    if(s != "")
                    {
                        action.AddTarget(s);
                    }
                }
            }
        }

        EditorGUILayout.Space();
        GUILayout.Label("Import Grammar", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold});
        path = EditorGUILayout.TextField("Grammar path", path);

        if(GUILayout.Button("Import"))
        {
            var _grammar = GrammarReader.ReadGrammar(path);
            if(_grammar == null)
            {
                throw new Exception("Could not load Grammar File");
            }
            grammar.GrammarTree = _grammar;
            EditorUtility.SetDirty(target);

            actionFoldouts = new List<bool>();
            if (grammar.GrammarTree != null)
            {
                for(int i = 0; i < grammar.ActionCount; i++)
                {
                    actionFoldouts.Add(false);
                }
            }
        }
    }
}

