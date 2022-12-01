using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class RenderObjectPivot : MonoBehaviour
{
    [SerializeField]
    public Camera cam;
    [SerializeField]
    private Transform pivotCam;

    [SerializeField]
    public Transform root;

    public Transform cpMain;
    public List<Transform> canvasPivots;

    public List<RectTransform> rects;

    public void Clear()
    {
        for (int i = 0; i < root.childCount; i++)
        {
            var g = root.GetChild(i);
            DestroyImmediate(g.gameObject);
        }
    }

    public void SetPref(GameObject go)
    {
        Clear();
        Instantiate(go, root);
    }

    public void Update()
    {
        for (int i = 0; i < canvasPivots.Count; i++)
        {
            var pivot = canvasPivots[i];
            var p = cam.WorldToScreenPoint(pivot.transform.position);
            rects[i].localPosition = p - new Vector3(cam.pixelWidth, cam.pixelHeight)/2;
        }
    }

    internal void SetRotateCam(float value)
    {
        pivotCam.rotation = Quaternion.Euler(new Vector3(0, value, 0));
    }

    internal void SetDistanceCam(float value)
    {
        cam.transform.position = pivotCam.transform.position -(cam.transform.forward * value);
    }

    internal void LabelDist(float value)
    {
        cpMain.transform.localScale = Vector3.one * value;
    }
}

