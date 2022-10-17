using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderObjectPivot : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Transform pivotCam;

    [SerializeField]
    private Transform root;

    public void SetPref(GameObject go)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            var g = root.GetChild(i);
            DestroyImmediate(g.gameObject);
        }

        Instantiate(go, root);
    }
}
