using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utility;

namespace LBS
{
    public static class TagsIntance
    {
        public static Dictionary<string, LBSTagLists> tagsLists = new Dictionary<string, LBSTagLists>();

        public static LBSTagLists GetInstance(string name)
        {
            var ins = DirectoryTools.GetScriptable<LBSTagLists>(name);
            if (ins == null)
            {
                ins = ScriptableObject.CreateInstance<LBSTagLists>();
                var path = EditorUtility.SaveFilePanel("Create 'Tags' file", "", "name", "asset");
                AssetDatabase.CreateAsset(ins, path);
                AssetDatabase.SaveAssets();
            }
            return ins;
        }

        public static SerializedObject GetSerialized(string name)
        {
            return new SerializedObject(GetInstance(name));
        }

        public static void c() // importante para las tags de tile
        {
            new List<string>() { "Floor", "Ceiling", "Wall", "Door", "Prop" };
        }
    }

    [System.Serializable]
    [CreateAssetMenu(fileName = "New tags list", menuName = "ISILab/LBS plugin/Tags List")]
    public class LBSTagLists : ScriptableObject // (!) esta clase va a terminar cambiando tarde o temprano
    {
        public static LBSTagLists GetInstance(string name)
        {
            var ins = DirectoryTools.GetScriptable<LBSTagLists>(name);
            if (ins == null)
            {
                ins = ScriptableObject.CreateInstance<LBSTagLists>();
                var path = EditorUtility.SaveFilePanel("Create 'Tags' file", "", "name", "asset");
                AssetDatabase.CreateAsset(ins, path);
                AssetDatabase.SaveAssets();
            }
            return ins;
        }

        public static SerializedObject GetSerialized(string name)
        {
            return new SerializedObject(GetInstance(name));
        }

        [SerializeField]
        private List<string> basics = new List<string>();

        [SerializeField]
        private List<string> others = new List<string>();

        public List<string> Basics => new List<string>(basics);
        public List<string> Others => new List<string>(others);
        public List<string> Alls => new List<string>(basics).Concat(others).ToList();

        public void SetTag(int n, string value)
        {
            others[n] = value;
            EditorUtility.SetDirty(this);
        }

        public void AddTag(string tag = "")
        {
            others.Add(tag);
            EditorUtility.SetDirty(this);
        }

        public void RemoveLast()
        {
            if (others.Count > 0)
            {
                others.Remove(others.Last());
            }
            EditorUtility.SetDirty(this);
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }


    [CustomEditor(typeof(LBSTagLists))]
    public class LBSTags_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            var t = target as LBSTagLists;

            if (t == null)
                return;

            serializedObject.Update();
            EditorGUILayout.LabelField("Tags", EditorStyles.boldLabel);
            for (int i = 0; i < t.Basics.Count; i++)
            {
                var tag = t.Basics[i];
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.SelectableLabel(tag, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                EditorGUI.EndDisabledGroup();
            }
            for (int i = 0; i < t.Others.Count; i++)
            {
                var tag = t.Others[i];
                var v = EditorGUILayout.TextField(tag);
                t.SetTag(i, v);
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("+", GUILayout.MaxWidth(50)))
            {
                t.AddTag("");
            }

            if (GUILayout.Button("-", GUILayout.MaxWidth(50)))
            {
                t.RemoveLast();
            }
            EditorGUILayout.EndHorizontal();

            if(GUILayout.Button("save"))
            {
                t.Save();
            }

            EditorGUILayout.HelpBox("These objects contain sensitive information for the operation of the LBS package," +
                " please do not move, delete or change their names.", MessageType.Warning);
            serializedObject.ApplyModifiedProperties();
        }
    }

}