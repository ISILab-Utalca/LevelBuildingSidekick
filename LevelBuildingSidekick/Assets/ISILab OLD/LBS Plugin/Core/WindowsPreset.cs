using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New WindowsPreset", menuName = "ISILab/LBS plugin/Presets/Windows preset", order =0)]
public class WindowsPreset : ScriptableObject
{
    [SerializeField]
    private Texture2D thumnail;

    [SerializeField] 
    private List<string> windows = new List<string>();

    public Texture2D Thumnail => thumnail;
    public List<string> Windows => new List<string>(windows); // no se puede pedir la lista entera para editarla solo para leerla

    public void AddWindow(string idWindow)
    {
        windows.Add(idWindow);
    }

    public void RemoveWindow(string idWindow)
    {
        windows.Remove(idWindow);
    }
}
