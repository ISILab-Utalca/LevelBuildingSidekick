using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ISILab.LBS
{       
        [ExecuteAlways]
        public class QuestChecker : MonoBehaviour
        {
                public bool completeQuest;
                private QuestTrigger quest;

                private void Awake()
                {
                        quest = GetComponent<QuestTrigger>();
                }


                private void ApplyCompletionState()
                {
                        if (!quest) return;

                        var field = typeof(QuestTrigger).GetField("isCompleted",
                                BindingFlags.NonPublic | BindingFlags.Instance);
                        if (field == null) return;
                        field.SetValue(quest, completeQuest);
                        Debug.Log($"Quest completion manually set to {completeQuest}");
                }

                [CustomEditor(typeof(QuestChecker))]
                public class QuestCheckerEditor : UnityEditor.Editor
                {
                        public override void OnInspectorGUI()
                        {
                                DrawDefaultInspector();

                                var checker = (QuestChecker)target;

                                if (GUILayout.Button("Apply Quest Completion")) checker.ApplyCompletionState();
                        }
                }
        }
}