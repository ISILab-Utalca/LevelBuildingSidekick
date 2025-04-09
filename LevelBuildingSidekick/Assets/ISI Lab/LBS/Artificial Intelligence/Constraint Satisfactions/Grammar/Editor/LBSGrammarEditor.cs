using ISILab.AI.Grammar;
using ISILab.LBS.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ISILab.LBS.VisualElements
{
    [CustomEditor(typeof(LBSGrammar))]
    public class LBSGrammarEditor : UnityEditor.Editor
    {
        string path = "";
        bool foldout;
        List<bool> actionFoldouts = new ();

        public override void OnInspectorGUI()
        {
            var grammar = (LBSGrammar)target;
            foldout = EditorGUILayout.Foldout(foldout, "Actions");

            if (foldout)
            {
                if (actionFoldouts.Count != grammar.ActionCount)
                {
                    for (int i = 0; i < grammar.ActionCount; i++)
                    {
                        actionFoldouts.Add(false);
                    }
                }

                for (int i = 0; i < grammar.ActionCount; i++)
                {
                    var action = grammar.GetAction(i);
                    GUILayout.Label(action.GrammarElement.ID);
                }
            }

            EditorGUILayout.Space();
            GUILayout.Label("Import Grammar", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
            path = EditorGUILayout.TextField("Grammar path", path);

            if (GUILayout.Button("Import"))
            {
                var _grammar = GrammarReader.ReadGrammar(path);
                if (_grammar == null)
                {
                    throw new Exception("Could not load Grammar File");
                }

                grammar.GrammarTree = _grammar;
                EditorUtility.SetDirty(target);

                // Reset foldouts after importing the grammar
                actionFoldouts = new List<bool>();
                if (grammar.GrammarTree != null)
                {
                    for (int i = 0; i < grammar.ActionCount; i++)
                    {
                        actionFoldouts.Add(false);
                    }
                }

                // Generate and print all possible permutations
                GenerateAndLogPermutations(grammar);
            }
        }

        /// <summary>
        /// Generate and log all quest permutations and create a text file with the permutations.
        /// </summary>
        private void GenerateAndLogPermutations(LBSGrammar grammar)
        {
            // Generate all permutations
            var questDefinitions = grammar.GenerateQuestDefinitions();
            string allPermutations = string.Join("\n", questDefinitions.ConvertAll(q => string.Join(" -> ", q)));

            // Write the permutations to a .txt file
            string filePath = Path.Combine(Application.dataPath, "QuestPermutations.txt");
            File.WriteAllText(filePath, allPermutations);
            Debug.Log("Quest permutations have been written to: " + filePath);
        }
    }
}
