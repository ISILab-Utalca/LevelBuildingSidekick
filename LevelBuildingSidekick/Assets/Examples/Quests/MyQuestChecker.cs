using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ISILab.LBS
{
    public class MyQuestChecker : MonoBehaviour
    {
        QuestTrigger quest;

        public KeyCode key;

        // Start is called before the first frame update
        void Start()
        {
            quest = GetComponent<QuestTrigger>();
            quest.IsCompleted = QuestComplete;
        }

        public bool QuestComplete()
        {
            if (Input.GetKeyDown(key))
            {
                return true;
            }
            return false;
        }
    }
}