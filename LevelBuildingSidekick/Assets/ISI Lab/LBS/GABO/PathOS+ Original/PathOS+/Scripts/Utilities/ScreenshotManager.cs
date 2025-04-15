using System;
using System.IO;
using UnityEngine;


/*
ScreenshotManager.cs 
based off of: https://kpprt.de/code-snippet/in-editor-screenshot-script-for-unity/
(Atiya Nova) 2021
 */

[RequireComponent(typeof(Camera))]
public class ScreenshotManager : MonoBehaviour
{
    //Screenshot capture variables
    private int width = 1024, height = 512;
    private string folder = "ScreenCapture", prefix = "capture",
        directory = "", filename = "", path = "",
        dash = "_", date = "yyMMdd_HHmmss", pngStr = ".png", 
        slash = "/";
    private new Camera camera;
    private RenderTexture rendTex, currRendTex;
    private Texture2D screenshot;

    public Texture2D GetNewScreenshot()
    {
        if (camera == null)
        {
            camera = GetComponent<Camera>();
        }

        //Creates and renders the texture
        rendTex = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        camera.targetTexture = rendTex;
        camera.Render();

        //Saves current render texture to override
        currRendTex = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        //Creates new texture to add render texture to 
        screenshot = new Texture2D(width, height, TextureFormat.ARGB32, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);

        //Convert pngs to srgb 
        if (QualitySettings.activeColorSpace == ColorSpace.Linear)
        {
            Color[] pixels = screenshot.GetPixels();

            for (int p = 0; p < pixels.Length; p++)
            {
                pixels[p] = pixels[p].gamma;
            }

            screenshot.SetPixels(pixels);
        }

        //Apply screenshot texture changes and save it
        screenshot.Apply(false);

        //Ref removal for garbage collection
        camera.targetTexture = null;

        //Replace original texture
        RenderTexture.active = currRendTex;

        return screenshot;
    }

    public Texture2D GetScreenshot()
    {
        return screenshot;
    }
    public void TakeScreenshotEvaluation(string evalFolder, string screenshotName)
    {
        if (camera == null)
        {
            camera = GetComponent<Camera>();
        }

        //Sets the file name and path
        directory = evalFolder + slash;
        filename = screenshotName.Replace(".csv", "") + pngStr;
        path = directory + filename;

        //Creates and renders the texture
        rendTex = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        camera.targetTexture = rendTex;
        camera.Render();

        //Saves current render texture to override
        currRendTex = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        //Creates new texture to add render texture to 
        screenshot = new Texture2D(width, height, TextureFormat.ARGB32, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);

        //Convert pngs to srgb 
        if (QualitySettings.activeColorSpace == ColorSpace.Linear)
        {
            Color[] pixels = screenshot.GetPixels();

            for (int p = 0; p < pixels.Length; p++)
            {
                pixels[p] = pixels[p].gamma;
            }

            screenshot.SetPixels(pixels);
        }

        //Apply screenshot texture changes and save it
        screenshot.Apply(false);
        Directory.CreateDirectory(directory);
        byte[] png = screenshot.EncodeToPNG();
        File.WriteAllBytes(path, png);

        //Ref removal for garbage collection
        camera.targetTexture = null;

        //Replace original texture
        RenderTexture.active = currRendTex;
    }

    public void TakeScreenshotGeneral()
    {
        if (camera == null)
        {
            camera = GetComponent<Camera>();
        }

        //Sets the file name and path
        folder = GetSafePath(folder.Trim('/'));
        prefix = GetSafeFilename(prefix);
        directory = Application.dataPath + slash + folder + slash;
        filename = prefix + dash + DateTime.Now.ToString(date) + pngStr;
        path = directory + filename;

        //Creates and renders the texture
        rendTex = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        camera.targetTexture = rendTex;
        camera.Render();

        //Saves current render texture to override
        currRendTex = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        //Creates new texture to add render texture to 
        screenshot = new Texture2D(width, height, TextureFormat.ARGB32, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);

        //Convert pngs to srgb 
        if (QualitySettings.activeColorSpace == ColorSpace.Linear)
        {
            Color[] pixels = screenshot.GetPixels();

            for (int p = 0; p < pixels.Length; p++)
            {
                pixels[p] = pixels[p].gamma;
            }

            screenshot.SetPixels(pixels);
        }

        //Apply screenshot texture changes and save it
        screenshot.Apply(false);
        Directory.CreateDirectory(directory);
        byte[] png = screenshot.EncodeToPNG();
        File.WriteAllBytes(path, png);

        //Ref removal for garbage collection
        camera.targetTexture = null;

        //Replace original texture
        RenderTexture.active = currRendTex;

    }

    public string GetSafePath(string path)
    {
        return string.Join("_", path.Split(Path.GetInvalidPathChars()));
    }

    public string GetSafeFilename(string filename)
    {
        return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
    }

    //Setters
    public void SetFolderName(string newName)
    {
        folder = newName;
    }

    public void SetFilename(string newName)
    {
        prefix = newName;
    }

    //Getters
    public string GetFolderName()
    {
        return folder;
    }

    public string GetFilename()
    {
        return prefix;
    }
}
