using UnityEditor;
using UnityEngine;
using ISILab.AI.Grammar;
using System;
using System.Collections.Generic;

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
                            EditorGUILayout.LabelField("▪ " + expansion);
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
                        EditorGUILayout.LabelField("▪ " + action);
                    }
                }
            }

            // Import Section
            EditorGUILayout.Space(10);
            GUILayout.Label("Import SRGS Grammar", EditorStyles.boldLabel);
            _grammarAsset = (TextAsset)EditorGUILayout.ObjectField("Grammar File", _grammarAsset, typeof(TextAsset), false);

            if (_grammarAsset != null && GUILayout.Button("Read Grammar File"))
            {
                try
                {
                    string assetPath = AssetDatabase.GetAssetPath(_grammarAsset);
                    var structure = LBSGrammarReader.ReadGrammar(assetPath);

                    grammar.SetGrammarStructure(structure);
                    EditorUtility.SetDirty(grammar);
                    _ruleFoldouts.Clear();
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
