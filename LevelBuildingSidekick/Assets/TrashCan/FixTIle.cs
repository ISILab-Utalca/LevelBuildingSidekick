using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FixTIle : MonoBehaviour
{
    private float trh = 1000f;
    private List<Transform> tR = new List<Transform>();

    [ContextMenu("Fix tile")]
    void DoSomething()
    {
        tR.Clear();
        CollectRemove(this.transform);
        Debug.Log(tR.Count);
        for (int i = tR.Count -1; i >= 0; i--)
        {
            DestroyImmediate(tR[i].gameObject);
        }
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    private void CollectRemove(Transform t)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            var ch =  t.GetChild(i);

            if (Mathf.Abs(ch.position.x) > trh || Mathf.Abs(ch.position.y) > trh || Mathf.Abs(ch.position.z) > trh)
            {
                tR.Add(ch);
            }
            else
            {
                CollectRemove(ch);
            }
        }
    }
}
