using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModuleTexturizer
{
    public abstract Texture2D ToTexture(LBSModule module);
}
