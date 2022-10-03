using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LBS.Settings
{
    public static class LBSSettingProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateTileTagProvider()
        {
            var provider = new SettingsProvider("LBS/Tags/Tile", SettingsScope.Project)
            {
                label = "Tile tags",
                guiHandler = (searchContext) =>
                {
                    var tags = TagsIntance.GetInstance("Tile tags");
                    for (int i = 0; i < tags.Basics.Count; i++)
                    {
                        var tag = tags.Basics[i];
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.SelectableLabel(tag, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        EditorGUI.EndDisabledGroup();
                    }
                    for (int i = 0; i < tags.Others.Count; i++)
                    {
                        var tag = tags.Others[i];
                        var v = EditorGUILayout.TextField(tag);
                        tags.SetTag(i, v);
                    }
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    if (GUILayout.Button("+", GUILayout.MaxWidth(50)))
                    {
                        tags.AddTag("");
                    }
                    if (GUILayout.Button("-", GUILayout.MaxWidth(50)))
                    {
                        tags.RemoveLast();
                    }
                    EditorGUILayout.EndHorizontal();
                },
                keywords = new HashSet<string>(new[] { "Number", "Some String" })
            };
            return provider;
        }

        [SettingsProvider]
        public static SettingsProvider CreateBrushTagProvider()
        {
            var provider = new SettingsProvider("LBS/Tags/BrushTags", SettingsScope.Project)
            {
                label = "Brush tags",
                guiHandler = (searchContext) =>
                {
                    var tags = TagsIntance.GetInstance("Brush tags");
                    for (int i = 0; i < tags.Basics.Count; i++)
                    {
                        var tag = tags.Basics[i];
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.SelectableLabel(tag, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        EditorGUI.EndDisabledGroup();
                    }
                    for (int i = 0; i < tags.Others.Count; i++)
                    {
                        var tag = tags.Others[i];
                        var v = EditorGUILayout.TextField(tag);
                        tags.SetTag(i, v);
                    }
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    if (GUILayout.Button("+", GUILayout.MaxWidth(50)))
                    {
                        tags.AddTag("");
                    }
                    if (GUILayout.Button("-", GUILayout.MaxWidth(50)))
                    {
                        tags.RemoveLast();
                    }
                    EditorGUILayout.EndHorizontal();
                },
                keywords = new HashSet<string>(new[] { "Number", "Some String" })
            };
            return provider;
        }
    }
}

