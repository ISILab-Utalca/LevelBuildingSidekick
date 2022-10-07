using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ISILab/LBS plugin/Presets/Stamp brush")]
public class StampPresset : ScriptableObject // tambien puede ser stamp brush
{
    [SerializeField] private string label;
    [SerializeField] private Texture2D icon;
    [SerializeField] private List<string> tags = new List<string>();
    [SerializeField] private List<GameObject> Prefabs = new List<GameObject>(); // estos podria ser una clase propia que obligemos a poner a los prefs 

    public string Label => label;
    public Texture2D Icon => icon;
    public List<string> Tags => new List<string>(tags);
}
