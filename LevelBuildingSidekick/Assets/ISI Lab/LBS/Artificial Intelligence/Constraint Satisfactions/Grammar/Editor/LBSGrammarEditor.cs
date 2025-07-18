using ISILab.AI.Grammar;
using System;
using System.Collections.Generic;
using ISILab.Macros;
using UnityEditor;
using UnityEngine;

namespace ISILab.LBS.VisualElements
{
    [CustomEditor(typeof(LBSGrammar))]
    public class LBSGrammarEditor : UnityEditor.Editor
    {
        private TextAsset _grammarAsset;
        private bool _foldout;
        private bool _terminalFoldout;
        private readonly Dictionary<string, bool> _ruleFoldouts = new();

        public override void OnInspectorGUI()
        {
            var grammar = (LBSGrammar)target;

            // Show grammar rules
            _foldout = EditorGUILayout.Foldout(_foldout, "Grammar Rules", true);
            if (_foldout)
            {
                var rules = grammar.GetRules();
                foreach (var rule in rules)
                {
                    _ruleFoldouts.TryAdd(rule.Key, false);

                    _ruleFoldouts[rule.Key] = EditorGUILayout.Foldout(_ruleFoldouts[rule.Key], rule.Key);
                    if (_ruleFoldouts[rule.Key])
                    {
                        foreach (var expansion in rule.Value)
                        {
                            EditorGUILayout.LabelField("-" + expansion + "-");
                        }
                    }
                }

                if (rules.Count == 0)
                {
                    EditorGUILayout.LabelField("No rules loaded.");
                }
            }

            // Show terminal actions
            EditorGUILayout.Space(10);
            _terminalFoldout = EditorGUILayout.Foldout(_terminalFoldout, "Terminal Actions", true);
            if (_terminalFoldout)
            {
                var terminals = grammar.TerminalActions;
                if (terminals.Count == 0)
                {
                    EditorGUILayout.LabelField("No terminal actions found.");
                }
                else
                {
                    foreach (var action in terminals)
                    {
                        EditorGUILayout.LabelField("-" + action + "-");
                    }
                }
            }

            // In OnInspectorGUI():
            EditorGUILayout.Space(10);
            GUILayout.Label("Import SRGS Grammar", EditorStyles.boldLabel);

// Drag-and-drop TextAsset field
            _grammarAsset = (TextAsset)EditorGUILayout.ObjectField("Grammar File", _grammarAsset, typeof(TextAsset), false);

            if (_grammarAsset is not null && GUILayout.Button("Read Grammar File"))
            {
                try
                {
                    // Get the asset path in the project (e.g., "Assets/Grammars/example.txt")
                    string assetPath = AssetDatabase.GetAssetPath(_grammarAsset);

                    // Convert it to an absolute path (needed for SrgsDocument)
                   // string fullPath = System.IO.Path.Combine(Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length), assetPath);

                    var structure = LBSGrammarReader.ReadGrammar(assetPath);
                    grammar.SetGrammarStructure(structure);
                    EditorUtility.SetDirty(grammar);
                    _ruleFoldouts.Clear(); // reset foldouts for new rules
                    Debug.Log($"Grammar imported with {structure.Rules.Count} rules and {structure.Terminals.Count} terminal actions.");
                }
                catch (Exception ex)
                {
                    Debug.LogError("Import failed: " + ex.Message);
                }
            }
        }
    }
}
