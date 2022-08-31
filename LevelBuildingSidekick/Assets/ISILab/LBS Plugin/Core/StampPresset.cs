using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ISILab/LBS plugin/Presets/Stamp brush")]
public class StampPresset : ScriptableObject // tambien puede ser stamp brush
{
    [SerializeField] private Texture2D icon;
    [SerializeField] private List<string> tags;
    [SerializeField] private List<GameObject> Prefabs; // estos podria ser una clase propia que obligemos a poner a los prefs 

    public Texture2D Icon => icon;
}
