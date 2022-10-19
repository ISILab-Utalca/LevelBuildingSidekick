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

    public void SetPref(GameObject go)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            var g = root.GetChild(i);
            DestroyImmediate(g.gameObject);
        }

        Instantiate(go, root);
    }

    public void Update()
    {
        for (int i = 0; i < canvasPivots.Count; i++)
        {
            var pivot = canvasPivots[i];
            var p = cam.WorldToScreenPoint(pivot.transform.position);
            //var p = cam.ViewportToScreenPoint(pivot.transform.position);
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

[CustomEditor(typeof(RenderObjectPivot))]
public class ExampleEditor : Editor
{

    public void OnSceneGUI()
    {
        var t = target as RenderObjectPivot;
        var tr = t.transform;
        var pos = tr.position;
        // display an orange disc where the object is
        var color = new Color(1, 0.8f, 0.4f, 1);
        Handles.color = color;
        Handles.DrawWireDisc(pos, tr.up, 1.0f);
        // display object "value" in scene
        GUI.color = color;
        Handles.Label(pos, 1.ToString("F1"));
        //t.root.position = Handles.PositionHandle(t.root.position,Quaternion.identity);
        //Handles.DrawCamera(new Rect(0, 0, 100, 100), t.cam);

        foreach (var pivot in t.canvasPivots)
        {
            var p = t.cam.ViewportToScreenPoint(pivot.transform.position);
            Handles.Label(p,pivot.name);
        }

    }
}