using ISILab.AI.Grammar;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ISILab.LBS.VisualElements
{
    [CustomEditor(typeof(LBSGrammar))]
    public class LBSGrammarEditor : UnityEditor.Editor
    {
        private string _path = "";
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
                            EditorGUILayout.LabelField("• " + expansion);
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
                        EditorGUILayout.LabelField("• " + action);
                    }
                }
            }

            // Import section
            EditorGUILayout.Space(10);
            GUILayout.Label("Import SRGS Grammar", EditorStyles.boldLabel);
            _path = EditorGUILayout.TextField("Grammar Path", _path);

            if (GUILayout.Button("Import"))
            {
                try
                {
                    var structure = LBSGrammarReader.ReadGrammar(_path);
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
