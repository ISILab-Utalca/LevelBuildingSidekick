using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Rule
{
    public abstract bool Verify(object target);
}
