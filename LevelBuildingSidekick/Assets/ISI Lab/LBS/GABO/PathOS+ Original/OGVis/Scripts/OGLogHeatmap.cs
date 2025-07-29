﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OGVis;

/*
OGLogHeatmap.cs
OGLogHeatmap (c) Ominous Games 2019
*/

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class OGLogHeatmap : MonoBehaviour
{
    /* Display Settings */
    private Gradient gradient;
    private float alpha = 0.5f;

    private float displayHeight;

    /* Heatmap Mesh Data */
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;

    public Mesh mesh { get; private set; }
    private MeshFilter filter;
    private MeshRenderer rend;

    public Material mat { get; private set; }
    public Texture2D tex { get; private set; }

    /* Heatmap Generation */
    private Vector3 origin;
    private Vector3 gridSize;
    private float tileWidth;

    private int[,] tileCounts;

    public void Initialize(Extents extents, Gradient gradient, float alpha, float displayHeight, float tileSize)
    {
        filter = GetComponent<MeshFilter>();
        rend = GetComponent<MeshRenderer>();

        this.displayHeight = displayHeight;
        this.gradient = gradient;
        this.alpha = alpha;
        
        //Asset generation/allocation.
        if(null == mesh)
            mesh = new Mesh();

        filter.mesh = mesh;
        
        if(null == mat)
            mat = new Material(Shader.Find("Unlit/Transparent"));

        if (null == tex)
            tex = new Texture2D(0, 0);

        tex.filterMode = FilterMode.Point;
        tex.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;

        mat.mainTexture = tex;
        rend.material = mat;

        //Vertex order: Bottom left, top left, top right, bottom right.
        uv = new Vector2[4];
        uv[0] = new Vector2(0.0f, 0.0f);
        uv[1] = new Vector2(0.0f, 1.0f);
        uv[2] = new Vector2(1.0f, 1.0f);
        uv[3] = new Vector2(1.0f, 0.0f);

        triangles = new[] { 0, 1, 2, 0, 2, 3 };
        vertices = new Vector3[4];

        UpdateExtents(extents, tileSize);
    }

    public void Clear()
    {
        ClearTex();
        SetVisible(false);
    }

    //Resets heatmap texture to white.
    private void ClearTex()
    {
        if (null == tex || tex.width == 0 || tex.height == 0)
            return;

        Color32 white = new Color32(255, 255, 255, (byte)(alpha * 255));
        Color32[] resetArray = tex.GetPixels32();

        for (int i = 0; i < resetArray.Length; ++i)
        {
            resetArray[i] = white;
        }

        tex.SetPixels32(resetArray);
        tex.Apply();
    }

    private void OnApplicationQuit()
    {
        //Free resources.
        DestroyImmediate(tex);
        DestroyImmediate(mat);
        DestroyImmediate(mesh);
    }

    public void UpdateExtents(Extents extents, float tileSize)
    {
        tileWidth = tileSize;

        if (tileWidth <= 0)
            return;

        //What is the position of our heatmap in the scene?
        //Centre of all extents.
        origin.x = 0.5f * (extents.min.x + extents.max.x);
        origin.y = displayHeight;
        origin.z = 0.5f * (extents.min.z + extents.max.z);

        transform.position = origin;

        //Grid size should be symmetrical about the origin of the heatmap.
        gridSize.x = 2 * Mathf.Ceil((0.5f * (extents.max.x - extents.min.x)) / tileWidth);
        gridSize.y = 0;
        gridSize.z = 2 * Mathf.Ceil((0.5f * (extents.max.z - extents.min.z)) / tileWidth);

        tileCounts = new int[(int)gridSize.x, (int)gridSize.z];

        //Set up mesh data.
        vertices[0] = new Vector3(-0.5f * gridSize.x * tileWidth, 0.0f, -0.5f * gridSize.z * tileWidth);
        vertices[1] = new Vector3(-0.5f * gridSize.x * tileWidth, 0.0f,  0.5f * gridSize.z * tileWidth);
        vertices[2] = new Vector3( 0.5f * gridSize.x * tileWidth, 0.0f,  0.5f * gridSize.z * tileWidth);
        vertices[3] = new Vector3( 0.5f * gridSize.x * tileWidth, 0.0f, -0.5f * gridSize.z * tileWidth);

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        tex.Reinitialize((int)gridSize.x, (int)gridSize.z);
        ClearTex();
        tex.Apply();
    }

    public void UpdateData(List<PlayerLog> logs, bool enabledOnly, bool timeRangeOnly)
    {
        //Minimum coordinates derived from heatmap origin and size. 
        float minX = origin.x - 0.5f * gridSize.x * tileWidth;
        float minZ = origin.z - 0.5f * gridSize.z * tileWidth;

        float fac = 1.0f / tileWidth;

        int maxGridX = tileCounts.GetLength(0);
        int maxGridZ = tileCounts.GetLength(1);

        System.Array.Clear(tileCounts, 0, tileCounts.Length);

        int xGrid, zGrid = 0;

        //Update counts per-tile for agent position samples.
        for (int i = 0; i < logs.Count; ++i)
        {
            PlayerLog log = logs[i];

            if (enabledOnly && !log.visInclude)
                continue;

            int minIndex = (timeRangeOnly) ? log.displayStartIndex : 0;
            int maxIndex = (timeRangeOnly) ? log.displayEndIndex : log.pathPoints.Count - 1;

            for(int j = minIndex; j <= maxIndex; ++j)
            {
                xGrid = (int)((log.pathPoints[j].x - minX) * fac);
                zGrid = (int)((log.pathPoints[j].z - minZ) * fac);

                if (xGrid > 0 && xGrid < maxGridX
                    && zGrid > 0 && zGrid < maxGridZ)
                    ++tileCounts[xGrid, zGrid];
            }
        }

        int maxCount = 0;

        for(int x = 0; x < maxGridX; ++x)
        {
            for(int z = 0; z < maxGridZ; ++z)
            {
                if (tileCounts[x, z] > maxCount)
                    maxCount = tileCounts[x, z];
            }
        }

        //We want a minimum of 2 and a maximum of 10 bins.
        int levels = Mathf.Min(10, Mathf.Max(maxCount, 2));
        float colorStep = 1.0f / (levels - 1);

        List<Color32> colors = new List<Color32>();

        for(int i = 0; i < levels; ++i)
        {
            Color current = gradient.Evaluate(i * colorStep);
            current.a = alpha;
            colors.Add(current);
        }

        //Extra bin added as an overflow buffer.
        Color final = gradient.Evaluate(1.0f);
        final.a = alpha;
        colors.Add(final);

        float binSize = (float)Mathf.Max(1, maxCount) / levels;
        float binSizeFac = 1.0f / binSize;

        Color32[] heatmapArray = tex.GetPixels32();

        int rowSize = (int)gridSize.x;

        //Determine grid colours based on sample counts.
        for (int x = 0; x < maxGridX; ++x)
        {
            for(int z = 0; z < maxGridZ; ++z)
            {
                heatmapArray[(int)(z * rowSize + x)] =
                    colors[Mathf.RoundToInt(tileCounts[x, z] * binSizeFac)];
            }
        }

        tex.SetPixels32(heatmapArray);
        tex.Apply();
    }

    public void SetAlpha(float alpha)
    {
        this.alpha = alpha;
    }

    public void SetGradient(Gradient gradient)
    {
        this.gradient = gradient;
    }

    public void SetVisible(bool visible)
    {
        if (null == rend)
            rend = GetComponent<MeshRenderer>();

        rend.enabled = visible;
    }

    public void SetDisplayHeight(float displayHeight)
    {
        this.displayHeight = displayHeight;
        origin.y = displayHeight;

        transform.position = origin;
    }
}
